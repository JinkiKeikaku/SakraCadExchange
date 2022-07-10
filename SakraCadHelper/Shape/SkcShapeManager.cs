using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    internal static class SkcShapeManager
    {
        public static SkcShape? Create(string name)
        {
            return Shapes.FirstOrDefault(x => x?.Name == name, null)?.Create();
        }

        public static void WriteShape(SkcWriter w, SkcShape s)
        {
            w.WriteObject(s.Name, false, w =>
            {
                s.Write(w);
            });
            //w.Write($"{s.Name}(");
            //s.Write(w);
            //w.WriteLine($")");
        }

        public static void ReadShapes(SkcReader reader, List<SkcShape> shapes)
        {
            reader.ReadTags(new(), (reader, tag) =>
            {
                var s = SkcShapeManager.Create(tag);
                if (s == null)
                {
                    reader.SkipTag();
                }
                else
                {
                    s.Read(reader);
                    shapes.Add(s);
                }
            });
        }

        public static void WriteShapes(SkcWriter w, string tag, IEnumerable<SkcShape> shapes)
        {
            w.WriteObjects(tag, shapes, (w, s) =>
            {
                WriteShape(w, s);
            }, true);
        }

        static SkcShape[] Shapes = new SkcShape[]
        {
            new SkcLineShape(),
            new SkcCircleShape(),
            new SkcArcShape(),
            new SkcPolygonShape(),
            new SkcSplineShape(),
            new SkcTextShape(),
            new SkcMultiTextShape(),
            new SkcSizeShape(),
            new SkcRadiusShape(),
            new SkcDiameterShape(),
            new SkcAngleShape(),
            new SkcLabelShape(),
            new SkcBalloonShape(),
            new SkcMarkShape(),
            new SkcGroupShape(),
            new SkcBitmapShape(),
            new SkcOleShape(),
        };
    }
}
