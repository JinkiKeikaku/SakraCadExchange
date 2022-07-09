using SakraCadHelper.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    public class SkcDocument
    {
        public SkcFileInfo FileInfo { get; } = new();
        public SkcPaper SkcPaper { get; } = new();
        public bool Horz = true;
        public int LastUsedPage = 1;

        public SkcDocument()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public List<SkcPage> Pages { get; } = new();

        /// <summary>
        /// SKCファイルか調べます。
        /// </summary>
        public static bool IsSkcFile(string path)
        {
            using var r = new StreamReader(path, Encoding.GetEncoding("shift_jis"));
            var line = r.ReadLine();
            return line == "$$SakraCadText$$";
        }

        public void Read(string path)
        {
            using var r = new StreamReader(path, Encoding.GetEncoding("shift_jis"));
            var line = r.ReadLine();
            if (line != "$$SakraCadText$$") throw new Exception("Not SakraCad format");
            var reader = new SkcReader(r);
            reader.ReadTag("FILEINFO", ReadFileInfoSection);
            reader.ReadTag("PAPER", ReadPaperSection);
            reader.ReadTag("LASTUSED", ReadLastUsedSection);
            reader.ReadTag("PAGES", ReadPagesSection);
        }

        public void Write(TextWriter writer)
        {
            writer.WriteLine("$$SakraCadText$$", Encoding.GetEncoding("shift_jis"));
            var w = new SkcWriter(writer);
            w.Write<SkcFileInfo>("FILEINFO", FileInfo, true);
            w.Write<SkcPaper>("PAPER", SkcPaper, true);
            w.WriteObject("LASTUSED", false, w =>
            {
                w.Write<int>("PAGEINDEX", LastUsedPage);
            }, true);

            w.WriteObjects<SkcPage>("PAGES", Pages, (w, page) =>
            {
                w.WriteObject("PAGE", true, w =>
                {
                    page.Write(w);
                }, false);
                //writer.Write($"PAGE(");
                //page.Writer(writer);
                //writer.WriteLine($")");
            }, true);
        }

        void ReadFileInfoSection(SkcReader reader)
        {
            reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
            {
                { "ENCODING", (reader)=>FileInfo.EncodingName = reader.ReadString() },
                { "GENERATOR", (reader)=>FileInfo.GeneratorName = reader.ReadString() },
                { "SKCVERSION", (reader)=>FileInfo.SkcVersion = reader.ReadString() },
            });
        }

        void ReadPaperSection(SkcReader reader)
        {
            reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
            {
                { "PAPERSIZE", (reader)=>{
                    reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
                    {
                        { "NAME", (reader)=> SkcPaper.Name = reader.ReadString() },
                        { "WIDTH", (reader)=> SkcPaper.Width = reader.ReadDouble() },
                        { "HEIGHT", (reader)=> SkcPaper.Height = reader.ReadDouble() },
                    });
                }},
                { "HORZ", (reader)=> SkcPaper.Horz = reader.ReadInt() != 0 },
            });
        }

        void ReadLastUsedSection(SkcReader reader)
        {
            reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
            {
                { "PAGEINDEX", (reader)=> LastUsedPage = reader.ReadInt() },
            });
        }

        void ReadPagesSection(SkcReader reader)
        {
            reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
            {
                { "PAGE", (reader)=> ReadPage(reader) },
            });
        }

        void ReadPage(SkcReader reader)
        {
            var page = new SkcPage();

            reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
            {
                { "NAME", (reader)=> page.Name = reader.ReadString() },
                { "FLAG", (reader)=> page.Flag = reader.ReadInt() },
                { "LASTUSED", (reader)=>{
                    reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
                    {
                        { "SHEET", (reader)=> page.LastUsedSheet = reader.ReadString() },
                        { "LAYER", (reader)=> page.LastUsedLayer = reader.ReadString() },
                    });
                } },
                { "LAYERS", (reader)=> ReadLayers(reader, page) },
                { "SHEETS", (reader)=> ReadSheets(reader, page) },
            });
            Pages.Add(page);
        }

        void ReadLayers(SkcReader reader, SkcPage page)
        {
            reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
            {
                { "LAYER", (reader)=> {
                    var layer = new SkcLayer();
                    reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
                    {
                        { "NAME", (reader)=> layer.Name = reader.ReadString() },
                        { "FLAG", (reader)=> layer.Flag = reader.ReadInt() },
                    });
                    page.Layers.Add(layer);
                } },
            });
        }

        void ReadSheets(SkcReader reader, SkcPage page)
        {
            reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
            {
                { "SHEET", (reader)=> {
                    var sheet = new SkcSheet();
                    reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
                    {
                        { "NAME", (reader)=> sheet.Name = reader.ReadString() },
                        { "FLAG", (reader)=> sheet.Flag = reader.ReadInt() },
                        { "PAPERSCALE", (reader)=> ReadPaperScale(reader, sheet) },
                        { "LAYERS", (reader)=> ReadSheetLayers(reader, sheet) },
                    });
                    page.Sheets.Add(sheet);
                } },
            });
        }

        void ReadSheetLayers(SkcReader reader, SkcSheet sheet)
        {
            reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
            {
                { "LAYER", (reader)=>{
                    string name = "";
                    List<SkcShape> shapes = new();
                    reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
                    {
                        { "NAME", (reader)=> name = reader.ReadString() },
                        { "SHAPES", (reader)=> ReadShapes(reader, shapes) },
                    });
                    sheet.AddRange(name, shapes);
                }}
            }); ;
        }

        void ReadShapes(SkcReader reader, List<SkcShape> shapes)
        {
            reader.ReadTags(new Dictionary<string, Action<SkcReader>>(), (reader, tag) =>
            {
                var s = SkcShapeManager.Create(tag);
                if (s == null)
                {
                    reader.SkipTag();
                }
                else
                {
                    s.Read(reader);
                    shapes.Add(s);
                }
            });
        }

        void ReadPaperScale(SkcReader reader, SkcSheet sheet)
        {
            reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
            {
                { "NAME", (reader)=> sheet.PaperScale.Name = reader.ReadString() },
                { "NUMERATOR", (reader)=> sheet.PaperScale.Numerator = reader.ReadDouble() },
            });
        }
    }
}
