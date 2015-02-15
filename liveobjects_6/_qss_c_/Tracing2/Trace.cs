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

namespace QS._qss_c_.Tracing2
{
    public sealed class Trace : ITrace, QS.Fx.Serialization.ISerializable
    {
        public Trace(string trace_name, string trace_description, 
            int num_dimensions, string[] series_names, string[] series_units, string[] series_descriptions) 
            : this(new Metadata(trace_name, trace_description, num_dimensions, series_names, series_units, series_descriptions))
        {
        }

        public Trace(Metadata metadata)
        {
            this.metadata = metadata;
        }

        public Trace()
        {
        }

        private Metadata metadata;

        #region Class Metadata

        public sealed class Metadata : QS.Fx.Serialization.ISerializable
        {
            public Metadata(string trace_name, string trace_description,
                int num_dimensions, string[] series_names, string[] series_units, string[] series_descriptions)
            {
                this.trace_name = trace_name;
                this.trace_description = trace_description;
                this.num_dimensions = num_dimensions;
                this.series_names = series_names;
                this.series_units = series_units;
                this.series_descriptions = series_descriptions;
            }

            public Metadata()
            {
            }

            private string trace_name, trace_description;
            private int num_dimensions;
            private string[] series_names, series_units, series_descriptions;

            public int NumberOfDimensions
            {
                get { return num_dimensions; }
            }

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        #endregion

        #region ITrace Members

        void ITrace.Add(params double[] values)
        {
            if (values.Length != metadata.NumberOfDimensions)
                throw new Exception("Supplied " + values.Length.ToString() + " while the trace has " + 
                    metadata.NumberOfDimensions.ToString() + " dimensions.");






            // .....................................................................
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {









            get { throw new Exception("The method or operation is not implemented."); }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            





            throw new Exception("The method or operation is not implemented.");
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {






            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
