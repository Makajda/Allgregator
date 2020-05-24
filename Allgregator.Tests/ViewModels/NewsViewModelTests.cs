using NUnit.Framework;
using Allgregator.Rss.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Allgregator.Rss.Models;
using Moq;
using System.Collections.ObjectModel;
using System.Linq;

namespace Allgregator.Rss.ViewModels.Tests {
    [TestFixture()]
    public class NewsViewModelTests {
        private Reco reco;
        private NewsViewModel viewModel;

        [SetUp]
        public void Setup() {
            reco = new Reco() { ItemTitle = "Test" };

            var newRecos = new ObservableCollection<Reco>() {
                new Reco(),
                reco,
                new Reco(),
                new Reco()
            };
            var oldRecos = new ObservableCollection<Reco>() {
                new Reco(),
                new Reco(),
                new Reco(),
                new Reco()
            };

            var random = new Random();
            var mined = new Mined() {
                LastRetrieve = DateTimeOffset.Now.AddDays(random.Next(-100, 100)),
                AcceptTime = DateTimeOffset.Now.AddDays(random.Next(-100, 100)),
                NewRecos = newRecos,
                OldRecos = oldRecos
            };

            viewModel = new NewsViewModel() { Data = new Data() { Mined = mined } };
        }

        [Test()]
        public void NewsViewModelTest_Move() {
            var newsCount = viewModel.Data.Mined.NewRecos.Count;
            var oldsCount = viewModel.Data.Mined.OldRecos.Count;

            //Act
            viewModel.MoveCommand.Execute(reco);

            Assert.That(viewModel.Data.Mined.NewRecos.Count, Is.EqualTo(newsCount - 1));
            Assert.That(viewModel.Data.Mined.OldRecos.Count, Is.EqualTo(oldsCount + 1));
            Assert.That(viewModel.Data.Mined.IsNeedToSave, Is.True);
            Assert.That(viewModel.Data.Mined.NewRecos, !Contains.Item(reco));
            Assert.That(viewModel.Data.Mined.OldRecos, Contains.Item(reco));
        }

        [Test()]
        public void NewsViewModelTest_MoveLast() {
            viewModel.Data.Mined.NewRecos.Clear();

            //Act
            viewModel.MoveCommand.Execute(reco);

            Assert.That(viewModel.Data.Mined.LastRetrieve, Is.EqualTo(viewModel.Data.Mined.AcceptTime));
        }
    }
}