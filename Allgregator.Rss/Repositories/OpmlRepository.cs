using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Microsoft.Win32;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Allgregator.Rss.Repositories {
    internal class OpmlRepository {
        private readonly ChapterRepository chapterRepository;
        private readonly LinkedRepository linkedRepository;
        private readonly IEventAggregator eventAggregator;
        public OpmlRepository(
            ChapterRepository chapterRepository,
            LinkedRepository linkedRepository,
            IEventAggregator eventAggregator
            ) {
            this.chapterRepository = chapterRepository;
            this.linkedRepository = linkedRepository;
            this.eventAggregator = eventAggregator;
        }

        internal async Task<(int Chapters, int Links)> Import() {
            var cinks = await ImportPicker();
            if (cinks == null || cinks.Count == 0) {
                return default;
            }

            var chapters = (await chapterRepository.GetOrDefault()).ToList();
            var newChapters = new Chapter[cinks.Count];
            var indexChapters = 0;

            try {
                foreach (var cink in cinks) {
                    var newId = chapterRepository.GetNewId(chapters);
                    var newChapter = new Chapter() { Id = newId, Name = cink.Name };
                    newChapters[indexChapters++] = newChapter;
                    chapters.Add(newChapter);
                    var linked = await linkedRepository.GetOrDefault(newId);
                    linked.Links = cink.Links;
                    await linkedRepository.Save(newId, linked);
                }
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            eventAggregator.GetEvent<ChapterAddedEvent>().Publish(newChapters);

            return (cinks.Count, cinks.SelectMany(n => n.Links).Count());
        }

        internal async Task Export() {
            var chapters = await chapterRepository.GetOrDefault();
            var cinks = new List<Cink>();
            foreach (var chapter in chapters) {
                var linked = await linkedRepository.GetOrDefault(chapter.Id);
                cinks.Add(new Cink() { Name = chapter.Name, Links = linked.Links });
            }

            await ExportPicker(cinks);
        }

        private async Task<List<Cink>> ImportPicker() {
            var picker = new OpenFileDialog {
                Filter = "opml files (*.opml)|*.opml|All files (*.*)|*.*"
            };

            if (picker.ShowDialog() == true) {
                using var fileStream = picker.OpenFile();
                using var cancellationTokenSource = new CancellationTokenSource();
                var xdocument = await XDocument.LoadAsync(fileStream, LoadOptions.None, cancellationTokenSource.Token);
                return OpmlParse(xdocument);
            }

            return null;
        }

        private async Task ExportPicker(List<Cink> cinks) {
            const string filename = "allgregator";
            var picker = new SaveFileDialog {
                Filter = "opml files (*.opml)|*.opml|All files (*.*)|*.*",
                FileName = filename
            };

            if (picker.ShowDialog() == true) {
                using var fileStream = picker.OpenFile();
                using var cancellationTokenSource = new CancellationTokenSource();
                var xdocument = ToOpml(cinks);
                await xdocument.SaveAsync(fileStream, SaveOptions.DisableFormatting, cancellationTokenSource.Token);
            }
        }

        private XDocument ToOpml(List<Cink> cinks) {
            var doc = new XDocument();
            var root = new XElement("opml");
            root.Add(new XAttribute("version", "1.0"));
            var head = new XElement("head");
            var title = new XElement("title") { Value = $"from infeed {DateTime.Now}" };
            head.Add(title);
            root.Add(head);
            var body = new XElement("body");
            root.Add(body);
            foreach (var cink in cinks) {
                var collection = new XElement("outline");
                collection.Add(new XAttribute("text", cink.Name ?? string.Empty));
                body.Add(collection);
                if (cink.Links != null) {
                    foreach (var link in cink.Links) {
                        var outline = new XElement("outline");
                        outline.Add(new XAttribute("type", "rss"));
                        outline.Add(new XAttribute("text", link.Name ?? string.Empty));
                        outline.Add(new XAttribute("xmlUrl", link.XmlUrl ?? string.Empty));
                        outline.Add(new XAttribute("htmlUrl", link.HtmlUrl ?? string.Empty));
                        collection.Add(outline);
                    }
                }
            }

            doc.Add(root);
            return doc;
        }

        private List<Cink> OpmlParse(XDocument xdocument) {
            var retval = new List<Cink>();
            var xe = xdocument.Element("opml");
            if (xe == null) {
                return retval;
            }

            xe = xe.Element("body");
            if (xe == null) {
                return retval;
            }

            var outlines = xe.Elements();
            if (outlines == null) {
                return retval;
            }

            retval.Add(new Cink() { Links = new ObservableCollection<Link>() }); // to current collection

            foreach (var outline in outlines) {
                var link = LinkParse(outline);
                if (link.XmlUrl != null) {
                    retval[0].Links.Add(link);
                }
                else {
                    var os = outline.Elements();
                    if (os != null) {
                        var links = new ObservableCollection<Link>();
                        retval.Add(new Cink() { Name = link.Name, Links = links }); // to new collection
                        foreach (var o in os) {
                            link = LinkParse(o);
                            if (link.XmlUrl != null) {
                                links.Add(link);
                            }
                        }
                    }
                }
            }

            return retval;
        }

        private Link LinkParse(XElement xe) {
            var xmlUrl = xe.Attribute("xmlUrl")?.Value;
            var text = xe.Attribute("text")?.Value;
            var htmlUrl = xe.Attribute("htmlUrl")?.Value;
            return new Link() { XmlUrl = xmlUrl, Name = text, HtmlUrl = htmlUrl };
        }

        private class Cink {
            public string Name;
            public ObservableCollection<Link> Links;
        }
    }
}
