using SakraCadHelper;
using SakraCadHelper.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadExchange
{
    class SakraCadDrawer
    {
        SkcDocument mDoc;
        double mScale = 1.0;
        public SakraCadDrawer(SkcDocument doc)
        {
            mDoc = doc;
        }
        public void OnDraw(Graphics g, DrawContext d)
        {
            var page = mDoc.Pages[0];
            foreach (var sheet in page.Sheets)
            {
                d.Sheet = sheet;
                mScale = sheet.PaperScale.GetScale();
                foreach (var layer in page.Layers)
                {
                    if (!sheet.LayerShapes.ContainsKey(layer.Name)) continue;
                    var shapes = sheet.LayerShapes[layer.Name];
                    foreach (var shape in shapes)
                    {
                        OnDrawShape(g, d, shape);
                    }
                }
            }
        }

        void OnDrawShape(Graphics g, DrawContext d, SkcShape shape)
        {
            switch (shape)
            {
                case SkcLineShape s: OnDrawLine(g, d, s); break;
                case SkcCircleShape s: OnDrawCircle(g, d, s);break;
            }
        }
        void OnDrawLine(Graphics g, DrawContext d, SkcLineShape shape)
        {
            ApplyLineStyle(d.Pen, shape.LineColor, shape.LineWidth, shape.LineStyle);
            var p0 = d.DocToCanvas(ConvertPoint(shape.P0));
            var p1 = d.DocToCanvas(ConvertPoint(shape.P1));
            g.DrawLine(d.Pen, p0, p1);
        }

        void OnDrawCircle(Graphics g, DrawContext d, SkcCircleShape shape)
        {
            ApplyLineStyle(d.Pen, shape.LineColor, shape.LineWidth, shape.LineStyle);
            var p0 = d.DocToCanvas(ConvertPoint(shape.P0));
            var rx = d.DocToCanvas(ConvertLength(shape.Radius));
            var ry = (float)(rx * shape.Flat);
            var angle = d.DocToCanvasAngle(shape.Angle);
            var saved = g.Save();
            g.TranslateTransform(p0.X, p0.Y);
            g.RotateTransform(angle);
            g.DrawEllipse(d.Pen, -rx, -ry, rx * 2, ry * 2);
            g.Restore(saved);
        }

        CadPoint ConvertPoint(SkcPoint p)
        {
            return new CadPoint(mScale * p.X, mScale * p.Y);
        }
        double ConvertLength(double a)
        {
            return mScale * a;
        }

        void ApplyLineStyle(Pen pen, int lineColor, double lineWidth, int lineType)
        {
            var c = ConvertColor(lineColor);
            pen.Color = c;
            if (lineType == 0)
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            }
            else
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                pen.DashPattern = SkcLineStyle.LineStyleArray[lineType];
            }
            pen.Width = (float)lineWidth;
        }

        Color ConvertColor(int c)
        {
            if (c == SkcColor.NullColor)
            {
                return Color.Transparent;
            }
            return Color.FromArgb(c & 255, (c >> 8) & 255, (c >> 16) & 255);
        }
    }
}