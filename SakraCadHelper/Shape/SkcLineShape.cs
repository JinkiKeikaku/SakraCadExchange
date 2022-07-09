using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcLineShape : SkcShape
    {
        public SkcPoint P0 = new();
        public SkcPoint P1 = new();

        public int LineColor = 0;
        public double LineWidth = 0.0;
        public int LineStyle = 0;
        public SkcArrowAttribute StartArrow = new ();
        public SkcArrowAttribute EndArrow = new ();

        public override string Name => "LINE";
        public override SkcShape Create() => new SkcLineShape();

        public override void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "PARAM", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "P0", (reader)=> P0 = reader.ReadPoint()},
                        { "P1", (reader)=> P1 = reader.ReadPoint()},
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
                    })
                },
            });
        }

        public override void Write(SkcWriter w)
        {
            w.WriteObject("PARAM", false, w =>
            {
                w.Write<SkcPoint>("P0", P0);
                w.Write<SkcPoint>("P1", P1);
            });
            w.WriteObject("ATTR", false, w =>
            {
                WriteLineColor(w, "LC", LineColor);
                WriteLineWidth(w, "LW", LineWidth);
                WriteLineStyle(w, "LS", LineStyle);
                WriteArrow(w, "SA", StartArrow);
                WriteArrow(w, "EA", EndArrow);
            });
        }
    }
}
