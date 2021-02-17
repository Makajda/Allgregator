using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Spl.Models;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Allgregator.Spl.ViewModels {
    public class ButimeViewModel : DataViewModelBase<DataBase<MinedBase<Butime>>> {
        private readonly Settings settings;

        public ButimeViewModel(Settings settings, IEventAggregator eventAggregator) {
            this.settings = settings;
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(WindowClosing);

            Butasks = new Obsefy<Butask>();
            Butasks.Add(new Butask { Name = "1" });
            Butasks.Add(new Butask { Name = "2" });
            Butasks.Add(new Butask { Name = "3" });
        }

        private Obsefy<Butask> butasks;
        public Obsefy<Butask> Butasks {
            get { return butasks; }
            set { SetProperty(ref butasks, value); }
        }

        private void WindowClosing(CancelEventArgs obj) {
        }

        void Load() {
            var model = new List<(string, double)>
            {
                ("Пер", 19),
                ("Вто", 1),
                ("Тре", 4),
                ("Чет", 4),
                ("Пят", 10),
                ("Пер", 19),
                ("Вто", 1),
                ("Тре", 4),
                ("Чет", 4),
                ("Пят", 10),
            };

            var canvas = new Canvas();
            var max = model.Max(n => n.Item2);
            //var sum = model.Sum(n => n.Item2);
            var scaleH = canvas.ActualWidth / model.Count;
            var scaleV = canvas.ActualHeight / max;
            Debug.WriteLine($"scaleV={scaleV}, scaleH={scaleH}");

            var l = 0d;
            foreach (var m in model) {
                var b = new ButtonShaatt(Shapes.TopCap) {
                    BorderThickness = new Thickness(0),
                    Content = $"{m.Item1} {m.Item2}",
                    Height = m.Item2 * scaleV,
                    Width = scaleH - 2d,
                };
                Canvas.SetLeft(b, l);
                Canvas.SetTop(b, canvas.ActualHeight - b.Height);
                canvas.Children.Add(b);
                l += scaleH;
            }
        }
    }
}
