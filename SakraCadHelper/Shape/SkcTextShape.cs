using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcTextShape : SkcShape
    {
        public SkcPoint P0 = new();
        public string Text = "";

        public int LineColor = 0;
        public double LineWidth = 0.0;
        public int LineStyle = 0;
        public SkcFaceColor? FaceColor = null;
        public SkcTextAttribute TextAttribute = new();

        internal override string Name => "TEXT";
        internal override SkcShape Create() => new SkcTextShape();
        internal override void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "PARAM", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "P0", (reader)=> P0 = reader.ReadPoint()},
                    })
                },
                { "ATTR", (reader)=>
                    reader.ReadTags(new Dictionary<string, Action<SkcReader>>()
                    {
                        { "LC", (reader)=> LineColor = reader.ReadInt()},
                        { "LW", (reader)=> LineWidth = reader.ReadDouble()},
                        { "LS", (reader)=> LineStyle = reader.ReadInt()},
                        { "FC", (reader)=> FaceColor = ReadFaceColor(reader)},
                    }, (reader, tag) =>
                    {
                        if(!TextAttribute.Read(reader, tag))
                        {
                            reader.SkipTag();
                        }
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
                w.Write<SkcPoint>("P0", P0);
            });
            w.WriteObject("ATTR", false, w =>
            {
                WriteLineColor(w, "LC", LineColor);
                WriteLineWidth(w, "LW", LineWidth);
                WriteLineStyle(w, "LS", LineStyle);
                WriteFaceColor(w, "FC", FaceColor);
                TextAttribute.Write(w);
            });
            w.NewLine();
            w.WriteObject("TEXT", false, w =>
            {
                w.WriteString("SRC", Text);
            });
        }
    }
}
