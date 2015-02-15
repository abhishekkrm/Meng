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

namespace QS._qss_x_.Simulations_
{
    [XmlType("Scenario")]
    public sealed class Scenario_
    {
     #region Constructors

        public Scenario_(string _id, QS.Fx.Reflection.Xml.Parameter[] _parameters)
        {
            this._id = _id;
            this._parameters = _parameters;
        }

        public Scenario_()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("id")]
        private string _id;

        [QS.Fx.Printing.Printable("parameters")]
        private QS.Fx.Reflection.Xml.Parameter[] _parameters;

        #endregion

        #region Accessors

        [XmlAttribute("id")]
        public string _Identifier
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlElement("Parameter")]
        public QS.Fx.Reflection.Xml.Parameter[] _Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        #endregion
    }
}
