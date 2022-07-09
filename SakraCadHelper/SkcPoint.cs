using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    public class SkcPoint
    {
        /// <summary>
        /// X座標
        /// </summary>
        public double X;
        /// <summary>
        /// Y座標
        /// </summary>
        public double Y;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SkcPoint() { }
        public SkcPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            return obj is SkcPoint point &&
                   X == point.X &&
                   Y == point.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        /// <inheritdoc/>
        public override string ToString() => $"{X},{Y}";
    }
}
