using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcGroupShape : SkcShape
    {
        public SkcPoint P0 = new();
        public List<SkcShape> Shapes = new();
        public override string Name => "GROUP";
        public override SkcShape Create() => new SkcGroupShape();
        public override void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "PARAM", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "P0", (reader)=> reader.ReadPoint()},
                    }) 
                },
                { "SHAPES", (reader)=>SkcShapeManager.ReadShapes(reader, Shapes) },
            });
        }
        public override void Write(SkcWriter w)
        {
            w.WriteObject("PARAM", false, w =>
            {
                w.Write("P0", P0);
            });
            SkcShapeManager.WriteShapes(w, "SHAPES", Shapes);
        }
    }
}
