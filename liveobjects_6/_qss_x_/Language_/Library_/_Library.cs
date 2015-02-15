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

namespace QS._qss_x_.Language_.Library_
{
/*
    public sealed class Library : QS.Fx.Language.Base.ILibrary
    {
        #region StandardLibrary

        public static QS.Fx.Language.Base.ILibrary StandardLibrary
        {
            get { return standard_library; }
        }

        private static readonly Library standard_library =
            new Library(
                new QS.Fx.Language.Base.ILibrary[] { Bool.Type, UInt.Type, UIntSet.Type, Versioned.Type });

        #endregion

        #region Constructors

        public Library(IEnumerable<QS.Fx.Language.Base.IType> types)
        {
            foreach (QS.Fx.Language.Base.IType type in types)
                this.types.Add(type.Name, type);
        }

        public Library()
        {
        }

        #endregion

        #region Fields

        private IDictionary<string, QS.Fx.Language.Base.IType> types = new Dictionary<string, QS.Fx.Language.Base.IType>();

        #endregion

        #region ILibrary Members

        IEnumerable<QS.Fx.Language.Base.IType> QS.Fx.Language.Base.ILibrary.Types
        {
            get { return types.Values; }
        }

        QS.Fx.Language.Base.IType QS.Fx.Language.Base.ILibrary.GetType(string name)
        {
            QS.Fx.Language.Base.IType type;
            return types.TryGetValue(name, out type) ? type : null;
        }

        #endregion
    }
*/ 
}
