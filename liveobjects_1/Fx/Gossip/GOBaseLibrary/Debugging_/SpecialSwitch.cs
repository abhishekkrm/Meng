/*

Copyright (c) 2004-2009 Deepak Nataraj. All rights reserved.

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
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GOBaseLibrary.Debugging
{
    public enum DebugSwitchLevel
    {
        Mute = 0,
        Terse = 1,
        Verbose = 2,
        VeryVerbose = 3
    }

    public class SpecialSwitch : Switch
    {
        public SpecialSwitch(string displayName, string description) :
            base(displayName, description)
        {
        }

        public DebugSwitchLevel Level
        {
            get
            {
                return ((DebugSwitchLevel)base.SwitchSetting);
            }
            set
            {
                base.SwitchSetting = (int)value;
            }
        }
        public bool Mute
        {
            get
            {
                return (base.SwitchSetting == 0);
            }
        }

        public bool Terse
        {
            get
            {
                return (base.SwitchSetting >= (int)(DebugSwitchLevel.Terse));
            }
        }
        public bool Verbose
        {
            get
            {
                return (base.SwitchSetting >= (int)DebugSwitchLevel.Verbose);
            }
        }
        public bool VeryVerbose
        {
            get
            {
                return (base.SwitchSetting >= (int)DebugSwitchLevel.VeryVerbose);
            }
        }

        protected new int SwitchSetting
        {
            get
            {
                return ((int)base.SwitchSetting);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 4)
                    value = 4;

                base.SwitchSetting = value;
            }
        }
    }
}
