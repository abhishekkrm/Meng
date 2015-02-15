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

#define OPTION_EnableLogging
#define OPTION_SanityChecking

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Backbone_.Scope.Controllers
{
    /// <summary>
    /// A simple controller that creates a single recovery domain for the entire scope, for all topics simultaneously.
    /// </summary>
    public sealed class Controller1a : Controller, IController
    {
        #region Constructor

        public Controller1a(IControllerContext context) : base(context)
        {
            localdomain = context.CreateLocal("common");
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private ILocalDomain localdomain;

        #endregion

        #region IController Members

        #region Register(topic)

        void IController.Register(ITopic topic)
        {
            topic.Root = localdomain;
        }

        #endregion

        #region Register(domain)

        void IController.Register(IDomain domain)
        {
            localdomain.Register(domain, MembershipType.Active);
        }

        #endregion

        #region Register(domain,topic,membershiptype)

        void IController.Register(IDomain domain, ITopic topic, MembershipType membershiptype)
        {
            if (membershiptype != MembershipType.Active)
                throw new Exception("Currently only active membership is supported.");
        }

        #endregion

        #region Unregister(domain)

        void IController.Unregister(IDomain domain)
        {
            localdomain.Unregister(domain);
        }

        #endregion

        #region Unregister(topic)

        void IController.Unregister(ITopic topic)
        {
            topic.Root = null;
        }

        #endregion

        #endregion
    }
}
