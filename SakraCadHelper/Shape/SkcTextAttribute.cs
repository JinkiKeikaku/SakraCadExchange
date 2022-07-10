using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcTextAttribute
    {
        public int Color;
        public double Angle;
        public string FontName = "";
        public double FontHeight = 3.0;        ////ﾌｫﾝﾄの高さ(Paper座標 [mm])
        public double FontWidthScale;    ////ﾌｫﾝﾄの幅の比(デフォルトの幅に対する比：0の場合、デフォルト=1.0と同じ)
        public int Flag;

        internal bool Read(SkcReader reader, string tag)
        {
            var f = true;
            switch (tag)
            {
                case "TC":
                    Color = reader.ReadInt();
                    break;
                case "TA":
                    Angle = reader.ReadDouble();
                    break;
                case "TS":
                    Flag = reader.ReadInt();
                    break;
                case "FONTNAME":
                    FontName = reader.ReadString();
                    break;
                case "FONTHEIGHT":
                    FontHeight = reader.ReadDouble();
                    break;
                case "FONTWIDTHSCALE":
                    FontWidthScale = reader.ReadDouble();
                    break;
                default:
                    f = false;
                    break;
            }
            return f;
        }

        internal void Write(SkcWriter w)
        {
            w.WriteObject("PARAM", false, w =>
            {
                w.Write("TC", Color);
                w.Write("TA", Angle, 0.0);
                w.Write("TS", Flag, 0);
                w.WriteString("FONTNAME", FontName);
                w.Write("FONTHEIGHT", FontHeight);
                if (FontWidthScale != 0.0 && FontWidthScale != 1.0)
                {
                    w.Write("FONTWIDTHSCALE", FontWidthScale, 0.0);
                }
            });
        }
    }
}
