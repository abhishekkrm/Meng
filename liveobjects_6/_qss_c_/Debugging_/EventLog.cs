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
	public class EventLog<C>
	{
		public EventLog() : this(null)
		{
		}

		public EventLog(QS.Fx.Clock.IClock clock)
		{
			this.clock = clock;
		}

		private QS.Fx.Clock.IClock clock;
		private System.Collections.Generic.List<Event> events = new List<Event>();

		public void Add(C data, string comment)
		{
			lock (events)
			{
				events.Add(new Event(data, ((clock != null) ? clock.Time : -1), comment));
			}
		}

		public string Log
		{
			get
			{
				lock (events)
				{
					StringBuilder s = new StringBuilder();
					foreach (Event e in events)
						s.AppendLine(e.ToString());
					return s.ToString();
				}
			}
		}

		#region Class Event

		public class Event
		{
			public Event(C data, double time, string comment)
			{
				this.data = data;
				this.time = time;
				this.comment = comment;
			}

			private C data;
			private double time;
			private string comment;

			public C Data
			{
				get { return data; }
			}

			public double Time
			{
				get { return time; }
			}

			public string Comment
			{
				get { return comment; }
			}

			public override string ToString()
			{
				StringBuilder s = new StringBuilder();
				if (time >= 0)
				{
					s.Append("[");
					s.Append(time.ToString());
					s.Append("] ");
				}
				s.Append(data.ToString());
				s.Append(" \"");
				s.Append(comment);
				s.Append("\"");
				return s.ToString();
			}
		}

		#endregion
	}
}
