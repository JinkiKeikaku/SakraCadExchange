using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcCircleShape : SkcShape
    {
        public SkcPoint P0 = new();
        public double Radius = 0.0;
        public double Flat = 1.0;
        public double Angle = 0.0;

        public int LineColor = 0;
        public double LineWidth = 0.0;
        public int LineStyle = 0;
        public SkcFaceColor? FaceColor = null;

        public override string Name => "CIRCLE";
        public override SkcShape Create() => new SkcCircleShape();

        public override void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "PARAM", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "P0", (reader)=> P0 = reader.ReadPoint()},
                        { "RADIUS", (reader)=> Radius = reader.ReadDouble()},
                        { "FLAT", (reader)=> Flat = reader.ReadDouble()},
                        { "ANGLE", (reader)=> Angle = reader.ReadDouble()},
                    })
                },
                { "ATTR", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "LC", (reader)=> LineColor = reader.ReadInt()},
                        { "LW", (reader)=> LineWidth = reader.ReadDouble()},
                        { "LS", (reader)=> LineStyle = reader.ReadInt()},
                        {"FC", (reader)=> FaceColor = ReadFaceColor(reader)},
                    })
                },
            });
        }

        public override void Write(SkcWriter w)
        {
            w.WriteObject("PARAM", false, w =>
            {
                w.Write<SkcPoint>("P0", P0);
                w.Write("RADIUS", Radius);
                w.Write("FLAT", Flat, 1.0);
                w.Write("ANGLE", Angle, 0.0);
            });
            w.WriteObject("ATTR", false, w =>
            {
                WriteLineColor(w, "LC", LineColor);
                WriteLineWidth(w, "LW", LineWidth);
                WriteLineStyle(w, "LS", LineStyle);
                WriteFaceColor(w, "FC", FaceColor);
            });

            //w.Write($"PARAM(P0({P0})RADIUS({Radius})");
            //if (Flat != 1.0) w.Write($"FLAT({Flat})");
            //if (Angle != 0.0) w.Write($"ANGLE({Angle})");
            //w.Write($")");
            //w.Write($"ATTR(");
            //WriteLineColor(w, "LC", LineColor);
            //WriteLineWidth(w, "LW", LineWidth);
            //WriteLineStyle(w, "LS", LineStyle);
            //WriteFaceColor(w, "FC", FaceColor);
            //w.Write($")");
        }
    }
}
