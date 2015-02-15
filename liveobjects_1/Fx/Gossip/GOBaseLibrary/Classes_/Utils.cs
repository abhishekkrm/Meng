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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization;
using System.Collections;
using GOBaseLibrary.Debugging;
using System.Net;

namespace GOBaseLibrary.Common
{
    public delegate Rumor ReceiveCallback();

    public static class Utils
    {
        public static EventLog eventLog = null;
        
        static public Assembly assem;
        static public string logFile;
        static XmlSerializer rumorListSerializer = new XmlSerializer(typeof(RumorList));
        static MemoryStream memoryStream = new MemoryStream();
        static ASCIIEncoding encodingAscii = new ASCIIEncoding();
        static UTF8Encoding encodingUtf = new UTF8Encoding();
        static object syncObj = new object();
        public static SpecialSwitch debugSwitch = new SpecialSwitch("GOBaseLibrary", "Gossip Service");
        public static FileStream outLogFile;

        public static void InitDebugging(DebugSwitchLevel level)
        {
            try
            {
                String BaseFileName = AppDomain.CurrentDomain.FriendlyName;

                string logFile = AppDomain.CurrentDomain.BaseDirectory + "logs\\" + BaseFileName + "_"
                                + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Year + ".log";

                Console.WriteLine("logfile: " + logFile);
                outLogFile = new FileStream(logFile, FileMode.OpenOrCreate);

                //Debug.Listeners.Add(new TextWriterTraceListener(
                //                    outLogFile));

                //Debug.AutoFlush = true;
                Trace.Listeners.Add(new TextWriterTraceListener(outLogFile));
                Trace.AutoFlush = true;
                Trace.UseGlobalLock = true;

                debugSwitch.Level = level;
            }
            catch (Exception e)
            {
                throw new UseCaseException("100", "Utils::InitDebugging " + e.Message + ", " + e.StackTrace);
            }
        }

        static Utils()
        {
            String ServiceName = "GossipService";
            String logName = "GossipServiceTestLog";

            if (!EventLog.SourceExists(ServiceName))
            {
                EventLog.CreateEventSource(ServiceName, logName);
            }

            eventLog = new EventLog();
            eventLog.Source = ServiceName;
        }

        /// <summary>
        /// Serialize RumorDigest object to an XML representation
        /// </summary>
        /// <param name="rumor">RumorDigest to be serialized</param>
        /// <returns>xml serialized version of RumorDigest</returns>
        public static byte[] SerializeDigestXML(RumorList digest)
        {
            lock (syncObj)
            {
                try{
                    memoryStream = new MemoryStream();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

                    rumorListSerializer.Serialize(xmlTextWriter, digest);
                    memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                }
                catch (Exception e)
                {
                    throw new UseCaseException("120", "Utils::SerializeDigestXML " + e + ", " + e.StackTrace);
                }

                return memoryStream.ToArray();
            }
        }

        public static byte[] SerializeDigestBinary(RumorList digest)
        {
            try
            {
                MemoryStream outStream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(outStream);

                if (digest == null)
                {
                    return null;
                }

                List<Rumor> rumors = digest.GetAll();

                /*
                 * <Int32> number of rumors in this message
                 * <String> rumor id
                 * <Int32> hopcount
                 * <String> source group
                 * <String> destination group
                 * <String> source node
                 * <String> destination node
                 * <String> payload
                 * */
                writer.Write((Int32)rumors.Count);
                foreach (Rumor rumor in rumors)
                {
                        writer.Write(rumor.Id);
                        writer.Write((int)rumor.Type);
                        writer.Write(rumor.HopCount);
                        writer.Write(rumor.SourceGroup.Id);
                        writer.Write(rumor.DestinationGroup.Id);
                        writer.Write(rumor.SourceNode.Id);
                        writer.Write(rumor.SourceNode.Ip);
                        writer.Write(rumor.DestinationNode.Id);
                        writer.Write(rumor.PayLoad);
                        writer.Write(rumor.Age);
                }

                return outStream.ToArray();
            }
            catch (Exception e)
            {
                throw new UseCaseException("140", "Utils::SerializeDigestBinary " + e + ", " + e.StackTrace);
            }
        }


        /// <summary>
        /// Method to reconstruct an Object from XML string representation of a RumorDigest
        /// </summary>
        /// <param name="pXmlizedString">xml string representation of RumorDigest</param>
        /// <returns>Object representation of deserialized xml string</returns>
        public static Object DeserializeDigestXML(byte[] pXmlizedString)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream(pXmlizedString);
                return rumorListSerializer.Deserialize(memoryStream);
            }
            catch (Exception e)
            {
                throw new UseCaseException("160", "Utils::DeserializeDigestXML " + e + ", " + e.StackTrace);
            }
        }

        public static Object DeserializeDigestBinary(byte[] pXmlizedString)
        {
            try
            {
                MemoryStream inStream = new MemoryStream(pXmlizedString);
                BinaryReader reader = new BinaryReader(inStream);
                RumorList rumorList = new RumorList();

                Int32 numberOfRumors = reader.ReadInt32();

                lock (syncObj)
                {
                    for (int i = 0; i < numberOfRumors; i++)
                    {
                        Rumor rumor = new Rumor();

                        rumor.Id = reader.ReadString();
                        rumor.Type = (GossipType)reader.ReadInt32();
                        rumor.HopCount = reader.ReadInt32();
                        rumor.SourceGroup = new Group(reader.ReadString(), 1000);
                        rumor.DestinationGroup = new Group(reader.ReadString(), 1000);
                        rumor.SourceNode = new Node(reader.ReadString());
                        rumor.SourceNode.Ip = reader.ReadString();
                        rumor.DestinationNode = new Node(reader.ReadString());
                        rumor.PayLoad = reader.ReadString();
                        rumor.Age = reader.ReadInt64();

                        rumorList.AddRumorAsIs(rumor);

                    }
                }

                return rumorList;
            }
            catch (Exception e)
            {
                throw new UseCaseException("180", "Utils::DeserializeDigestBinary " + e + ", " + e.StackTrace);
            }
        }


        // Create a byte array from a integer.
        public static byte[] IntegerToByteArray(Int32 src)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(src);
                    return stream.ToArray();
                }
            }
        }

        // Create a byte array from a integer.
        public static Int32 ByteArrayToInteger(byte[] src)
        {
            using (MemoryStream stream = new MemoryStream(src))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    return reader.ReadInt32();
                }
            }
        }

        // Create a byte array from a decimal.
        public static byte[] StringToByteArray(String src)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(src);
                    return stream.ToArray();
                }
            }
        }

        // Create a byte array from a decimal.
        public static String ByteArrayToString(byte[] src)
        {
            using (MemoryStream stream = new MemoryStream(src))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    return reader.ReadString();
                }
            }
        }


        /// <summary>
        /// To convert a Byte Array of Unicode values (ASCII encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from ASCII Byte Array</returns>
        static private String ASCIIByteArrayToString(Byte[] characters)
        {
            String constructedString = encodingAscii.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// Converts the String to ASCII Byte array and is used in de-serialization
        /// </summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        static private Byte[] StringToASCIIByteArray(String pXmlString)
        {
            Byte[] byteArray = encodingUtf.GetBytes(pXmlString);
            return byteArray;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }


        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static String GetLocalIPAddress()
        {
            IPAddress[] a = Dns.GetHostByName(Dns.GetHostName()).AddressList;
    
            return a != null && a.Length > 0 
                    ? a[0].ToString()
                    : null;

        }
    }
}
