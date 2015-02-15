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

namespace QS._qss_c_.Statistics_
{
    [QS._core_c_.Diagnostics.Component("Statistics_Controller")]
    [QS.Fx.Base.Inspectable]
    public class MemoryController : QS.Fx.Inspection.Inspectable, QS._core_c_.Statistics.IStatisticsController, IDisposable, QS._core_c_.Diagnostics2.IModule
    {
        public MemoryController()
        {
        }

        [QS.Fx.Inspection.Ignore]
        [QS._core_c_.Diagnostics.Ignore]
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        [QS._core_c_.Diagnostics.ComponentCollection("Registered_Statistics")]        
        private IList<QS._core_c_.Diagnostics.IDataCollector> registered = new List<QS._core_c_.Diagnostics.IDataCollector>();

        public QS._core_c_.Diagnostics2.IContainer DiagnosticsContainer
        {
            get { return diagnosticsContainer; }
        }

        #region IStatisticsController Members

        QS._core_c_.Statistics.ISamples1D QS._core_c_.Statistics.IStatisticsController.Allocate1D(string name, string description, string x_name, string x_units, string x_description, string y_name, string y_units, string y_description)
        {
            lock (this)
            {
                Samples1D samples = new Samples1D(
                    Samples1D.LoggingOption.Once, name, description, x_name, x_units, x_description, y_name, y_units, y_description);
                registered.Add(samples);
                ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(name, new QS._core_c_.Diagnostics2.Property(samples));
                return samples;
            }
        }

        QS._core_c_.Statistics.ISamples2D QS._core_c_.Statistics.IStatisticsController.Allocate2D(string name, string description, string x_name, string x_units, string x_description, string y_name, string y_units, string y_description)
        {
            lock (this)
            {
                Samples2D samples = new Samples2D(name, description, x_name, x_units, x_description, y_name, y_units, y_description);
                registered.Add(samples);
                ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(name, new QS._core_c_.Diagnostics2.Property(samples));
                return samples;
            }
        }

        void QS._core_c_.Statistics.IStatisticsController.Close()
        {
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public void Clear()
        {
            lock (this)
            {
                diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
                registered.Clear();
            }
        }
    }
}
