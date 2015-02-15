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

namespace QS._qss_c_.Debugging_
{
	public static class Locking
	{
		public static void Check(object o, QS.Fx.Logging.ILogger logger, Log lockingLog)
		{
			Locking.Check(o, logger, TimeSpan.FromSeconds(1), lockingLog);
		}

		public static void Check(object o, QS.Fx.Logging.ILogger logger)
		{
			Locking.Check(o, logger, TimeSpan.FromSeconds(1));
		}

		public static void Check(object o, QS.Fx.Logging.ILogger logger, TimeSpan timeout)
		{
			Locking.Check(o, logger, timeout, null);
		}

		public static void Check(object o, QS.Fx.Logging.ILogger logger, TimeSpan timeout, Log lockingLog)
		{
			try
			{
				if (!System.Threading.Monitor.TryEnter(o, timeout))
					throw new Exception("Cannot grab a lock.");
				System.Threading.Monitor.Exit(o);
			}
			catch (Exception exc)
			{
				logger.Log(exc.ToString() + "\n" + exc.StackTrace + ((lockingLog != null) ? ("\n" + lockingLog.Log) : ""));
				throw new Exception("Cannot grab a lock.");
			}
		}

		public static void WaitCheck(object o, QS.Fx.Logging.ILogger logger, TimeSpan timeout)
		{
			while (!System.Threading.Monitor.TryEnter(o, timeout))
				logger.Log(null, "Cannot grab a lock on " + QS._core_c_.Helpers.ToString.ObjectRef(o) + ".\n" + 
					System.Threading.CompressedStack.GetCompressedStack().ToString());
			System.Threading.Monitor.Exit(o);
		}

		public enum Operation
		{
			LOCK, UNLOCK
		}

		public class Log : EventLog<Operation>
		{
			public Log() : base()
			{
			}
		}
	}
}
