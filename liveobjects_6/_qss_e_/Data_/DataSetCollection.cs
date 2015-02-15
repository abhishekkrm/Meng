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

namespace QS._qss_e_.Data_
{
    public abstract class DataSetCollection<C> : QS._core_e_.Data.IDataSet, QS.Fx.Inspection.IAttributeCollection where C : QS._core_e_.Data.IDataSet
    {
        public DataSetCollection(string name)
        {
            this.name = name;
        }

        protected string name;
        protected IDictionary<string, C> dataSetCollection = new Dictionary<string, C>();

        protected abstract void draw(System.Drawing.Graphics graphics);

        public void Add(string name, C dataSet)
        {
            dataSetCollection.Add(name, dataSet);
        }

        #region IDataSet Members

        QS._core_e_.Data.IDataSet QS._core_e_.Data.IDataSet.downsample(System.Drawing.Size targetResolution)
        {
            return this;
        }

        void QS._core_e_.Data.IDataSet.draw(System.Drawing.Graphics graphics)
        {
            draw(graphics);
        }

        QS._core_e_.Data.Rectangle QS._core_e_.Data.IDataSet.Range
        {
            get { return new QS._core_e_.Data.Rectangle(); }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get { return dataSetCollection.Keys; }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get { return new QS.Fx.Inspection.ScalarAttribute(attributeName, dataSetCollection[attributeName]); }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return name; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion
    }
}
