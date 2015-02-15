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
	public static class Threads
	{
		public static string DumpStates
		{
			get
			{
				System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
				StringBuilder s = new StringBuilder("Process " + process.ProcessName + "\n");
				foreach (System.Diagnostics.ProcessThread thread in process.Threads)
				{
					System.Diagnostics.ThreadState state = thread.ThreadState;
					string wait_reason = string.Empty;
					if (state == System.Diagnostics.ThreadState.Wait)
					{
						try
						{
							wait_reason = thread.WaitReason.ToString();
						}
						catch (Exception exc)
						{
							wait_reason = exc.ToString();
						}
					}
					s.AppendLine("Thread " + thread.Id.ToString() + ", " + state.ToString() + 
						((state == System.Diagnostics.ThreadState.Wait) ? (", " + wait_reason) : string.Empty));
				}
				return s.ToString();
			}
		}
	}
}
