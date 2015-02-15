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

namespace QS._qss_p_.Structure_
{
    public interface IEnvironment_
    {
        IEnvironment_ _Environment
        {
            get;
        }

        void _Add(IDeclaration_ _declaration);

        IDeclaration_ _Get(string _identifier);
        IDeclaration_ _Get(string _identifier, Declaration_.Type_ _type);
        DeclarationClass _Get<DeclarationClass>(string _identifier) where DeclarationClass : IDeclaration_;
        DeclarationClass _Get<DeclarationClass>(string _identifier, Declaration_.Type_ _type) where DeclarationClass : IDeclaration_;
        
        bool _Get(string _identifier, out IDeclaration_ _declaration);
        bool _Get(string _identifier, Declaration_.Type_ _type, out IDeclaration_ _declaration);
        bool _Get<DeclarationClass>(string _identifier, out DeclarationClass _declaration) where DeclarationClass : IDeclaration_;
        bool _Get<DeclarationClass>(string _identifier, Declaration_.Type_ _type, out DeclarationClass _declaration) where DeclarationClass : IDeclaration_;
    }
}
