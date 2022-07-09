using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    public class SkcPage
    {
        const int PAGE_SHOW = 1;	//		見えるか
        const int PAGE_PRINT = 2;	//		印刷できるか

        public string Name = "page";
        public int Flag = PAGE_SHOW | PAGE_PRINT;
        public string LastUsedSheet = "";
        public string LastUsedLayer = "";
        public List<SkcLayer> Layers { get; } = new();
        public List<SkcSheet> Sheets { get; } = new();

        public void Write(SkcWriter w) 
        {
            w.WriteString("NAME", Name);
            w.Write<int>("FLAG", Flag, true);
            w.WriteObject("LASTUSED", false, w =>
            {
                w.WriteString("SHEET", LastUsedSheet);
                w.WriteString("LAYER", LastUsedLayer);
            }, true);
            w.WriteObjects<SkcLayer>("LAYERS", Layers, (w, layer) =>
            {
                w.WriteObject("LAYER", false, w =>
                {
                    w.WriteString("NAME", layer.Name);
                    w.Write<int>("FLAG", layer.Flag);
                });
            }, true);
            w.WriteObjects<SkcSheet>("SHEETS", Sheets, (w, sheet) =>
            {
                w.WriteObject("SHEET", true, w =>
                {
                    sheet.Write(w);
                }, false);

            }, true);


                //            w.WriteLine($"NAME(\"{Name}\")FLAG({Flag})");
                //            w.WriteLine($"LASTUSED(SHEET(\"{LastUsedSheet}\")LAYER(\"{LastUsedLayer}\"))");
                //w.WriteLine($"LAYERS(");
                //foreach(var layer in Layers)
                //{
                //    w.Write("LAYER(");
                //    w.Write(layer.ToString());
                //    w.WriteLine(")");
                //}
                //w.WriteLine($")");
            //    w.WriteLine($"SHEETS(");
            //foreach (var sheet in Sheets)
            //{
            //    w.WriteLine($"SHEET(");
            //    sheet.Write(w);
            //    w.WriteLine($")");
            //}
            //w.WriteLine($")");
        }
    }


}
