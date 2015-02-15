/*

Copyright (c) 2004-2009 Bo Peng, Qi Huang. All rights reserved.

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
using System.Text;
using System.Collections.Generic;

namespace Quilt.HostDetector.NATCheck
{
    /// <summary>
    /// This class implements NATChecker CHANGE-REQUEST attribute. Defined in RFC 3489 11.2.4.
    /// </summary>
    public class ChangeRequest
    {
        private bool m_ChangeIP = true;
        private bool m_ChangePort = true;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ChangeRequest()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="changeIP">Specifies if NATChecker server must send response to different IP than request was received.</param>
        /// <param name="changePort">Specifies if NATChecker server must send response to different port than request was received.</param>
        public ChangeRequest(bool changeIP, bool changePort)
        {
            m_ChangeIP = changeIP;
            m_ChangePort = changePort;
        }

        #region Properties Implementation

        /// <summary>
        /// Gets or sets if NATChecker server must send response to different IP than request was received.
        /// </summary>
        public bool ChangeIP
        {
            get { return m_ChangeIP; }
            set { m_ChangeIP = value; }
        }

        /// <summary>
        /// Gets or sets if NATChecker server must send response to different port than request was received.
        /// </summary>
        public bool ChangePort
        {
            get { return m_ChangePort; }
            set { m_ChangePort = value; }
        }

        #endregion

    }
}
