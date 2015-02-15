/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following
   disclaimer in the documentation and/or other materials provided
   with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S)
AND ALL OTHER CONTRIBUTORS AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDER(S) OR ANY OTHER
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
SUCH DAMAGE.

*/

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._core_e_.MyMath
{
	public struct BaseStatistics
	{
		private double sum, sum2;
		private int nsamples;

//		public void Reset()
//		{
//			sum = sum2 = 0;
//		}

		public void AddSample(double x)
		{
			sum += x;
			sum2 += x * x;
			nsamples++;
		}

		public double Average // Xbar
		{
			get { return (nsamples > 0) ? (sum / ((double)nsamples)) : double.NaN; }
		}

		public double Variance // S2
		{
			get { return (nsamples > 1) ?  (sum2 - sum * sum / ((double)nsamples)) / (((double)nsamples) - 1) : double.NaN; }
		}

		public double CI95Size // Radius of the 95% confidence interval
		{
			get { return 1.96 * System.Math.Sqrt(Variance); }
		}

		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			s.AppendLine("Average = " + Average.ToString());
			s.AppendLine("Variance = " + Variance.ToString());
			s.AppendLine("CI95Size = " + CI95Size.ToString());
			return s.ToString();
		}
	}
}
