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

namespace QS._qss_d_.Scheduler_1_
{
    [System.Serializable]
    public class LogCollection : QS._core_c_.Base.Logger
    {
        public LogCollection(string name) : this(name, new LogCollection[] {})
        {
        }

        public LogCollection(string name, LogCollection[] subcollections) : this()
        {
            this.name = name;
            this.subcollections = subcollections;
        }

		public LogCollection() : base(null)
		{
			inspectableSubcollections = new QS._qss_e_.Inspection_.Array("Subcomponents", subcollections);
		}

		private string name;
        private LogCollection[] subcollections;

		[QS.Fx.Base.Inspectable("Subcomponents", QS.Fx.Base.AttributeAccess.ReadOnly)]
		[NonSerialized]
		private QS._qss_e_.Inspection_.Array inspectableSubcollections;

		public override string ToString()
        {
            return name;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public LogCollection[] Subcollections
        {
            get { return subcollections; }
            set { subcollections = value; }
        }

        public LogCollection Clone
        {
            get
            {
                LogCollection[] cloned_subcollections = new LogCollection[subcollections.Length];
                for (uint ind = 0; ind < cloned_subcollections.Length; ind++)
                    cloned_subcollections[ind] = subcollections[ind].Clone;
                LogCollection cloned_collection = new LogCollection(name, cloned_subcollections);

                cloned_collection.Buffering = this.Buffering;
                cloned_collection.Console = this.Console;
                cloned_collection.TimestampingModeOf = this.TimestampingModeOf;
                cloned_collection.PrefixString = this.PrefixString;
                cloned_collection.Messages = this.Messages;

                return cloned_collection;
            }
        }
	}
}
