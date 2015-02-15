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

namespace QS._qss_e_.Inspection_
{
	public class LoggerCollection : QS.Fx.Inspection.IAttributeCollection
	{
		public LoggerCollection()
		{
		}

		private System.Collections.Generic.Dictionary<string, LoggerAttribute> loggers = new Dictionary<string, LoggerAttribute>();

		public void add(string name, QS._core_c_.Base.IOutputReader logger)
		{
			loggers.Add(name, new LoggerAttribute(name, logger));
		}

		#region Class LoggerAttribute

		private class LoggerAttribute : QS.Fx.Inspection.IScalarAttribute
		{
			public LoggerAttribute(string name, QS._core_c_.Base.IOutputReader logger)
			{
				this.name = name;
				this.logger = logger;
			}

			private string name;
			private QS._core_c_.Base.IOutputReader logger;

			#region IScalarAttribute Members

			object QS.Fx.Inspection.IScalarAttribute.Value
			{
				get { return logger.CurrentContents; }
			}

			#endregion

			#region IAttribute Members

            string QS.Fx.Inspection.IAttribute.Name
			{
				get { return name; }
			}

            QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
			{
				get { return QS.Fx.Inspection.AttributeClass.SCALAR; }
			}

			#endregion
		}

		#endregion

		#region IAttributeCollection Members

		IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
		{
			get 
			{
//				Dictionary<string, QS.CMS.Base.IOutputReader>.KeyCollection keys = loggers.Keys;
//				string[] result = new string[keys.Count];
//				keys.CopyTo(result, 0);
//				return result;

				return loggers.Keys;
			}
		}

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
		{
			get { return loggers[attributeName]; }
		}

		#endregion

		#region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
		{
			get { return "Log Collection"; }
		}

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
		}

		#endregion
	}
}
