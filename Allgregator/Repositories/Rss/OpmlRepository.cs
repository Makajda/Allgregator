using Allgregator.Models.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cink = System.Tuple<string, System.Collections.Generic.List<Allgregator.Models.Rss.Link>>;

namespace Allgregator.Repositories.Rss {
    public class OpmlRepository {
        private readonly ChapterRepository chapterRepository;
        private readonly LinkedRepository linkedRepository;

        public OpmlRepository(
            ChapterRepository collectionRepository,
            LinkedRepository linkedRepository
            ) {
            this.chapterRepository = collectionRepository;
            this.linkedRepository = linkedRepository;
        }

        public async Task<(int Chapters, int Links)> Import() {
            var cinks = await ImportPicker();
            if (cinks == null || cinks.Count == 0) {
                return default;
            }

            var chapters = await chapterRepository.GetOrDefault();

            foreach (var cink in cinks.Where(n => n.Item2.Count > 0)) {
                var collection = await chapterRepository.AddCollection(cink.Item1);
                await linkedRepository.AddLinks(collection.Id, cink.Item2);
            }

            var resval = new Tuple<int, int>(cinks.Count, cinks.SelectMany(n => n.Item2).Count());
            return resval;
        }

        public async Task Export() {
            var collections = await chapterRepository.GetCollections();
            var cinks = new List<Cink>();
            foreach (var collection in collections) {
                var elinks = await linkedRepository.GetLinks(collection.Id);
                var links = elinks == null ? new List<Link>() : elinks.ToList();
                cinks.Add(new Cink(collection.Name, links));
            }

            await ExportPicker(cinks);
        }

        private async Task<List<Cink>> ImportPicker() {
            var fileOpenPicker = new FileOpenPicker() {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            fileOpenPicker.FileTypeFilter.Add(".opml");
            var storageFile = await fileOpenPicker.PickSingleFileAsync();
            if (storageFile == null) {
                return null;
            }

            using (var fileStream = await storageFile.OpenStreamForReadAsync()) {
                var xdocument = XDocument.Load(fileStream);
                return OpmlParse(xdocument);
            }
        }

        private async Task ExportPicker(List<Cink> cinks) {
            const string filename = "infeed";
            var fileSavePicker = new FileSavePicker() {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            fileSavePicker.FileTypeChoices.Add("opml", new List<string>() { ".opml" });
            fileSavePicker.SuggestedFileName = filename;
            var storageFile = await fileSavePicker.PickSaveFileAsync();
            if (storageFile != null) {
                var xdocument = ToOpml(cinks);
                using (var fileStream = await storageFile.OpenStreamForWriteAsync()) {
                    xdocument.Save(fileStream, SaveOptions.DisableFormatting);
                }
            }
        }

        private XDocument ToOpml(List<Cink> cinks) {
            var doc = new XDocument();
            var root = new XElement("opml");
            root.Add(new XAttribute("version", "1.0"));
            var head = new XElement("head");
            var title = new XElement("title") { Value = $"from infeed {DateTime.Now.ToString()}" };
            head.Add(title);
            root.Add(head);
            var body = new XElement("body");
            root.Add(body);
            foreach (var cink in cinks) {
                var collection = new XElement("outline");
                collection.Add(new XAttribute("text", cink.Item1 ?? string.Empty));
                body.Add(collection);
                if (cink.Item2 != null) {
                    foreach (var link in cink.Item2) {
                        var outline = new XElement("outline");
                        outline.Add(new XAttribute("type", "rss"));
                        outline.Add(new XAttribute("text", link.Text ?? string.Empty));
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
            var resval = new List<Cink>();
            var xe = xdocument.Element("opml");
            if (xe == null) {
                return resval;
            }

            xe = xe.Element("body");
            if (xe == null) {
                return resval;
            }

            var outlines = xe.Elements();
            if (outlines == null) {
                return resval;
            }

            resval.Add(new Cink(null, new List<Link>())); // to current collection

            foreach (var outline in outlines) {
                var link = LinkParse(outline);
                if (link.XmlUrl != null) {
                    resval[0].Item2.Add(link);
                }
                else {
                    var os = outline.Elements();
                    if (os != null) {
                        var links = new List<Link>();
                        resval.Add(new Cink(link.Text, links)); // to new collection
                        foreach (var o in os) {
                            link = LinkParse(o);
                            if (link.XmlUrl != null) {
                                links.Add(link);
                            }
                        }
                    }
                }
            }

            return resval;
        }

        private Link LinkParse(XElement xe) {
            var xmlUrl = xe.Attribute("xmlUrl")?.Value;
            var text = xe.Attribute("text")?.Value;
            var htmlUrl = xe.Attribute("htmlUrl")?.Value;
            return new Link() { XmlUrl = xmlUrl, Name = text, HtmlUrl = htmlUrl };
        }
    }
}
