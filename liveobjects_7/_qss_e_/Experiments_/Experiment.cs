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

﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_e_.Experiments_
{
	public class Experiment : QS.Fx.Inspection.Inspectable, IExperiment
	{
		private static Experiment experiment = new Experiment();
		public static IExperiment None
		{
			get { return experiment; }
		}

		private Experiment()
		{
		}

		#region IExperiment Members

		public virtual void run(QS._qss_e_.Runtime_.IEnvironment environment, QS.Fx.Logging.ILogger logger, QS._core_c_.Components.IAttributeSet arguments, QS._core_c_.Components.IAttributeSet results)
		{
			logger.Log(this, "__Run");
		}

		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
		}

		#endregion

		// ................

		public static QS._core_c_.Components.AttributeSet DefaultArguments(IExperiment experiment)
		{
			return new QS._core_c_.Components.AttributeSet(((experiment.GetType().GetCustomAttributes(
				typeof(QS._qss_e_.Base_1_.ArgumentsAttribute), false)[0]) as QS._qss_e_.Base_1_.ArgumentsAttribute).Arguments);
		}
	}
}
