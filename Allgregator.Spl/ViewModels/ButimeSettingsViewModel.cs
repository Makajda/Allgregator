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
            var task = GetTask();
            if (task != null) {
                ScheduleOn = task.Definition.Settings.Enabled;
                ScheduleOn = task.Enabled;
                var trigger = GetTrigger(task);
                if (trigger != null)
                    ScheduleInterval = trigger.Repetition.Interval.TotalMinutes;
            }
        }

        private DelegateCommand<string> addCommand; public ICommand AddCommand => addCommand ??= new DelegateCommand<string>(Add);
        private DelegateCommand scheduleCommand; public ICommand ScheduleCommand => scheduleCommand ??= new DelegateCommand(Schedule);

        private bool scheduleOn;
        public bool ScheduleOn {
            get { return scheduleOn; }
            set { SetProperty(ref scheduleOn, value); }
        }

        private double scheduleInterval;
        public double ScheduleInterval {
            get { return scheduleInterval; }
            set { SetProperty(ref scheduleInterval, value, OnScheduleIntervalChanged); }
        }

        private Task GetTask() {
            var task = TaskService.Instance.RootFolder.Tasks.FirstOrDefault(n => n.Name == taskName);
            return task;
        }

        private TimeTrigger GetTrigger(Task task) {
            var trigger = task?.Definition.Triggers.FirstOrDefault(n => n.GetType() == typeof(TimeTrigger)) as TimeTrigger;
            return trigger;
        }

        private void OnScheduleIntervalChanged() {
            if (ScheduleOn) {
                ScheduleOn = false;
                var task = GetTask();
                if (task != null)
                    task.Enabled = false;
            }
        }

        private void Add(string name) {
            if (!string.IsNullOrEmpty(name)) {
                Data.Mined.Butasks.Insert(0, new Butask { Name = name });
            }
        }

        private void Schedule() {
            try {
                if (ScheduleOn) {
                    var task = GetTask();
                    if (task != null) {
                        task.Enabled = false;
                    }

                    ScheduleOn = false;
                }
                else {
                    if (ScheduleInterval > 0) {
                        var task = GetTask();
                        if (task == null) {
                            CreateTask();
                        }
                        else {
                            task.Enabled = true;
                            var trigger = GetTrigger(task);
                            if (trigger == null) {
                                TaskService.Instance.RootFolder.DeleteTask(task.Name);
                                CreateTask();
                            }
                            else
                                trigger.Repetition.Interval = TimeSpan.FromMinutes(ScheduleInterval);
                        }

                        ScheduleOn = true;
                    }
                }
            }
            catch (Exception exception) {
                MessageBox.Show(exception.Message, "Schedule turns error");
            }
        }

        private void CreateTask() {
            var action = new ExecAction(Process.GetCurrentProcess().MainModule.FileName);
            var taskDefinition = TaskService.Instance.NewTask();
            taskDefinition.Actions.Add(action);
            var trigger = new TimeTrigger();
            trigger.StartBoundary = DateTime.Now;
            trigger.Repetition.Interval = TimeSpan.FromMinutes(ScheduleInterval);
            taskDefinition.Triggers.Add(trigger);
            taskDefinition.Validate(true);
            TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, taskDefinition);
        }
    }
}
