using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    public class SkcPaper
    {
        public string Name = "A3";
        public double Width = 420.0;
        public double Height = 297.0;
        public int Horz = 1;

        internal void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                {"PAPERSIZE", reader =>
                {
                    reader.ReadTags(new()
                    {
                        {"NAME",  reader => Name = reader.ReadString()},
                        {"WIDTH",  reader => Width = reader.ReadDouble()},
                        {"HEIGHT",  reader => Height = reader.ReadDouble()},
                    });
                } },
                {"HORZ", reader =>Horz = reader.ReadInt() },
            });
        }


        internal void Write(SkcWriter writer)
        {
            writer.WriteObject("PAPERSIZE", false, writer =>
            {
                writer.WriteString("NAME", Name);
                writer.Write("WIDTH", Width);
                writer.Write("HEIGHT", Height);
            });
            writer.Write("HORZ", Horz);
        }
        public override string ToString()
        {
            return $"PAPERSIZE(NAME(\"{Name}\")WIDTH({Width})HEIGHT({Height}))HORZ({Horz})";
        }
    }
}
