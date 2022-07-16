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
        public SkcDocument()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public SkcFileInfo FileInfo { get; } = new();
        public SkcPaper PaperInfo { get; } = new();
        public bool Horz { get; set; } = true;
        public int LastUsedPage { get; set; } = 0;
        /// <summary>
        /// ページオブジェクトのリスト
        /// </summary>
        public List<SkcPage> Pages { get; } = new();

        /// <summary>
        /// SKCファイルか調べます。ファイル先頭のヘッダ一行で判断します。
        /// </summary>
        public static bool IsSkcFile(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var r = new StreamReader(path, Encoding.GetEncoding("shift_jis"));
            var line = r.ReadLine();
            return line == "$$SakraCadText$$";
        }

        /// <summary>
        /// ページ数を返す。ファイル全体を読むのでそれなりに遅いので注意。
        /// </summary>
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

        /// <summary>
        /// 0から始まる[pageIndex]で指定するページを読み込みます。
        /// 指定したページが読み込めた場合はtrueを返します。
        /// </summary>
        public bool ReadSinglePage(string path, int pageIndex)
        {
            using var r = new StreamReader(path, Encoding.GetEncoding("shift_jis"));
            var line = r.ReadLine();
            if (line != "$$SakraCadText$$") throw new Exception("Not SakraCad format");
            var reader = new SkcReader(r);
            var f = false;
            reader.ReadTag("FILEINFO", reader => FileInfo.Read(reader));
            reader.ReadTag("PAPER", reader => PaperInfo.Read(reader));
            reader.ReadTag("LASTUSED", ReadLastUsedSection);
            reader.ReadTag("PAGES", reader => { 
                f = ReadPagesSection(reader, pageIndex);
            });
            return f;
        }

        /// <summary>
        /// 全てのページを読み込みます。
        /// </summary>
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

        public void Write(string path)
        {
            using var w = new StreamWriter(path, false, Encoding.GetEncoding("shift_jis"));
            Write(w);
        }

        public void Write(TextWriter writer)
        {
            writer.WriteLine("$$SakraCadText$$");
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

        bool ReadPagesSection(SkcReader reader, int pageIndex)
        {
            var index = 0;
            LastUsedPage = 0;
            var f = false;
            reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
            {
                { "PAGE", (reader)=> {
                   if(index == pageIndex) {
                        ReadPage(reader);
                        f = true;
                    }
                    else
                    {
                        reader.SkipTag();
                    }
                   index++;
                } },
            });
            return f;
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
