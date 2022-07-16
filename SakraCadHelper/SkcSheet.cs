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
        public const int SHEET_SHOW = 256;	    //	見えるか
        public const int SHEET_PRINT = 512;	//  印刷できるか

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

        internal void Write(SkcWriter w)
        {
            w.WriteString("NAME", Name);
            w.Write("FLAG", Flag);
            w.Write("PAPERSCALE", PaperScale, true);
            w.WriteObjects("LAYERS", LayerShapes, (w, ls) =>
            {
                w.WriteObject("LAYER", true, w =>
                {
                    w.WriteString("NAME", ls.Key, true);
                    w.NewLine();
                    SkcShapeManager.WriteShapes(w, "SHAPES", ls.Value);
                }, false);
            }, true);
        }
    }
}
