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
        public SkcPaper PaperInfo { get; } = new();
        public bool Horz = true;
        public int LastUsedPage = 0;

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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var r = new StreamReader(path, Encoding.GetEncoding("shift_jis"));
            var line = r.ReadLine();
            return line == "$$SakraCadText$$";
        }

        public static int GetPageCount(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var r = new StreamReader(path, Encoding.GetEncoding("shift_jis"));
            var line = r.ReadLine();
            if (line != "$$SakraCadText$$") throw new Exception("Not SakraCad format");
            var reader = new SkcReader(r);
            int pageSize = 0;
            reader.ReadTags(new()
            {
                {"PAGES", reader =>{
                    reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
                    {
                        { "PAGE", (reader)=> {reader.SkipTag(); pageSize++; } },
                    });
                } }
            });
            return pageSize;
        }

        public void ReadSinglePage(string path, int pageIndex)
        {
            using var r = new StreamReader(path, Encoding.GetEncoding("shift_jis"));
            var line = r.ReadLine();
            if (line != "$$SakraCadText$$") throw new Exception("Not SakraCad format");
            var reader = new SkcReader(r);

            reader.ReadTag("FILEINFO", reader => FileInfo.Read(reader));
            reader.ReadTag("PAPER", reader => PaperInfo.Read(reader));
            reader.ReadTag("LASTUSED", ReadLastUsedSection);
            reader.ReadTag("PAGES", reader => { 
                ReadPagesSection(reader, pageIndex);
            });
        }

        public void Read(string path)
        {
            using var r = new StreamReader(path, Encoding.GetEncoding("shift_jis"));
            var line = r.ReadLine();
            if (line != "$$SakraCadText$$") throw new Exception("Not SakraCad format");
            var reader = new SkcReader(r);
            reader.ReadTag("FILEINFO", reader => FileInfo.Read(reader));
            reader.ReadTag("PAPER", reader => PaperInfo.Read(reader));
            reader.ReadTag("LASTUSED", ReadLastUsedSection);
            reader.ReadTag("PAGES", ReadPagesSection);
        }

        public void Write(TextWriter writer)
        {
            writer.WriteLine("$$SakraCadText$$", Encoding.GetEncoding("shift_jis"));
            var w = new SkcWriter(writer);
            w.WriteObject("FILEINFO", false, w => FileInfo.Write(w), true);
            w.WriteObject("PAPER", false, w => PaperInfo.Write(w), true);
            w.WriteObject("LASTUSED", false, w =>
            {
                w.Write("PAGEINDEX", LastUsedPage);
            }, true);

            w.WriteObjects("PAGES", Pages, (w, page) =>
            {
                w.WriteObject("PAGE", true, w => page.Write(w), false);
            }, true);
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

        void ReadPagesSection(SkcReader reader, int pageIndex)
        {
            var index = 0;
            LastUsedPage = 0;
            reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
            {
                { "PAGE", (reader)=> {
                   if(index == pageIndex) {
                        ReadPage(reader);
                    }
                    else
                    {
                        reader.SkipTag();
                    }
                   index++;
                } },
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
                        { "SHAPES", (reader)=> SkcShapeManager.ReadShapes(reader, shapes) },
                    });
                    sheet.AddRange(name, shapes);
                }}
            }); ;
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
