using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    public class SkcPage
    {
        public const int PAGE_SHOW = 1;	//		見えるか
        public const int PAGE_PRINT = 2;	//		印刷できるか

        public string Name = "page";
        public int Flag = PAGE_SHOW | PAGE_PRINT;
        public string LastUsedSheet = "";
        public string LastUsedLayer = "";
        public List<SkcLayer> Layers { get; } = new();
        public List<SkcSheet> Sheets { get; } = new();

        internal void Write(SkcWriter w) 
        {
            w.WriteString("NAME", Name);
            w.Write("FLAG", Flag, true);
            w.WriteObject("LASTUSED", false, w =>
            {
                w.WriteString("SHEET", LastUsedSheet);
                w.WriteString("LAYER", LastUsedLayer);
            }, true);
            w.WriteObjects("LAYERS", Layers, (w, layer) =>
            {
                w.WriteObject("LAYER", false, w =>
                {
                    w.WriteString("NAME", layer.Name);
                    w.Write<int>("FLAG", layer.Flag);
                });
            }, true);
            w.WriteObjects("SHEETS", Sheets, (w, sheet) =>
            {
                w.WriteObject("SHEET", true, w =>
                {
                    sheet.Write(w);
                }, false);

            }, true);
        }
    }
}
