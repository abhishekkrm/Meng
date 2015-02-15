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

namespace QS.Fx.Base
{
    public static class Synchronization
    {
        public static QS.Fx.Base.SynchronizationOption CombineOptions(QS.Fx.Base.SynchronizationOption _defaults, QS.Fx.Base.SynchronizationOption _override)
        {
            QS.Fx.Base.SynchronizationOption _options = SynchronizationOption.None;
            QS.Fx.Base.SynchronizationOption _mask;

            _mask = QS.Fx.Base.SynchronizationOption.Asynchronous;
            _options = _options | (_defaults & _mask) | (_override & _mask); // inherited

            _mask = QS.Fx.Base.SynchronizationOption.Synchronous;
            _options = _options | (_defaults & _mask) | (_override & _mask); // inherited

            _mask = QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Synchronous;
            if ((_options & _mask) == _mask)
                _options = _options & ~QS.Fx.Base.SynchronizationOption.Asynchronous; // overridden by synchronous

            _mask = QS.Fx.Base.SynchronizationOption.Multithreaded;
            _options = _options | (_defaults & _mask) & (_override & _mask); // non-inherited

            _mask = QS.Fx.Base.SynchronizationOption.Singlethreaded;
            _options = _options | (_defaults & _mask) | (_override & _mask); // inherited

            _mask = QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Singlethreaded;
            if ((_options & _mask) == _mask)
                _options = _options & ~QS.Fx.Base.SynchronizationOption.Multithreaded; // overridden by singlethreaded

            _mask = QS.Fx.Base.SynchronizationOption.Concurrent;
            _options = _options | (_defaults & _mask) | (_override & _mask); // inherited

            _mask = QS.Fx.Base.SynchronizationOption.Serialized;
            _options = _options | (_defaults & _mask) | (_override & _mask); // inherited

            _mask = QS.Fx.Base.SynchronizationOption.Concurrent | QS.Fx.Base.SynchronizationOption.Serialized;
            if ((_options & _mask) == _mask)
                _options = _options & ~QS.Fx.Base.SynchronizationOption.Concurrent; // overridden by serialized

            _mask = QS.Fx.Base.SynchronizationOption.Replicated;
            _options = _options | (_defaults & _mask) | (_override & _mask); // inherited

            _mask = QS.Fx.Base.SynchronizationOption.Aggregated;
            _options = _options | (_defaults & _mask) | (_override & _mask); // inherited

            _mask = QS.Fx.Base.SynchronizationOption.Replicated | QS.Fx.Base.SynchronizationOption.Aggregated;
            if ((_options & _mask) == _mask)
                _options = _options & ~QS.Fx.Base.SynchronizationOption.Replicated; // overridden by aggregated

            _mask = QS.Fx.Base.SynchronizationOption.Replicated;
            if ((_options & _mask) == _mask)
                _options = _options & ~QS.Fx.Base.SynchronizationOption.Concurrent; // overridden by replicated

            _mask = QS.Fx.Base.SynchronizationOption.Aggregated;
            if ((_options & _mask) == _mask)
                _options = _options & ~QS.Fx.Base.SynchronizationOption.Concurrent; // overridden by aggregated

            return _options;
        }
    }
}
