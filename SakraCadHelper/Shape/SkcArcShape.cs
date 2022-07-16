using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcArcShape : SkcShape
    {
        [Flags]
        public enum ArcFlag
        {
            None = 0,
            Fan = 1,

        }

        public SkcPoint P0 = new();
        public double Radius = 0.0;
        public double Flat = 1.0;
        public double Angle = 0.0;
        public double Start = 0.0;
        public double End = 360.0;
        public ArcFlag Flag = 0;

        public int LineColor = 0;
        public double LineWidth = 0.0;
        public int LineStyle = 0;
        public SkcArrowAttribute StartArrow = new();
        public SkcArrowAttribute EndArrow = new();
        public SkcFaceColor? FaceColor = null;

        internal override string Name => "ARC";
        internal override SkcShape Create() => new SkcArcShape();
        internal override void Read(SkcReader reader)
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
                        { "FLAG", (reader)=> Flag = (ArcFlag)reader.ReadInt()},
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

        internal override void Write(SkcWriter w)
        {
            w.WriteObject("PARAM", false, w =>
            {
                w.Write("P0", P0);
                w.Write("RADIUS", Radius);
                w.Write("FLAT", Flat, 1.0);
                w.Write("ANGLE", Angle, 0.0);
                w.Write("FLAG", (int)Flag, 0);
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
        }

    }
}
