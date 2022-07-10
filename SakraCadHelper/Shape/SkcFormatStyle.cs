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
        public string Header = "";
        public string Footer = "";

        internal void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "ROUND", (reader)=> Round = reader.ReadDouble()},
                { "FLAG", (reader)=> Flag = reader.ReadInt()},
                { "HEADER", (reader)=> Header = reader.ReadString()},
                { "FOOTER", (reader)=> Footer = reader.ReadString()},
           });
        }
        internal void Write(SkcWriter w)
        {
            w.Write("ROUND", Round);
            w.Write("FLAG", Flag);
            if (Header != "") w.WriteString("HEADER", Header);
            if (Footer != "") w.WriteString("FOOTER", Footer);
        }
    }
}
