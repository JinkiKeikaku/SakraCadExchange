using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public abstract class SkcShape
    {
        public abstract string Name { get; }

        public abstract SkcShape Create();
        public abstract void Read(SkcReader reader);
        public abstract void Write(SkcWriter w);

        public static SkcFaceColor? ReadFaceColor(SkcReader reader)
        {
            SkcFaceColor? fc = null;
            if (reader.ReadTag("SOLID", (reader) =>
            {
                var c = reader.ReadInt();
                fc = new SkcSolidColor(c);
            })) return fc;
            reader.ReadTag("GRAD", (reader) =>
            {
                var g = new SkcGradationColor();
                g.Read(reader);
                fc = g;
            });
            return fc;
        }
        public static void WriteFaceColor(SkcWriter w, SkcFaceColor fc)
        {
            if (fc is SkcSolidColor solid)
            {
                w.Write<int>("SOLID", solid.Color);
//                w.Write($"SOLID({solid.Color})");
                return;
            }
            if (fc is SkcGradationColor grad)
            {
                w.WriteObject("GRAD", false, w =>
                {
                    grad.Write(w);
                });
                //w.Write($"GRAD(");
                //w.Write($")");
            }
        }

        public static void WriteFaceColor(SkcWriter w, string tag, SkcFaceColor? fc)
        {
            if (fc != null)
            {
                w.WriteObject(tag, false, w =>
                {
                    WriteFaceColor(w, fc);
                });
                //w.Write($"{tag}(");
                //w.Write($")");
            }
        }

        public static void WriteArrow(SkcWriter w, string tag, SkcArrowAttribute arrow)
        {
            if (arrow.ID != 0)
            {
                w.Write<SkcArrowAttribute>(tag, arrow);
//                w.Write($"{tag}({arrow})");
            }
        }

        public static void WriteLineColor(SkcWriter w, string tag, int color)
        {
            w.Write<int>(tag, color);
//            w.Write($"{tag}({color})");
        }

        public static void WriteLineWidth(SkcWriter w, string tag, double width)
        {
            w.Write<double>(tag, width, 0.0);
//            if (width != 0.0) w.Write($"{tag}({width})");
        }
        public static void WriteLineStyle(SkcWriter w, string tag, int style)
        {
            w.Write<int>(tag, style, 0);
//            if (style != 0) w.Write($"{tag}({style})");
        }
        public static void ReadVertex(SkcReader reader, List<SkcPoint> vertex)
        {
            reader.ReadTags(new()
            {
                { "V", (reader)=> vertex.Add(reader.ReadPoint())},
            });
        }

        public static void WriteVertex(SkcWriter w, string tag, IReadOnlyList<SkcPoint> vertex)
        {
            w.WriteObjects<SkcPoint>(tag, vertex, (w, p) =>
            {
                w.Write<SkcPoint>("V", p);
//                w.WriteLine($"V({v})");
            }, true);
            //w.WriteLine($"{tag}(");
            //foreach(var v in vertex)
            //{
            //    w.WriteLine($"V({v})");
            //}
            //w.WriteLine($")");
        }

        //public static void WriteString(TextWriter w, string tag, string text)
        //{
        //    var s = text.Replace("\"", "\"\"");
        //    w.Write($"{tag}(\"{s}\")");
        //}


    }
}
