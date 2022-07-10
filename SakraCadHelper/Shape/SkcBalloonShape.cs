using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcBalloonShape : SkcShape
    {
        public List<SkcPoint> Vertex = new();
        public double MinRadius = 0.0;
        public double MaxRadius = 0.0;
        public string Text = "";

        public int LineColor = 0;
        public double LineWidth = 0.0;
        public int LineStyle = 0;
        public SkcFaceColor? FaceColor = null;
        public int TextColor = 0;
        public int TextStyle = 0;
        public double TextAngle = 0.0;
        public SkcBalloonStyle BalloonStyle = new();

        public override string Name => "BALLOON";
        public override SkcShape Create() => new SkcBalloonShape();

        public override void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "PARAM", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "VERTEX", (reader)=> ReadVertex(reader, Vertex)},
                        { "MINRADIUS", (reader)=> MinRadius = reader.ReadDouble()},
                        { "MAXRADIUS", (reader)=> MaxRadius = reader.ReadDouble()},
                    })
                },
                { "ATTR", (reader)=>
                    reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
                    {
                        { "LC", (reader)=> LineColor = reader.ReadInt()},
                        { "LW", (reader)=> LineWidth = reader.ReadDouble()},
                        { "LS", (reader)=> LineStyle = reader.ReadInt()},
                        { "FC", (reader)=> FaceColor = ReadFaceColor(reader)},
                        { "TC", (reader)=> TextColor = reader.ReadInt()},
                        { "TS", (reader)=> TextStyle = reader.ReadInt()},
                        { "TA", (reader)=> TextAngle = reader.ReadDouble()},
                        { "BALLOONSTYLE", (reader)=> BalloonStyle.Read(reader)},
                    })
                },
                { "TEXT", (reader)=>
                    reader.ReadTag("SRC", (reader)=>{
                        Text = reader.ReadString();
                    })
                },
            }); ;
        }

        public override void Write(SkcWriter w)
        {
            w.WriteObject("PARAM", false, w =>
            {
                WriteVertex(w, "VERTEX", Vertex);
                w.Write("MINRADIUS", MinRadius, 0.0);
                w.Write("MAXRADIUS", MaxRadius, 0.0);
            });
            w.WriteObject("ATTR", false, w =>
            {
                WriteLineColor(w, "LC", LineColor);
                WriteLineWidth(w, "LW", LineWidth);
                WriteLineStyle(w, "LS", LineStyle);
                WriteFaceColor(w, "FC", FaceColor);
                w.Write("TC", TextColor);
                w.Write("TS", TextStyle, 0);
                w.Write("TA", TextAngle, 0.0);
                w.WriteObject("BALLOONSTYLE", false, w => BalloonStyle.Write(w));
            });
            w.NewLine();
            w.WriteObject("TEXT", false, w => w.WriteString("SRC", Text));
        }
    }

    public class SkcBalloonStyle
    {
        public string FontName = "";
        public double FontHeight = 3.0;        ////ﾌｫﾝﾄの高さ(Paper座標 [mm])
        public SkcArrowAttribute Arrow = new();

        public void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "FONTNAME", (reader)=> FontName = reader.ReadString()},
                { "FONTHEIGHT", (reader)=> FontHeight = reader.ReadDouble()},
                { "ARROW",  (reader)=> Arrow.Read(reader)},
           });
        }
        public void Write(SkcWriter w)
        {
            w.WriteString("FONTNAME", FontName);
            w.Write("FONTHEIGHT", FontHeight);
            SkcShape.WriteArrow(w, "ARROW", Arrow);
        }

    }

}
