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
using System.Xml.Serialization;

namespace QS._core_e_.Repository
{
    [XmlType("attribute")]
    [XmlInclude(typeof(AttributeCollection))]
    [XmlInclude(typeof(ScalarAttribute))]
    [Serializable]
    public abstract class Attribute : IAttribute
    {
        public Attribute(IRepository repository, string key, string name)
        {
            this.name = name;
            this.repository = repository;
            this.key = key;
        }

        public Attribute()
        {
        }

        protected string name;
        protected IRepository repository;
        protected string key;

        #region Accessors

        [XmlIgnore]
        public virtual IRepository Repository
        {
            get { return repository; }
            set { repository = value; }
        }

        [XmlIgnore]
        public virtual string Key
        {
            get { return key; }
            set { key = value; }
        }

        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion

        #region Inspection.IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return name; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { throw new NotSupportedException("This is a generic attribute class."); }
        }

        #endregion

        #region IAttribute Members

        IRepository IAttribute.Repository
        {
            get { return repository; }
        }

        string IAttribute.Key
        {
            get { return key; }
        }

        string IAttribute.Ref
        {
            get { throw new NotSupportedException("Abstract attribute cannot be referenced."); }
        }

        #endregion
    }
}
