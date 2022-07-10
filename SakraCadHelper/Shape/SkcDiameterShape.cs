using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcDiameterShape : SkcShape
    {
        public SkcPoint P0 = new();
        public double Radius;
        public double Angle = 0.0;
        public double TextPos = 0.5;
        public int Flag = 0;
        public string Text = "";

        public int LineColor = 0;
        public double LineWidth = 0.0;
        public int LineStyle = 0;
        public int TextColor = 0;
        public int TextStyle = 0;
        public SkcFaceColor? FaceColor = null;
        public SkcDiameterStyle DiameterStyle = new();
        public SkcFormatStyle FormatStyle = new();

        internal override string Name => "DIAMETER";
        internal override SkcShape Create() => new SkcDiameterShape();
        internal override void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "PARAM", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "P0", (reader)=> P0 = reader.ReadPoint()},
                        { "RADIUS", (reader)=> Radius = reader.ReadDouble()},
                        { "ANGLE", (reader)=> Angle = reader.ReadDouble()},
                        { "TEXTPOS", (reader)=> TextPos = reader.ReadDouble()},
                        { "FLAG", (reader)=> Flag = reader.ReadInt()},
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
                        { "DIAMETERSTYLE", (reader)=> DiameterStyle.Read(reader)},
                        { "FORMATSTYLE", (reader)=> FormatStyle.Read(reader)},
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
                w.Write("P0", P0);
                w.Write("RADIUS", Radius);
                w.Write("ANGLE", Angle, 0.0);
                w.Write("TEXTPOS", TextPos, 0.5);
                w.Write("FLAG", Flag, 0);
            });
            w.WriteObject("ATTR", false, w =>
            {
                WriteLineColor(w, "LC", LineColor);
                WriteLineWidth(w, "LW", LineWidth);
                WriteLineStyle(w, "LS", LineStyle);
                WriteFaceColor(w, "FC", FaceColor);
                w.Write("TC", TextColor);
                w.Write("TS", TextStyle, 0);
                w.WriteObject("DIAMETERSTYLE", false, w => DiameterStyle.Write(w));
                w.WriteObject("FORMATSTYLE", false, w => FormatStyle.Write(w));
            });
            w.NewLine();
            w.WriteObject("TEXT", false, w => w.WriteString("SRC", Text));
        }
    }

    public class SkcDiameterStyle
    {
        public double LineJut = 10.0;
        public double TextGap = 0.0;
        public string FontName = "";
        public double FontHeight = 3.0;
        public SkcArrowAttribute StartArrow = new();
        public SkcArrowAttribute EndArrow = new();

        internal void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "LINEJUT", (reader)=> LineJut = reader.ReadDouble()},
                { "TEXTGAP", (reader)=> TextGap = reader.ReadDouble()},
                { "FONTNAME", (reader)=> FontName = reader.ReadString()},
                { "FONTHEIGHT", (reader)=> FontHeight = reader.ReadDouble()},
                { "SA",  (reader)=> StartArrow.Read(reader)},
                { "EA",  (reader)=> EndArrow.Read(reader)},
           });
        }

        internal void Write(SkcWriter w)
        {
            w.Write("LINEJUT", LineJut);
            w.Write("TEXTGAP", TextGap);
            w.WriteString("FONTNAME", FontName);
            w.Write("FONTHEIGHT", FontHeight);
            SkcShape.WriteArrow(w, "SA", StartArrow);
            SkcShape.WriteArrow(w, "EA", EndArrow);
        }
    }
}
