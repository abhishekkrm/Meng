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

using QS._qss_x_.Runtime_1_;

namespace QS._qss_x_.Protocols_
{
    public sealed class LastCopy
    {
/*
        public sealed class _Local
        {
            public UIntSet Stable, CanCommit, HeardOf, Missing, Cached;
            public Indexed<Address, UIntSet> Push;
        }

        public sealed class _Aggr
        {
            public UIntSet Cached, Missing, HeardOf, Stable;

            public _Aggr(_Local _local)
            {
                this.Cached = ((IValue<UIntSet>) _local.Cached).Clone();
                this.Missing = ((IValue<UIntSet>) _local.Missing).Clone();
                this.HeardOf = ((IValue<UIntSet>) _local.HeardOf).Clone();
                this.Stable = ((IValue<UIntSet>) _local.Stable).Clone();
            }
        }

        public sealed class _Diss
        {
            public UIntSet CanCleanup, HeardOf;
            public Indexed<Address, UIntSet> Push;

            public _Diss(_Local _local)
            {
                this.CanCleanup = ((IValue<UIntSet>) _local.CanCleanup).Clone();
                this.HeardOf = ((IValue<UIntSet>) _local.HeardOf).Clone();
                this.Push = ((IValue<UIntSet>) _local.Push).Erase();
            }
        }
*/ 
    }
}
