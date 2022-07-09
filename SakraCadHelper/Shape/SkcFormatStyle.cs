using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcFormatStyle
    {
        public double Round;
        public int Flag;

        public void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "ROUND", (reader)=> Round = reader.ReadDouble()},
                { "FLAG", (reader)=> Flag = reader.ReadInt()},
           });
        }
        public void Write(SkcWriter w)
        {
                w.Write("ROUND", Round);
                w.Write("FLAG", Flag);
        }
    }
}
