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

namespace QS._qss_c_.Components_1_
{
    public interface ISetChange<T>
    {
        System.Collections.Generic.ICollection<T> ToAdd
        {
            get;
        }

        System.Collections.Generic.ICollection<T> ToRemove
        {
            get;
        }
    }

    public interface ISetChangeWithUpdates<T> : ISetChange<T>
    {
        System.Collections.Generic.ICollection<Base3_.Pair<T>> ToUpdate
        {
            get;
        }
    }

    public class SetChange<T> : ISetChange<T>
    {
        public SetChange()
        {
            toAdd = new System.Collections.ObjectModel.Collection<T>();
			toRemove = new System.Collections.ObjectModel.Collection<T>();               
        }

        private System.Collections.ObjectModel.Collection<T> toAdd, toRemove;
    
        #region ISetChange<T> Members

        public ICollection<T> ToAdd
        {
	        get { return toAdd; }
        }

        public ICollection<T> ToRemove
        {
	        get { return toRemove; }
        }

        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (T element in toAdd)
                s.Append(" add(" + element.ToString() + ")");
            foreach (T element in toRemove)
                s.Append(" rem(" + element.ToString() + ")");
            return s.ToString();
        }
    }

    public class SetChangeWithUpdates<T> : SetChange<T>, ISetChangeWithUpdates<T>
    {
        public SetChangeWithUpdates() : base()
        {
			toUpdate = new System.Collections.ObjectModel.Collection<Base3_.Pair<T>>();
        }

        private System.Collections.ObjectModel.Collection<Base3_.Pair<T>> toUpdate;

        #region ISetChangeWithUpdates<T> Members

        public ICollection<Base3_.Pair<T>> ToUpdate
        {
            get { return toUpdate; }
        }

        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder(base.ToString());
            foreach (Base3_.Pair<T> update in toUpdate)
                s.Append(" upd(" + update.Element1.ToString() + " -> " + update.Element2.ToString() + ")");
            return s.ToString();
        }
    }
}
