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
using System.Threading;

namespace QS._core_c_.Statistics
{
    [QS.Fx.Base.Inspectable]
    public sealed class File2D : QS.Fx.Inspection.Inspectable, IDisposable, ISamples2D, IFileOutput, Diagnostics.IDataCollector
    {
        #region Constructor

        public unsafe File2D(Core.IFile file, QS.Fx.Clock.IClock clock, int minimumChunkSize, int maximumChunkSize, double maximumRate,
            string attribute_name, string series_name, string series_description,
            string x_name, string x_units, string x_description, string y_name, string y_units, string y_description)
        {
            this.clock = clock;
            this.file = file;
            this.minimumChunkSize = minimumChunkSize;
            this.maximumChunkSize = maximumChunkSize;
            this.chunksize = minimumChunkSize;

            this.attribute_name = attribute_name;
            this.series_name = series_name;
            this.series_description = series_description;
            this.x_name = x_name;
            this.x_units = x_units;
            this.x_description = x_description;
            this.y_name = y_name;
            this.y_units = y_units;
            this.y_description = y_description;

            this.lastchecked = clock.Time;
            this.maximumRate = maximumRate;

            byte[] attribute_name_bytes = Encoding.Unicode.GetBytes(attribute_name);
            Base2.StringWrapper series_name_stringwrapper = new QS._core_c_.Base2.StringWrapper(series_name);
            Base2.StringWrapper series_description_stringwrapper = new QS._core_c_.Base2.StringWrapper(series_description);

            index1 = sizeof(int) + attribute_name_bytes.Length + sizeof(byte);
            index2 = index1 + 2 * sizeof(int) + sizeof(short) +
                series_name_stringwrapper.SerializableInfo.HeaderSize + series_description_stringwrapper.SerializableInfo.HeaderSize;
            index3 = index2 + sizeof(int);

            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock((uint) index3);
            IList<QS.Fx.Base.Block> buffers = new List<QS.Fx.Base.Block>();

            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                *((int*)pheader) = attribute_name_bytes.Length;
                pheader += sizeof(int);
                for (int ind = 0; ind < attribute_name_bytes.Length; ind++)
                {
                    *pheader = attribute_name_bytes[ind];
                    pheader++;
                }
                *pheader = (byte) Serialization.SerializationClass.Base3_ISerializable;
                pheader += sizeof(byte);
                *((int*)pheader) = 0;
                *((int*) (pheader + sizeof(int))) = 0;
                *((ushort*) (pheader + 2 * sizeof(int))) = (ushort)ClassID.TMS_Data_Data2D;                
            }
            header.consume(3 * sizeof(int) + attribute_name_bytes.Length + sizeof(byte) + sizeof(short));

            series_name_stringwrapper.SerializeTo(ref header, ref buffers);
            series_description_stringwrapper.SerializeTo(ref header, ref buffers);

            fixed (byte* parray = header.Array)
            {
                *((int*)(parray + header.Offset)) = 0;
            }

            header.reset();

            file.Write(0, header.Array, header.Count);
        }

        #endregion

        #region Fields

        private QS.Fx.Clock.IClock clock;
        private Core.IFile file;
        private string attribute_name, series_name, series_description, x_name, x_units, x_description, y_name, y_units, y_description;
        private int index1, index2, index3; // first is the 8-byte headersize/totalsize chunk, second is 4-byte nsamples, third is data offset
        private int nsamples, chunksize, minimumChunkSize, maximumChunkSize, chunkoffset;
        private bool closed;
        private double lastchecked, maximumRate, 
            minx = double.MaxValue, maxx = double.MinValue, miny = double.MaxValue, maxy = double.MinValue;
        private double[] chunk;

        #endregion

        #region Add

        public void Add(double x, double y)
        {
            lock (this)
            {
                if (closed)
                {
                    //throw new Exception("Already closed.");
                }
                else
                {
                    if (chunk == null)
                        chunk = new double[chunksize * 2];

                    chunk[chunkoffset] = x;
                    chunk[chunkoffset + 1] = y;
                    chunkoffset += 2;

                    nsamples++;
                    if (x < minx)
                        minx = x;
                    if (x > maxx)
                        maxx = x;
                    if (y < miny)
                        miny = y;
                    if (y > maxy)
                        maxy = y;

                    if (chunkoffset == 2 * chunksize)
                    {
                        file.Write(index3, chunk, chunkoffset * sizeof(double));

                        index3 += chunkoffset * sizeof(double);
                        chunk = null;
                        chunkoffset = 0;

                        double now = clock.Time;
                        int optimalchunks = (int)Math.Ceiling((((double)chunksize) / (now - lastchecked)) / maximumRate);
                        lastchecked = now;

                        if (chunksize < optimalchunks)
                        {
                            while (chunksize < optimalchunks)
                                chunksize = 2 * chunksize;
                            chunksize = Math.Min(chunksize, maximumChunkSize);
                        }
                        else
                        {
                            if (chunksize > 2 * optimalchunks)
                                chunksize = Math.Max(chunksize / 2, minimumChunkSize);
                        }


                        chunksize = Math.Min(2 * chunksize, maximumChunkSize);
                    }
                }
            }
        }

