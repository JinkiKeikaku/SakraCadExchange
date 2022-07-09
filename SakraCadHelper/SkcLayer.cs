using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    public class SkcLayer
    {
        const int LAYER_SHOW = 1;	//		見えるか
        const int LAYER_PRINT = 2;	//		印刷できるか
        const int LAYER_SNAP = 4;   //		SNAPできるか

        public string Name = "";
        public int Flag = LAYER_SHOW | LAYER_PRINT | LAYER_SNAP;

        public override string ToString()
        {
            return $"NAME(\"{Name}\")FLAG({Flag})"; ;
        }
    }
}
