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

namespace QS._qss_e_.Data_
{
    [QS.Fx.Serialization.ClassID(QS.ClassID.QS_TMS_Data_Series)]
    public class Series : QS.Fx.Serialization.ISerializable
    {
        #region Metadata

        public class Metadata
        {
            public Metadata(string series_name, string series_description, int ndimensions, 
                string[] variable_names, string[] variable_units, string[] variable_descriptions)
            {
                if (variable_names.Length != ndimensions || variable_units.Length != ndimensions || variable_descriptions.Length != ndimensions)
                    throw new Exception("Corrupted metadata.");

                this.series_name = series_name;
                this.series_description = series_description;
                this.ndimensions = ndimensions;
                this.variable_names = variable_names;
                this.variable_units = variable_units;
                this.variable_descriptions = variable_descriptions;
            }

            private string series_name, series_description;
            private int ndimensions; 
            private string[] variable_names, variable_units, variable_descriptions;

            #region Accessors

            public string SeriesName
            {
                get { return series_name; }
            }

            public string SeriesDescription
            {
                get { return series_description; }
            }

            public int NumberOfDimensions
            {
                get { return ndimensions; }
            }

            public string[] VariableNames
            {
                get { return variable_names; }
            }

            public string[] VariableUnits
            {
                get { return variable_units; }
            }

            public string[] VariableDescriptions
            {
                get { return variable_descriptions; }
            }

            #endregion

            #region ToBytes

            public byte[] ToBytes()
            {
                List<string> strings = new List<string>();
                strings.Add(series_name);
                strings.Add(series_description);
                for (int ind = 0; ind < ndimensions; ind++)
                {
                    strings.Add(variable_names[ind]);
                    strings.Add(variable_units[ind]);
                    strings.Add(variable_descriptions[ind]);
                }

                int totalsize = sizeof(int);
                List<ArraySegment<byte>> blocks = new List<ArraySegment<byte>>();
                blocks.Add(new ArraySegment<byte>(BitConverter.GetBytes(ndimensions)));

                foreach (string s in strings)
                {
                    byte[] bytes = Encoding.Unicode.GetBytes(s);
                    blocks.Add(new ArraySegment<byte>(BitConverter.GetBytes(bytes.Length)));
                    blocks.Add(new ArraySegment<byte>(bytes));
                    totalsize += sizeof(int) + bytes.Length;
                }

                byte[] buffer = new byte[totalsize];
                int offset = 0;
                foreach (ArraySegment<byte> block in blocks)
                {
                    Buffer.BlockCopy(block.Array, block.Offset, buffer, offset, block.Count);
                    offset += block.Count;
                }

                return buffer;
            }

            #endregion

            #region FromBytes

            public static Metadata FromBytes(byte[] bytes)
            {
                int nconsumed;
                return FromBytes(new ArraySegment<byte>(bytes), out nconsumed);
            }

            public static Metadata FromBytes(ArraySegment<byte> segment, out int nconsumed)
            {
                int ndimensions = BitConverter.ToInt32(segment.Array, segment.Offset);
                nconsumed = sizeof(int);
                Queue<string> strings = new Queue<string>();
                for (int ind = 0; ind < 2 + 3 * ndimensions; ind++)
                {
                    int nbytes = BitConverter.ToInt32(segment.Array, segment.Offset + nconsumed);
                    nconsumed += sizeof(int);
                    string name = Encoding.Unicode.GetString(segment.Array, segment.Offset + nconsumed, nbytes);
                    nconsumed += nbytes;
                    strings.Enqueue(name);
                }
                string series_name = strings.Dequeue();
                string series_description = strings.Dequeue();
                string[] variable_names = new string[ndimensions];
                string[] variable_units = new string[ndimensions];
                string[] variable_descriptions = new string[ndimensions];
                for (int ind = 0; ind < ndimensions; ind++)
                {
                    variable_names[ind] = strings.Dequeue();
                    variable_units[ind] = strings.Dequeue();
                    variable_descriptions[ind] = strings.Dequeue();
                }
                return new Metadata(series_name, series_description, ndimensions, variable_names, variable_units, variable_descriptions);
            }

            #endregion

            #region ToString

            public override string ToString()
            {
                StringBuilder s = new StringBuilder();
                s.AppendLine("Name = " + series_name);
                s.AppendLine("Description = " + series_description);
                s.AppendLine("Dimensions = " + ndimensions);
                for (int ind = 0; ind < ndimensions; ind++)
                {
                    s.AppendLine("Variable[" + ind.ToString() + "].Name = " + variable_names[ind]);
                    s.AppendLine("Variable[" + ind.ToString() + "].Units = " + variable_units[ind]);
                    s.AppendLine("Variable[" + ind.ToString() + "].Description = " + variable_descriptions[ind]);
                }
                return s.ToString();
            }

            #endregion
        }

        #endregion

        public Series(string series_name, string series_description, int ndimensions, 
            string[] variable_names, string[] variable_units, string[] variable_descriptions, int nsamples, double[] data)
            : this(new Metadata(series_name, series_description, ndimensions, variable_names, variable_units, variable_descriptions), nsamples, data)
        {
        }

        public Series(Metadata metadata, int nsamples, double[] data)
        {
            if (data.Length != metadata.NumberOfDimensions * nsamples)
                throw new Exception("Amount of data inconsistent with metadata.");

            this.metadata = metadata;
            this.nsamples = nsamples;
            this.data = data;
        }

        public Series()
        {
        }

        private Metadata metadata;
        private int nsamples;
        private double[] data;

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            { 
                byte[] bytes = metadata.ToBytes();
                int header_size = sizeof(int) + bytes.Length;
                int data_size = nsamples * metadata.NumberOfDimensions * sizeof(double);
                return new QS.Fx.Serialization.SerializableInfo((ushort) ClassID.QS_TMS_Data_Series, header_size, header_size + data_size, 1);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte *pbuffer = header.Array)
            {
                *((int *) (pbuffer + header.Offset)) = nsamples;
            }
            header.consume(sizeof(int));
            byte[] bytes = metadata.ToBytes();
            Buffer.BlockCopy(bytes, 0, header.Array, header.Offset, bytes.Length);
            header.consume(bytes.Length);
            bytes = new byte[nsamples * metadata.NumberOfDimensions * sizeof(double)];
            fixed (byte *pbuffer = bytes)
            {
                for (int ind = 0; ind < this.data.Length; ind++)
                    *(((double *) pbuffer) + ind) = this.data[ind];
            }
            data.Add(new QS.Fx.Base.Block(bytes));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                nsamples = *((int*)(pbuffer + header.Offset));
            }
            header.consume(sizeof(int));
            int nconsumed;
            metadata = Metadata.FromBytes(new ArraySegment<byte>(header.Array, (int) header.Offset, (int) header.Count), out nconsumed);
            header.consume(nconsumed);
            this.data = new double[nsamples * metadata.NumberOfDimensions];
            fixed (byte* pdata = data.Array)
            {
                for (int ind = 0; ind < this.data.Length; ind++)
                    this.data[ind] = *(((double*)(pdata + data.Offset)) + ind);
            }
            data.consume(nsamples * metadata.NumberOfDimensions * sizeof(double));
        }

        #endregion
    }
}