        #endregion

        #region Flush

        public void Flush()
        {
            lock (this)
            {
                if (chunk != null && chunkoffset > 0)
                {
                    file.Write(index3, chunk, chunkoffset * sizeof(double));

                    index3 += chunkoffset * sizeof(double);
                    chunk = null;
                    chunkoffset = 0;
                }
            }
        }

        #endregion

        #region IDisposable Members

        unsafe void IDisposable.Dispose()
        {
            lock (this)
            {
                if (!closed)
                {
                    closed = true;

                    Flush();

                    Base2.StringWrapper series_name_stringwrapper = new QS._core_c_.Base2.StringWrapper(series_name);
                    Base2.StringWrapper series_description_stringwrapper = new QS._core_c_.Base2.StringWrapper(series_description);
                    QS._core_e_.Data.Axis xAxis = new QS._core_e_.Data.Axis(x_name, x_units, new QS._core_e_.Data.Range(minx, maxx), x_description);
                    QS._core_e_.Data.Axis yAxis = new QS._core_e_.Data.Axis(y_name, y_units, new QS._core_e_.Data.Range(miny, maxy), y_description);

                    QS.Fx.Base.ConsumableBlock header1 = new QS.Fx.Base.ConsumableBlock(
                        (uint)(series_name_stringwrapper.SerializableInfo.HeaderSize + series_description_stringwrapper.SerializableInfo.HeaderSize));
                    QS.Fx.Base.ConsumableBlock header2 = new QS.Fx.Base.ConsumableBlock(
                        (uint)(((QS.Fx.Serialization.ISerializable)xAxis).SerializableInfo.HeaderSize + ((QS.Fx.Serialization.ISerializable)yAxis).SerializableInfo.HeaderSize));
                    IList<QS.Fx.Base.Block> buffers = new List<QS.Fx.Base.Block>();

                    series_name_stringwrapper.SerializeTo(ref header1, ref buffers);
                    series_description_stringwrapper.SerializeTo(ref header1, ref buffers);
                    ((QS.Fx.Serialization.ISerializable)xAxis).SerializeTo(ref header2, ref buffers);
                    ((QS.Fx.Serialization.ISerializable)yAxis).SerializeTo(ref header2, ref buffers);

                    header1.reset();
                    header2.reset();

                    int totalbuffers = 0;
                    foreach (QS.Fx.Base.Block s in buffers)
                        totalbuffers += (int) s.size;

                    int[] flush1 = new int[2];
                    flush1[1] = 2 * sizeof(int) + 3 * sizeof(ushort) + header1.Count + header2.Count + 2 * sizeof(double) * nsamples;
                    flush1[0] = flush1[1] + totalbuffers;

                    file.Write(index1, flush1, 2 * sizeof(int));
                    file.Write(index2, BitConverter.GetBytes(nsamples), sizeof(int));

                    byte[] flush2 = new byte[2 * sizeof(ushort) + header2.Count + totalbuffers];
                    fixed (byte* pbuf = flush2)
                    {
                        byte* pflush = pbuf;

                        *((ushort*)pflush) = (ushort)ClassID.TMS_Data_Axis;
                        *((ushort*)(pflush + sizeof(ushort))) = (ushort)ClassID.TMS_Data_Axis;
                        pflush += 2 * sizeof(ushort);
                        for (int ind = 0; ind < header2.Count; ind++)
                        {
                            *pflush = header2.Array[ind];
                            pflush++;
                        }
                        foreach (QS.Fx.Base.Block s in buffers)
                        {
                            if ((s.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && s.buffer != null)
                                for (int ind = 0; ind < s.size; ind++)
                                {
                                    *pflush = s.buffer[(int)s.offset + ind];
                                    pflush++;
                                }
                            else
                                throw new Exception("Unmanaged memory not supported.");
                        }
                    }

                    file.Write(index3, flush2, flush2.Length);
                }
            }
        }

        #endregion

        #region IFileOutput Members

        string IFileOutput.Filename
        {
            get { return file.Name; }
        }

        #endregion

        #region IDataCollector Members

        QS._core_e_.Data.IDataSet QS._core_c_.Diagnostics.IDataCollector.DataSet
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IDiagnosticsComponent Members

        QS.Fx.Diagnostics.ComponentClass QS.Fx.Diagnostics.IDiagnosticsComponent.Class
        {
            get { return QS.Fx.Diagnostics.ComponentClass.DataCollector; }
        }

        bool QS.Fx.Diagnostics.IDiagnosticsComponent.Enabled
        {
            get { return true; }
            set
            {
                if (!value)
                    throw new NotSupportedException();
            }
        }

        void QS.Fx.Diagnostics.IDiagnosticsComponent.ResetComponent()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
