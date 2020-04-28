﻿using Prism.Mvvm;
using System;
using System.Windows;

namespace Allgregator.Aux.Models {
    public class Settings : BindableBase {
        public double MainWindowLeft { get; set; }
        public double MainWindowTop { get; set; }
        public double MainWindowWidth { get; set; }
        public double MainWindowHeight { get; set; }
        public WindowState MainWindowState { get; set; }
        public bool MainWindowTopmost { get; set; }
        public int CurrentChapterId { get; set; }

        private int rssMaxOpenTabs = 12;
        public int RssMaxOpenTabs {
            get { return rssMaxOpenTabs; }
            set { SetProperty(ref rssMaxOpenTabs, value); }
        }

        private DateTimeOffset rssCutoffTime;
        public DateTimeOffset RssCutoffTime {
            get => rssCutoffTime;
            set => SetProperty(ref rssCutoffTime, value);
        }

        private DateTimeOffset finStartDate;
        public DateTimeOffset FinStartDate {
            get { return finStartDate; }
            set { SetProperty(ref finStartDate, value); }
        }
    }
}