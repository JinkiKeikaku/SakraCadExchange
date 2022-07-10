using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcMarkShape : SkcShape
    {
        public SkcPoint P0 = new();
        public double Angle;


        public int LineColor = 0;
        public double LineWidth = 0.0;
        public int LineStyle = 0;
        public SkcMarkStyle MarkStyle = new();

        internal override string Name => "MARK";
        internal override SkcShape Create() => new SkcMarkShape();
        internal override void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "PARAM", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "P0", (reader)=> reader.ReadPoint()},
                        { "ANGLE", (reader)=> Angle = reader.ReadDouble()},
                    })
                },
                { "ATTR", (reader)=>
                    reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
                    {
                        { "LC", (reader)=> LineColor = reader.ReadInt()},
                        { "LW", (reader)=> LineWidth = reader.ReadDouble()},
                        { "LS", (reader)=> LineStyle = reader.ReadInt()},
                        { "MARKSTYLE", (reader)=> MarkStyle.Read(reader)},
                    })
                },
            }); ;
        }

        internal override void Write(SkcWriter w)
        {
            w.WriteObject("PARAM", false, w =>
            {
                w.Write("P0", P0);
                w.Write("ANGLE", Angle, 0.0);
            });
            w.WriteObject("ATTR", false, w =>
            {
                WriteLineColor(w, "LC", LineColor);
                WriteLineWidth(w, "LW", LineWidth);
                WriteLineStyle(w, "LS", LineStyle);
                w.WriteObject("MARKSTYLE", false, w => MarkStyle.Write(w));
            });
        }
    }

    public class SkcMarkStyle
    {
        const int MARKERCODE_DOT = 0;
        const int CC_MARKERCODE_X = 1;
        const int CC_MARKERCODE_PLUS = 2;
        const int MARKERCODE_CIRCLE = 3;
        const int MARKERCODE_RECT = 4;
        const int MARKERCODE_TRIANGLE = 5;
        const int MARKERCODE_ASTERISK = 6;


        public int Code = 0;
        public double Radius = 1.0;

        internal void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "CODE", (reader)=> Code = reader.ReadInt()},
                { "RADIUS", (reader)=> Radius = reader.ReadDouble()},
           });
        }
        internal void Write(SkcWriter w)
        {
            w.Write("CODE", Code, 0);
            w.Write("RADIUS", Radius, 1.0);
        }
    }
}
