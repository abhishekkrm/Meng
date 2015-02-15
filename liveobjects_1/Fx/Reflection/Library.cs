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
    public static class Library
    {
        public interface ILibrary
        {
            IValueClass ValueClass<C>();
            IValueClass ValueClass(string uuid, params QS.Fx.Reflection.Parameter[] parameters);

            IInterfaceClass InterfaceClass<C>() where C : class, QS.Fx.Interface.Classes.IInterface;
            IInterfaceClass InterfaceClass(string uuid, params QS.Fx.Reflection.Parameter[] parameters);

            IEndpointClass EndpointClass<C>() where C : class, QS.Fx.Endpoint.Classes.IEndpoint;
            IEndpointClass EndpointClass(string uuid, params QS.Fx.Reflection.Parameter[] parameters);

            IObjectClass ObjectClass<C>() where C : class, QS.Fx.Object.Classes.IObject;
            IObjectClass ObjectClass(string uuid, params QS.Fx.Reflection.Parameter[] parameters);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> Object(string uuid, params QS.Fx.Reflection.Parameter[] parameters);
        }

        public static ILibrary LocalLibrary
        {
            get { return mycreate; }
        }

        private static ILibrary mycreate;

        public static void Initialize(ILibrary create)
        {
            if (System.Threading.Interlocked.CompareExchange<ILibrary>(ref mycreate, create, null) != null)
                throw new Exception(
                    "Could not initialize the library because it has already been initialized.");
        }
    }
}
