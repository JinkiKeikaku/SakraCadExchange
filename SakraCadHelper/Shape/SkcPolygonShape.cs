using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcPolygonShape : SkcShape
    {
        public List<SkcPoint> Vertex = new();
        public bool Loop = false;

        public int LineColor = 0;
        public double LineWidth = 0.0;
        public int LineStyle = 0;
        public SkcArrowAttribute StartArrow = new();
        public SkcArrowAttribute EndArrow = new();
        public SkcFaceColor? FaceColor = null;

        public override string Name => "POLYGON";
        public override SkcShape Create() => new SkcPolygonShape();

        public override void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "PARAM", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "VERTEX", (reader)=> ReadVertex(reader, Vertex)},
                        { "LOOP", (reader)=> Loop = reader.ReadInt() != 0},
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
                WriteVertex(w, "VERTEX", Vertex);
                w.Write("LOOP", Loop ? 1 : 0);
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
