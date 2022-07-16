using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public abstract class SkcShape
    {

        internal abstract string Name { get; }
        internal abstract SkcShape Create();
        internal abstract void Read(SkcReader reader);
        internal abstract void Write(SkcWriter w);

        internal static SkcFaceColor? ReadFaceColor(SkcReader reader)
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
        internal static void WriteFaceColor(SkcWriter w, SkcFaceColor fc)
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
            }
        }

        internal static void WriteFaceColor(SkcWriter w, string tag, SkcFaceColor? fc)
        {
            if (fc != null)
            {
                w.WriteObject(tag, false, w =>
                {
                    WriteFaceColor(w, fc);
                });
            }
        }

        internal static void WriteArrow(SkcWriter w, string tag, SkcArrowAttribute arrow)
        {
            if (arrow.ID != 0)
            {
                w.WriteObject(tag, false, w => arrow.Write(w));
            //                w.Write(tag, arrow);
        }
    }

        internal static void WriteLineColor(SkcWriter w, string tag, int color)
        {
            w.Write(tag, color);
        }

        internal static void WriteLineWidth(SkcWriter w, string tag, double width)
        {
            w.Write(tag, width, 0.0);
        }
        internal static void WriteLineStyle(SkcWriter w, string tag, int style)
        {
            w.Write(tag, style, 0);
        }
        internal static void ReadVertex(SkcReader reader, List<SkcPoint> vertex)
        {
            reader.ReadTags(new()
            {
                { "V", (reader)=> vertex.Add(reader.ReadPoint())},
            });
        }

        internal static void WriteVertex(SkcWriter w, string tag, IReadOnlyList<SkcPoint> vertex)
        {
            w.WriteObjects(tag, vertex, (w, p) =>
            {
                w.Write("V", p);
            }, true);
        }

    }
}
