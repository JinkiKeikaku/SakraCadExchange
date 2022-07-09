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
        public bool Horz = true;

        public override string ToString()
        {
            return $"PAPERSIZE(NAME(\"{Name}\")WIDTH({Width})HEIGHT({Height}))HORZ({(Horz ? 1 : 0)})";
        }
    }
}
