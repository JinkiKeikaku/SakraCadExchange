using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public static class SkcLineStyle
    {
		 readonly static float[] Continuous = {  };
		 readonly static float[] Dashed1 = { 6f, 1.5f };
		 readonly static float[] Dashed2 = { 6f, 6f };
		 readonly static float[] Dashed3 = { 12f, 1.5f, 0.5f, 1.5f };
		 readonly static float[] Dashed4 = { 12f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f };
		 readonly static float[] Dashed5 = { 12f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f };
		 readonly static float[] Dot = { 0.5f, 1.5f };
		 readonly static float[] Chain1 = { 12f, 1.5f, 3.5f, 1.5f };
		 readonly static float[] Chain2 = { 12f, 1.5f, 3.5f, 1.5f, 3.5f, 1.5f };
		 readonly static float[] Dashed6 = { 6f, 1.5f, 0.5f, 1.5f };
		 readonly static float[] Dashed7 = { 6f, 1.5f, 6f, 1.5f, 0.5f, 1.5f };
		 readonly static float[] Dashed8 = { 6f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f };
		 readonly static float[] Dashed9 = { 6f, 1.5f, 6f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f };
		 readonly static float[] Dashed10 = { 6f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f };
		 readonly static float[] Dashed11 = { 6f, 1.5f, 6f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f };

		public static float[][] LineStyleArray { get; } =
		{
			Continuous,	//0
			Dashed1,	//1
			Dashed2,	//2
			Dashed3,	//3
			Dashed4,	//4
			Dashed5,	//5
			Dot,		//6
			Chain1,		//7
			Chain2,		//8
			Dashed6,	//9
			Dashed7,	//10
			Dashed8,	//11
			Dashed9,	//12
			Dashed10,	//13
			Dashed11,	//14
		};
	}
}
