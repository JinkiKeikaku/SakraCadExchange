using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcLabelShape : SkcShape
    {
        [Flags]
        public enum LabelFlag
        {
            None = 0,
            AutoLabelPosition=1,
        }

        public List<SkcPoint> Vertex = new();
        public SkcPoint TextPoint = new();
        public LabelFlag Flag = LabelFlag.AutoLabelPosition;
        public string Text = "";

        public int LineColor = 0;
        public double LineWidth = 0.0;
        public int LineStyle = 0;
        public SkcFaceColor? FaceColor = null;

        public int TextColor = 0;
        public int TextStyle = 0;
        public double TextAngle = 0.0;

        public SkcLabelStyle LabelStyle = new();

        internal override string Name => "LABEL";
        internal override SkcShape Create() => new SkcLabelShape();
        internal override void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "PARAM", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "VERTEX", (reader)=> ReadVertex(reader, Vertex)},
                        { "TEXTPOINT", (reader)=> TextPoint = reader.ReadPoint()},
                        { "FLAG", (reader)=> Flag = (LabelFlag)reader.ReadInt()},
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
                        { "LABELSTYLE", (reader)=> LabelStyle.Read(reader)},
                    })
                },
                { "TEXT", (reader)=>
                    reader.ReadTag("SRC", (reader)=>{
                        Text = reader.ReadString();
                    })
                },
            }); ;
        }

        internal override void Write(SkcWriter w)
        {
            w.WriteObject("PARAM", false, w =>
            {
                WriteVertex(w, "VERTEX", Vertex);
                if(Flag == 0)
                {
                    w.Write("TEXTPOINT", TextPoint);
                }
                w.Write("FLAG", (int)Flag, (int)LabelFlag.AutoLabelPosition);
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
                w.WriteObject("LABELSTYLE", false, w => LabelStyle.Write(w));
            });
            w.NewLine();
            w.WriteObject("TEXT", false, w => w.WriteString("SRC", Text));
        }
    }
    public class SkcLabelStyle
    {
        public double TextGap = 0.0;
        public string FontName = "";
        public double FontHeight = 3.0;        ////ﾌｫﾝﾄの高さ(Paper座標 [mm])
        public SkcArrowAttribute Arrow = new();

        internal void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "TEXTGAP", (reader)=> TextGap = reader.ReadDouble()},
                { "FONTNAME", (reader)=> FontName = reader.ReadString()},
                { "FONTHEIGHT", (reader)=> FontHeight = reader.ReadDouble()},
                { "ARROW",  (reader)=> Arrow.Read(reader)},
           });
        }
        internal void Write(SkcWriter w)
        {
            w.Write("TEXTGAP", TextGap);
            w.WriteString("FONTNAME", FontName);
            w.Write("FONTHEIGHT", FontHeight);
            SkcShape.WriteArrow(w, "ARROW", Arrow);
        }

    }


}
