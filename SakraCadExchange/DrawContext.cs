using SakraCadHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadExchange
{
    class DrawContext
    {
        public double PaperWidth { get; }
        public double PaperHeight { get; }
        public Pen Pen = new(Color.Black, 0.0f);
        public float TranslateScale = 2.0f;
        public SkcSheet Sheet;
        public SkcLayer Layer;
        public DrawContext(double paperWidth, double paperHeight)
        {
            PaperWidth = paperWidth;
            PaperHeight = paperHeight;
        }
        /// <summary>
        /// DocumentとGDI+の半径などの変換。
        /// </summary>
        public float DocToCanvas(double radius)
        {
            return (float)(radius);
        }
        /// <summary>
        /// DocumentとGDI+の座標変換。Jwwは上が正なのでｙ座標のみ符号を変える。
        /// </summary>
        public PointF DocToCanvas(double x, double y)
        {
            return new PointF((float)(x), (float)(-y + PaperHeight));
        }

        /// <summary>
        /// DocumentとGDI+の座標変換。Jwwは上が正なのでｙ座標のみ符号を変える。
        /// </summary>
        public PointF DocToCanvas(CadPoint p)
        {
            return DocToCanvas(p.X, p.Y);
        }

        /// <summary>
        /// DocumentとGDI+の角度の変換。Jwwの角度は左回り。GDI+は右回り。
        /// 符号を変えるだけだが座標変換に合わせて間違えないようにあえてこれを使う。
        /// </summary>
        public float DocToCanvasAngle(double angle)
        {
            return -(float)angle;
        }


    }
}
