using Allgregator.Models.Rss;
using Allgregator.ViewModels.Rss;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Allgregator.Views.Rss {
    /// <summary>
    /// Interaction logic for RecosView
    /// </summary>
    public partial class RecosView : UserControl {
        public RecosView() {
            InitializeComponent();
        }

        public RecosView(bool isNew)
            : this() {
            IsNew = isNew;
        }

        public bool IsNew {
            get { return (bool)GetValue(IsNewProperty); }
            set { SetValue(IsNewProperty, value); }
        }

        public static readonly DependencyProperty IsNewProperty =
            DependencyProperty.Register("IsNew", typeof(bool), typeof(RecosView), new PropertyMetadata(false, OnIsNewChanged));

        private static void OnIsNewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var view = (RecosView)d;
            var isNew = (bool)e.NewValue;

            var path = string.Format("{0}.{1}.{2}{3}s", nameof(Chapter), nameof(Mined), isNew ? "New" : "Old", nameof(Reco));
            view.list.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(path));
        }
    }
}
