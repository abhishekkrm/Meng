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

namespace QS.Fx.Attributes
{
    public static class AttributeClasses
    {
        #region Constants 

        public const string CLASSID_id = "9F64B7ABB95245F28596BFFD1F549558";
        public const string CLASSID_name = "9F4C608A9A6D44FFAD8A2FDC662E185B";
        public const string CLASSID_comment = "07EC27D89CBD410DADE60463E4D46985";

        #endregion

        #region Constructor

        static AttributeClasses()
        {
            _classes = new Dictionary<QS.Fx.Base.ID, IAttributeClass>();

            _class_id =
                new AttributeClass(
                    new QS.Fx.Base.ID(CLASSID_id),
                    "id",
                    "A globally unique identifier.");
            _classes.Add(_class_id.ID, _class_id);

            _class_name =
                new AttributeClass(
                    new QS.Fx.Base.ID(CLASSID_name),
                    "name",
                    "A human-readable name that might not be globally unique.");
            _classes.Add(_class_name.ID, _class_name);

            _class_comment =
                new AttributeClass(
                    new QS.Fx.Base.ID(CLASSID_comment),
                    "comment",
                    "A human-readable comment.");
            _classes.Add(_class_comment.ID, _class_comment);
        }

        #endregion

        #region Fields

        private static readonly IDictionary<QS.Fx.Base.ID, IAttributeClass> _classes;
        private static readonly IAttributeClass _class_id, _class_name, _class_comment;

        #endregion

        #region Accessors

        public static IAttributeClass CLASS_id
        {
            get { return _class_id; }
        }

        public static IAttributeClass CLASS_name
        {
            get { return _class_name; }
        }

        public static IAttributeClass CLASS_comment
        {
            get { return _class_comment; }
        }

        #endregion

        #region GetAttributeClass

        public static bool GetAttributeClass(QS.Fx.Base.ID _id, out IAttributeClass _class)
        {
            return _classes.TryGetValue(_id, out _class);
        }

        public static IAttributeClass GetAttributeClass(QS.Fx.Base.ID _id)
        {
            IAttributeClass _class;
            if (!_classes.TryGetValue(_id, out _class))
                throw new Exception("Cannot find attribute class \"" + _id.ToString() + "\".");
            return _class;
        }

        #endregion
    }
}
