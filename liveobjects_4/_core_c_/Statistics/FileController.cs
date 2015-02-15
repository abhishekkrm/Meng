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

// #define DEBUG_Logging

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._core_c_.Statistics
{
    [QS.Fx.Base.Inspectable]
    public class FileController : QS.Fx.Inspection.Inspectable, IStatisticsController, IDisposable
    {
        public const int DefaultMinimumChunkSize     = 100;
        public const int DefaultMaximumChunkSize    = 100000;
        public const double DefaultMaximumRate      = 1;

        public FileController(Core.ICore core, QS.Fx.Logging.ILogger logger, string root)
        {
            this.core = core;
            this.logger = logger;
            this.root = root;
        }

        private Core.ICore core;
        private QS.Fx.Logging.ILogger logger;
        private string root;
        [QS.Fx.Base.Inspectable]
        private IList<Collector> collectors = new List<Collector>();

        #region Class Collector

        [QS.Fx.Base.Inspectable]
        private class Collector : QS.Fx.Inspection.Inspectable, IDisposable
        {
            public Collector(Core.IFile outputfile, IDisposable samples)
            {
                this.outputfile = outputfile;
                this.samples = samples;
            }

            public Core.IFile outputfile;
            [QS.Fx.Base.Inspectable]
            public IDisposable samples;
            
            #region IDisposable Members

            void IDisposable.Dispose()
            {
                samples.Dispose();
                outputfile.Dispose();
            }

            #endregion
        }

        #endregion

        #region IStatisticsController Members

        ISamples1D IStatisticsController.Allocate1D(string name, string description, 
            string x_name, string x_units, string x_description, string y_name, string y_units, string y_description)
        {
            lock (this)
            {
                string filename = root + "\\" + QS._core_e_.Repository.ScalarAttribute.GenerateFileName(root);

#if DEBUG_Logging
                logger.Log(this, "Creating : " + filename);
#endif

                Core.IFile outputfile = core.OpenFile(filename);
                File1D samples = new File1D(outputfile, core, DefaultMinimumChunkSize, DefaultMaximumChunkSize, DefaultMaximumRate, 
                    name, name, description, x_name, x_units, x_description, y_name, y_units, y_description);
                collectors.Add(new Collector(outputfile, samples));
                return samples;
            }
        }

        ISamples2D IStatisticsController.Allocate2D(string name, string description, 
            string x_name, string x_units, string x_description, string y_name, string y_units, string y_description)
        {
            lock (this)
            {
                string filename = root + "\\" + QS._core_e_.Repository.ScalarAttribute.GenerateFileName(root);

#if DEBUG_Logging
                logger.Log(this, "Creating : " + filename);
#endif

                Core.IFile outputfile = core.OpenFile(filename);
                File2D samples = new File2D(outputfile, core, DefaultMinimumChunkSize, DefaultMaximumChunkSize, DefaultMaximumRate, 
                    name, name, description, x_name, x_units, x_description, y_name, y_units, y_description);
                collectors.Add(new Collector(outputfile, samples));
                return samples;
            }
        }

        void IStatisticsController.Close()
        {
            lock (this)
            {
                foreach (Collector collector in collectors)
                {
#if DEBUG_Logging
                    logger.Log(this, "Closing : " + collector.outputfile.Name);
#endif

                    ((IDisposable)collector).Dispose();
                }

                collectors.Clear();
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            ((IStatisticsController) this).Close();
        }

        #endregion
    }
}
