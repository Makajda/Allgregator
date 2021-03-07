using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Microsoft.Win32.TaskScheduler;
using Prism.Commands;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Allgregator.Spl.ViewModels {
    public class ButimeSettingsViewModel : DataViewModelBase<DataBase<Bumined>> {
        private const string taskName = "AllgregatorButime";
        private readonly DialogService dialogService;
        public ButimeSettingsViewModel(
            DialogService dialogService
            ) {
            this.dialogService = dialogService;

            try {
                var task = TaskService.Instance.RootFolder.Tasks.FirstOrDefault(n => n.Name == taskName);
                if (task?.Definition.Triggers.FirstOrDefault(n => n.GetType() == typeof(TimeTrigger)) is TimeTrigger trigger) {
                    ScheduleOn = task.Definition.Settings.Enabled;
                    ScheduleInterval = trigger.Repetition.Interval.TotalMinutes;
                }
            }
            catch (Exception exception) {
                MessageBox.Show(exception.Message, "Schedule get error");
            }
        }

        private DelegateCommand addCommand; public ICommand AddCommand => addCommand ??= new DelegateCommand(Add);
        private DelegateCommand scheduleCommand; public ICommand ScheduleCommand => scheduleCommand ??= new DelegateCommand(Schedule);
        private DelegateCommand<Butask> selectColorCommand; public ICommand SelectColorCommand => selectColorCommand ??= new DelegateCommand<Butask>(SelectColor);
        private DelegateCommand<Butask> moveUpCommand; public ICommand MoveUpCommand => moveUpCommand ??= new DelegateCommand<Butask>(MoveUp);
        private DelegateCommand<Butask> moveDownCommand; public ICommand MoveDownCommand => moveDownCommand ??= new DelegateCommand<Butask>(MoveDown);
        private DelegateCommand<Butask> deleteCommand; public ICommand DeleteCommand => deleteCommand ??= new DelegateCommand<Butask>(Delete);

        private string newName;
        public string NewName {
            get { return newName; }
            set { SetProperty(ref newName, value); }
        }

        private bool scheduleOn;
        public bool ScheduleOn {
            get { return scheduleOn; }
            set { SetProperty(ref scheduleOn, value); }
        }

        private double scheduleInterval = 60d;
        public double ScheduleInterval {
            get { return scheduleInterval; }
            set {
                try {
                    if (value <= 0) throw new Exception("interval > 0");//todo validation
                    ScheduleRegister(ScheduleOn, value);
                    SetProperty(ref scheduleInterval, value);
                }
                catch (Exception exception) {
                    MessageBox.Show(exception.Message, "Schedule interval error");
                }
            }
        }

        private void Add() {
            if (!string.IsNullOrEmpty(NewName)) {
                Data.Mined.Butasks.Insert(0, new Butask { Name = NewName });
                NewName = null;
            }
        }

        private void Schedule() {
            try {
                ScheduleRegister(!ScheduleOn, ScheduleInterval);
                ScheduleOn = !ScheduleOn;
            }
            catch (Exception exception) {
                MessageBox.Show(exception.Message, "Schedule turns error");
            }
        }

        private void SelectColor(Butask butask) {
            var colorsTypePropertyInfos = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);
            dialogService.Show(colorsTypePropertyInfos.Select(n => n.GetValue(null)), n => butask.Color = (Color)n);
        }

        private void MoveUp(Butask butask) {
            if (Data?.Mined?.Butasks == null) return;//<-------------------------

            var index = Data.Mined.Butasks.IndexOf(butask) - 1;
            if (index >= 0) {
                Data.Mined.Butasks[index + 1] = Data.Mined.Butasks[index];
                Data.Mined.Butasks[index] = butask;
                Data.Mined.IsNeedToSave = true;
            }
        }

        private void MoveDown(Butask butask) {
            if (Data?.Mined?.Butasks == null) return;//<-------------------------

            var index = Data.Mined.Butasks.IndexOf(butask) + 1;
            if (index < Data.Mined.Butasks.Count) {
                Data.Mined.Butasks[index - 1] = Data.Mined.Butasks[index];
                Data.Mined.Butasks[index] = butask;
                Data.Mined.IsNeedToSave = true;
            }
        }

        private void Delete(Butask butask) {
            dialogService.Show($"{butask?.Name}?", DeleteReal, 20, true);

            void DeleteReal() {
                if (Data?.Mined?.Butasks == null || butask == null) return;//<-------------------------

                Data.Mined.Butasks.Remove(butask);
                Data.Mined.IsNeedToSave = true;
            }
        }

        private static void ScheduleRegister(bool scheduleOn, double value) {
            var filename = Process.GetCurrentProcess().MainModule.FileName;
            using var action = new ExecAction(filename, null, Path.GetDirectoryName(filename));
            using var taskDefinition = TaskService.Instance.NewTask();
            taskDefinition.Settings.Enabled = scheduleOn;
            taskDefinition.Actions.Add(action);
            var trigger = new TimeTrigger {
                StartBoundary = DateTime.Now
            };
            trigger.Repetition.Interval = TimeSpan.FromMinutes(value);
            taskDefinition.Triggers.Add(trigger);
            taskDefinition.Validate(true);
            TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, taskDefinition);
        }
    }
}
