using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    public class SkcPaperScale
    {
        public string Name = "";
        /// <summary>
        /// 負の場合、縮尺は-1/Numeratorとなる。正の場合、縮尺はNumeratorです。
        /// </summary>
        public double Numerator = 1.0;
        /// <summary>
        /// 縮尺を返します。
        /// </summary>
        public double GetScale() => Numerator < 0 ? 1 / (-Numerator) : Numerator;
        /// <summary>
        /// 縮尺を設定します。このメソッドを使うと縮尺からNumeratorを設定することができます。
        /// </summary>
        public void SetScale(double x)
        {
            var nm = (int)(1.0 / x + 0.5);
            Numerator = (x -1.0 / nm == 0) ? -nm : x;
        }

        public override string ToString()
        {
            return $"NAME(\"{Name}\")NUMERATOR({Numerator})";
        }
    }
}
