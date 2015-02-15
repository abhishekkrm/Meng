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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Inspection_.Hierarchy_
{
    public sealed class HierarchyView : IHierarchyView
    {
        #region Constructor

        public HierarchyView(string name)
        {
            this.name = name;
        }

        #endregion

        #region Fields

        private string name;
        private IDictionary<object, HierarchyViewObject> objects = new Dictionary<object, HierarchyViewObject>();

        #endregion

        #region IHierarchyView Members

        string IHierarchyView.Name
        {
            get { return name; }
        }

        IEnumerable<IHierarchyViewObject> IHierarchyView.Objects
        {
            get 
            {
                lock (this)
                {
                    IHierarchyViewObject[] result = new IHierarchyViewObject[objects.Count];
                    int index = 0;
                    foreach (HierarchyViewObject obj in objects.Values)
                        result[index++] = obj;
                    return result;
                }
            }
        }

        #endregion

        #region Accessors

        public void Add(object id, string name, string description)
        {
            lock (this)
            {
                objects.Add(id, new HierarchyViewObject(id, name, description));
            }
        }

        public void Link(object id1, object id2, string name, string description)
        {
            lock (this)
            {
                HierarchyViewObject obj1 = objects[id1], obj2 = objects[id2];
                HierarchyViewConnection conn = new HierarchyViewConnection(name, description, obj1, obj2);
                obj1.Add(true, conn);
                obj2.Add(false, conn);
            }
        }

        #endregion
    }
}
