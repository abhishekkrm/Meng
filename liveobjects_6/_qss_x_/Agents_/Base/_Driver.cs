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

namespace QS._qss_x_.Agents_.Driver
{
/*
    public abstract class Driver : QS.TMS.Inspection.Inspectable, IDriver
    {
        #region Constructor

        protected Driver()
        {
        }

        #endregion

        #region Fields

        protected QS.Fx.Base.AgentID id;
        protected Membership.IView view;
        protected Agents.Driver.IDriverContext context;
        protected int memberno;
        protected bool isleader, isnormal;

        #endregion

        #region Methods to override

        protected virtual void _Initialize()
        {
        }

        protected virtual void _Reconfigure()
        {
        }

        protected virtual void _Receive(uint viewno, uint memberno, QS.Fx.Serialization.ISerializable message)
        {
        }

        protected virtual void _Cleanup()
        {
        }

        #endregion

        #region _Calculate

        private void _Calculate()
        {
            memberno = -1;
            for (int ind = 0; ind < view.Members.Length; ind++)
            {
                Membership.IMember member = view.Members[ind];
                if (((IEquatable<QS.Fx.Base.QualifiedName>)member.ID).Equals(id.Subdomain))
                {
                    System.Diagnostics.Debug.Assert(member.Channel == null);
                    memberno = ind;
                    break;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(member.Channel != null);
                }
            }

            System.Diagnostics.Debug.Assert(memberno >= 0 && memberno < view.Members.Length);

            isnormal = view.Members[memberno].Type == QS.Fx.Agents.Membership.MemberType.Normal;
            isleader = (memberno == 0);
        }

        #endregion

        #region IDriver Members

        void QS.Fx.Agents.Driver.IDriver.Initialize(Agents.Driver.IDriverContext context)
        {
            lock (this)
            {
                this.context = context;
                this.id = context.ID;
                this.view = context.View;
                _Calculate();
                _Initialize();
            }
        }

        void QS.Fx.Agents.Driver.IDriver.Reconfigure()
        {
            lock (this)
            {
                this.view = context.View;
                _Calculate();
                _Reconfigure();
            }
        }

        void QS.Fx.Agents.Driver.IDriver.Receive(uint viewno, uint memberno, QS.Fx.Serialization.ISerializable message)
        {
            lock (this)
            {
                _Receive(viewno, memberno, message);
            }
        }

        void IDriver.Cleanup()
        {
            lock (this)
            {
                _Cleanup();
            }
        }

        #endregion
    }
*/ 
}
