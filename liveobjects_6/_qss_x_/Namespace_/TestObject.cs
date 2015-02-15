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

namespace QS._qss_x_.Namespace_
{
    public sealed class TestObject : Object, IObject
    {
        public TestObject(ulong identifier, string name, Category category, string[] actions, QS.Fx.Logging.ILogger logger) 
            : base(identifier, name)
        {
            this.category = category;
            this.actions = actions;
            this.logger = logger;
        }

        private Category category;
        private string[] actions;
        private QS.Fx.Logging.ILogger logger;

        #region IObject Members

        Category IObject.Category
        {
            get { return category; }
        }

        IEnumerable<IAction> IObject.Actions
        {
            get 
            {
                IAction[] result = new IAction[actions.Length];
                for (int ind = 0; ind < actions.Length; ind++)
                    result[ind] = new Action((ulong)ind, actions[ind], 0);
                return result; 
            }
        }

        bool IObject.Invoke(ulong actionIdentifier, ulong actionContext)
        {
            logger.Log("Object \"" + name + "\" with id " + this.identifier.ToString() +
                " performs action \"" + actions[(int) actionIdentifier] + "\".");
            return true;
        }

        #endregion
    }
}
