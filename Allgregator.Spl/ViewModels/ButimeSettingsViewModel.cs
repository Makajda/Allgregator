using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Microsoft.Win32.TaskScheduler;
using Prism.Commands;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Allgregator.Spl.ViewModels {
    public class ButimeSettingsViewModel : DataViewModelBase<DataBase<Bumined>> {
        private const string taskName = "AllgregatorButime";
        public ButimeSettingsViewModel() {
            try {
                var task = TaskService.Instance.RootFolder.Tasks.FirstOrDefault(n => n.Name == taskName);
                var trigger = task?.Definition.Triggers.FirstOrDefault(n => n.GetType() == typeof(TimeTrigger)) as TimeTrigger;
                if (trigger != null) {
                    ScheduleOn = task.Definition.Settings.Enabled;
                    ScheduleInterval = trigger.Repetition.Interval.TotalMinutes;
                }
            }
            catch (Exception exception) {
                MessageBox.Show(exception.Message, "Schedule get error");
            }
        }

        private DelegateCommand<string> addCommand; public ICommand AddCommand => addCommand ??= new DelegateCommand<string>(Add);
        private DelegateCommand scheduleCommand; public ICommand ScheduleCommand => scheduleCommand ??= new DelegateCommand(Schedule);

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

        private void Add(string name) {
            if (!string.IsNullOrEmpty(name)) {
                Data.Mined.Butasks.Insert(0, new Butask { Name = name });
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

        private static void ScheduleRegister(bool scheduleOn, double value) {
            using var action = new ExecAction(Process.GetCurrentProcess().MainModule.FileName);
            using var taskDefinition = TaskService.Instance.NewTask();
            taskDefinition.Settings.Enabled = scheduleOn;
            taskDefinition.Actions.Add(action);
            var trigger = new TimeTrigger();
            trigger.StartBoundary = DateTime.Now;
            trigger.Repetition.Interval = TimeSpan.FromMinutes(value);
            taskDefinition.Triggers.Add(trigger);
            taskDefinition.Validate(true);
            TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, taskDefinition);
        }
    }
}
