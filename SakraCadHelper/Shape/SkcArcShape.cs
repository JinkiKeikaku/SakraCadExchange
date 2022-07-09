using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcArcShape : SkcShape
    {
        public SkcPoint P0 = new();
        public double Radius = 0.0;
        public double Flat = 1.0;
        public double Angle = 0.0;
        public double Start = 0.0;
        public double End = 360.0;
        public int Flag = 0;

        public int LineColor = 0;
        public double LineWidth = 0.0;
        public int LineStyle = 0;
        public SkcArrowAttribute StartArrow = new();
        public SkcArrowAttribute EndArrow = new();
        public SkcFaceColor? FaceColor = null;

        public override string Name => "ARC";
        public override SkcShape Create() => new SkcArcShape();

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
                        { "START", (reader)=> Start = reader.ReadDouble()},
                        { "END", (reader)=> End = reader.ReadDouble()},
                        { "FLAG", (reader)=> Flag = reader.ReadInt()},
                    })
                },
                { "ATTR", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "LC", (reader)=> LineColor = reader.ReadInt()},
                        { "LW", (reader)=> LineWidth = reader.ReadDouble()},
                        { "LS", (reader)=> LineStyle = reader.ReadInt()},
                        { "SA",  (reader)=> StartArrow.Read(reader)},
                        { "EA",  (reader)=> EndArrow.Read(reader)},
                        {"FC", (reader)=> FaceColor = ReadFaceColor(reader)},
                    })
                },
            });
        }

        public override void Write(SkcWriter w)
        {
            w.WriteObject("PARAM", false, w =>
            {
                w.Write("P0", P0);
                w.Write("RADIUS", Radius);
                w.Write("FLAT", Flat, 1.0);
                w.Write("ANGLE", Angle, 0.0);
                w.Write("FLAG", Flag, 0);
                w.Write("START", Start);
                w.Write("END", End);
            });
            w.WriteObject("ATTR", false, w =>
            {
                WriteLineColor(w, "LC", LineColor);
                WriteLineWidth(w, "LW", LineWidth);
                WriteLineStyle(w, "LS", LineStyle);
                WriteFaceColor(w, "FC", FaceColor);
                WriteArrow(w, "SA", StartArrow);
                WriteArrow(w, "EA", EndArrow);
            });
            //w.Write($"PARAM(P0({P0})RADIUS({Radius})");
            //if (Flat != 1.0) w.Write($"FLAT({Flat})");
            //if (Angle != 0.0) w.Write($"ANGLE({Angle})");
            //if (Flag != 0) w.Write($"Flag({Flag})");
            //w.Write($"START({Start})");
            //w.Write($"END({End})");
            //w.Write($")");

            //w.Write($"ATTR(");
            //WriteLineColor(w, "LC", LineColor);
            //WriteLineWidth(w, "LW", LineWidth);
            //WriteLineStyle(w, "LS", LineStyle);
            //WriteArrow(w, "SA", StartArrow);
            //WriteArrow(w, "EA", EndArrow);
            //WriteFaceColor(w, "FC", FaceColor);
            //w.Write($")");
        }

    }
}
