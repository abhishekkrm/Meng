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

namespace QS.Fx.Reflection
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
    public sealed class ParameterAttribute : System.Attribute
    {
        public ParameterAttribute(
            string _id,
            QS.Fx.Reflection.ParameterClass _parameterclass)
            : this(_id, null, _parameterclass, null)
        {
        }

        public ParameterAttribute(
            string _id, 
            QS.Fx.Reflection.ParameterClass _parameterclass,
            Type _constraint)
            : this(_id, null, _parameterclass, _constraint)
        {
        }

        public ParameterAttribute(
            string _id, 
            string _comment, 
            QS.Fx.Reflection.ParameterClass _parameterclass)
            : this(_id, _comment, _parameterclass, null)
        {
        }

        //public ParameterAttribute(
        //    string _id, 
        //    string _comment, 
        //    QS.Fx.Reflection.ParameterClass _parameterclass, 
        //    string _defaultvalue)
        //{
        //    this._id = _id;
        //    this._comment = _comment;
        //    this._parameterclass = _parameterclass;
        //    this._defaultvalue = _defaultvalue;
        //}

        public ParameterAttribute(
            string _id, 
            string _comment, 
            QS.Fx.Reflection.ParameterClass _parameterclass, 
            Type _constraint)
        {
            this._id = _id;
            this._comment = _comment;
            this._parameterclass = _parameterclass;
            this._constraint = _constraint;
        }

        [QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASSID_id)]
        private string _id;
        [QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASSID_comment)]
        private string _comment;
        private object _defaultvalue;
        private QS.Fx.Reflection.ParameterClass _parameterclass;
        private object _constraint;

        public string ID
        {
            get { return _id; }
            //set { _id = value; }
        }

        public string Comment
        {
            get { return _comment; }
            //set { _comment = value; }
        }

        public QS.Fx.Reflection.ParameterClass ParameterClass
        {
            get { return _parameterclass; }
            //set { _parameterclass = value; }
        }

        public object DefaultValue
        {
            get { return _defaultvalue; }
            //set { _defaultvalue = value; }
        }

        public object Constraint
        {
            get { return _constraint; }
            //set { _constraint = value; }
        }
    }
}
