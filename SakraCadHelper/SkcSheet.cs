using SakraCadHelper.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    public class SkcSheet
    {
        const int SHEET_SHOW = 256;	    //	見えるか
        const int SHEET_PRINT = 512;	//  印刷できるか

        public string Name = "";
        public int Flag = SHEET_SHOW | SHEET_PRINT;
        public SkcPaperScale PaperScale { get; } = new();
        public Dictionary<string, List<SkcShape>> LayerShapes = new();

        public void Add(string layerName, SkcShape s)
        {
            if (!LayerShapes.ContainsKey(layerName))
            {
                LayerShapes.Add(layerName, new List<SkcShape>());
            }
            LayerShapes[layerName].Add(s);
        }
        public void AddRange(string layerName, List<SkcShape> shapes)
        {
            if (!LayerShapes.ContainsKey(layerName))
            {
                LayerShapes.Add(layerName, new List<SkcShape>());
            }
            LayerShapes[layerName].AddRange(shapes);
        }

        public void Write(SkcWriter w)
        {
            w.WriteString("NAME", Name);
            w.Write<int>("FLAG", Flag);
            w.Write<SkcPaperScale>("PAPERSCALE", PaperScale, true);
            w.WriteObjects("LAYERS", LayerShapes, (w, ls) =>
            {
                w.WriteObject("LAYER", true, w =>
                {
                    w.WriteString("NAME", ls.Key, true);
                    w.NewLine();
                    w.WriteObjects("SHAPES", ls.Value, (w, s) =>
                    {
                        SkcShapeManager.WriteShape(w, s);
                    }, true);

                }, false);
            }, true);
//            w.WriteLine($"NAME(\"{Name}\")FLAG({Flag})");
//            w.WriteLine($"PAPERSCALE({PaperScale})");
            //w.WriteLine($"LAYERS(");
            //foreach (var ls in LayerShapes)
            //{
            //    w.WriteLine($"LAYER(NAME(\"{ls.Key}\")");
            //    w.WriteLine($"SHAPES(");
            //    foreach(var s in ls.Value)
            //    {
            //        SkcShapeManager.WriteShape(w, s);
            //    }
            //    w.WriteLine($")");
            //    w.WriteLine($")");
            //}
            //w.WriteLine($")");
        }
    }
}
