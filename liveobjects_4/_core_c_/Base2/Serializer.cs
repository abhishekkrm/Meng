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

// #define DEBUG_Serializer

using System;
using System.Diagnostics;
using System.Net;

namespace QS._core_c_.Base2
{
	/// <summary>
	/// Summary description for Serializer.
	/// </summary>

	public interface ISerializer
	{
        Base2.IBase2Serializable CreateObject(ushort classID);
        void registerClass(ushort classID, System.Type type);

        Base2.IBase2Serializable CreateObject(QS.ClassID classID);
        void registerClass(QS.ClassID classID, System.Type type);

		void registerClasses();
		void registerClasses(QS.Fx.Logging.ILogger logger);
		void registerClasses(QS.Fx.Logging.ILogger logger, System.Reflection.Assembly assembly);

/*
		uint typeOf(ISerializableObject serializableObject);

		void serialize(ISerializableObject serializableObject, IBlockOfData blockOfData);
		IBlockOfData serialize(ISerializableObject serializableObject);

		ISerializableObject deserialize(IBlockOfData blockOfData);
*/
	}

	public sealed class Serializer : ISerializer
	{
		public void registerClasses()
		{
			this.registerClasses(null, null);
		}

		public void registerClasses(QS.Fx.Logging.ILogger logger)
		{
			this.registerClasses(logger, null);
		}

        public void registerClasses(QS.Fx.Logging.ILogger logger, System.Reflection.Assembly assembly)
        {
            registerClasses(logger, assembly, typeof(QS._core_c_.Base2.IBase2Serializable), new RegisterClassCallback(this.registerClass));
        }

        public delegate void RegisterClassCallback(ushort classID, System.Type type);

        public static void registerClasses(
            QS.Fx.Logging.ILogger logger, System.Reflection.Assembly assembly, System.Type serializableType, RegisterClassCallback registerClassCallback)
        {
            try
            {
                if (assembly == null)
                    assembly = System.Reflection.Assembly.GetAssembly(serializableType);

                foreach (System.Type type in assembly.GetTypes())
                {
                    try
                    {
                        if (serializableType.IsAssignableFrom(type) && type.IsClass)
                        {
                            object[] attribs = type.GetCustomAttributes(typeof(QS.Fx.Serialization.ClassIDAttribute), false);
                            if (attribs.Length > 0)
                            {
                                ushort classID = ((QS.Fx.Serialization.ClassIDAttribute)attribs[0]).ClassID;
                                if (!classID.Equals((ushort)QS.ClassID.Nothing))
                                {
#if DEBUG_Serializer
							logger.Log(this, "Registering " + type.FullName);
#endif
                                    registerClassCallback(classID, type);
                                }
                                else
                                {
#if DEBUG_Serializer
							logger.Log(this, "Ignoring " + type.FullName);
#endif
                                }
                            }
                            else
                            {
                                if (logger != null)
                                    logger.Log(null, "Cannot register class : " + type.FullName);
                            }
                        }
                    }
                    catch (Exception _exc)
                    {
                        throw new Exception("Could not register type \"" + type.Name + "\".", _exc);
                    }
                }
            }
            catch (Exception _exc)
            {
                throw new Exception("Could not register some serializable types in assembly \"" + assembly.FullName + "\".", _exc);
            }
		}

		private static object[] emptyObjects = new object[] {};
		private const uint defaultAnticipatedNumberOfRegisteredClasses = 100;
		private static ISerializer commonSerializer = new Serializer(defaultAnticipatedNumberOfRegisteredClasses);

		public static ISerializer CommonSerializer
		{
			get
			{
				return commonSerializer;
			}
		}

		public Serializer(uint anticipatedNumberOfRegisteredClasses)
		{
			this.classID2ConstructorMappings = new Collections.Hashtable(anticipatedNumberOfRegisteredClasses);
		}

		private Collections.IDictionary classID2ConstructorMappings;

		public void registerClass(ushort classID, System.Type type)
		{
            registerClassWith(classID, type, classID2ConstructorMappings);
        }

        public void registerClass(QS.ClassID classID, System.Type type)
        {
            registerClassWith((ushort) classID, type, classID2ConstructorMappings);
        }

        public static void registerClassWith(ushort classID, System.Type type, Collections.IDictionary classID2ConstructorMappings)
        {
			System.Reflection.ConstructorInfo constructorInfo = type.GetConstructor(System.Type.EmptyTypes);
			if (constructorInfo == null)
			{
				throw new Exception("Class " + type.ToString() + " does not have a default no-paremeters public " +
					"constructor and cannot be registered with this serializer.");
			}

			Collections.IDictionaryEntry dic_en = classID2ConstructorMappings.lookupOrCreate(classID);
			if (dic_en.Value != null)
			{
				System.Reflection.ConstructorInfo oldconstructorInfo = (System.Reflection.ConstructorInfo) dic_en.Value;

				if (!oldconstructorInfo.Equals(constructorInfo))
				{
					throw new Exception("Conflicting registration: cannot register type \"" + type.Name + "\", class id \"" + classID.ToString() + 
						"\" has already been registered with a different type \"" + oldconstructorInfo.DeclaringType.Name + "\".");
				}
			}
			else
			{
				dic_en.Value = constructorInfo;
			}
		}


		/*
				public uint typeOf(ISerializableObject serializableObject)
				{
					return ((ClassIDAttribute) serializableObject.GetType().GetCustomAttributes(typeof(ClassIDAttribute), false)[0]).ClassID;
				}

				private void save(ISerializableObject serializableObject, )
				{
					byte[] serializableClassIDAsBytes = BitConverter.GetBytes(this.typeOf(serializableObject));
					Buffer.BlockCopy(serializableClassIDAsBytes, 0, blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) blockOfData.SizeOfData);
					blockOfData.consume(sizeOfUInt32);
					objectAsSerializable.serializeTo(blockOfData.Buffer, blockOfData.OffsetWithinBuffer, blockOfData.SizeOfData);
				}
		*/

		#region ISerializer Members

		/*
				public void serialize(ISerializableObject serializableObject, IBlockOfData blockOfData)
				{
					ISerializable objectAsSerializable = serializableObject.AsSerializable;

					Debug.Assert(blockOfData.SizeOfData >= objectAsSerializable.Size + sizeOfUInt32);

				}
		*/
		public Base2.IBase2Serializable CreateObject(ushort classID)
		{
            return (Base2.IBase2Serializable)CreateObjectWith(classID, classID2ConstructorMappings);
        }

        public Base2.IBase2Serializable CreateObject(QS.ClassID classID)
        {
            return (Base2.IBase2Serializable)CreateObjectWith((ushort) classID, classID2ConstructorMappings);
        }

        public static object CreateObjectWith(ushort classID, Collections.IDictionary classID2ConstructorMappings)
        {
            try
            {
                System.Reflection.ConstructorInfo constructorInfo = (System.Reflection.ConstructorInfo)classID2ConstructorMappings[classID];
                return constructorInfo.Invoke(emptyObjects);
            }
            catch (Exception exc)
            {
                throw new Exception("Cannot create object of type " + classID.ToString(), exc);
            }
        }

        /*
				public ISerializableObject deserialize(IBlockOfData blockOfData)
				{
					Debug.Assert(blockOfData.SizeOfData > sizeOfUInt32);

					uint serializableClassID = BitConverter.ToUInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
					blockOfData.consume(sizeOfUInt32);

					ISerializableObject serializableObject = this.createObject(serializableClassID);
					serializableObject.initializeWith(blockOfData);
					return serializableObject;
				}

				public IBlockOfData serialize(ISerializableObject serializableObject)
				{
					ISerializable objectAsSerializable = serializableObject.AsSerializable;

					IBlockOfData blockOfData = new BlockOfData(new 



				}
		*/
		#endregion

		public static void saveInt32(int num, Base2.IBlockOfData blockOfData)
		{
			Buffer.BlockCopy(BitConverter.GetBytes(num), 0, blockOfData.Buffer, 
				(int) blockOfData.OffsetWithinBuffer, (int) Base2.SizeOf.Int32);
			blockOfData.consume(Base2.SizeOf.Int32);
		}

		public static int loadInt32(Base2.IBlockOfData blockOfData)
		{
			int num = BitConverter.ToInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
			blockOfData.consume(Base2.SizeOf.Int32);
			return num;
		}

		public static void saveUInt32(uint num, Base2.IBlockOfData blockOfData)
		{
			Buffer.BlockCopy(BitConverter.GetBytes(num), 0, blockOfData.Buffer, 
				(int) blockOfData.OffsetWithinBuffer, (int) Base2.SizeOf.UInt32);
			blockOfData.consume(Base2.SizeOf.UInt32);
		}

		public static uint loadUInt32(Base2.IBlockOfData blockOfData)
		{
			uint num = BitConverter.ToUInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
			blockOfData.consume(Base2.SizeOf.UInt32);
			return num;
		}

		public static void saveUInt16(ushort num, Base2.IBlockOfData blockOfData)
		{
			Buffer.BlockCopy(BitConverter.GetBytes(num), 0, blockOfData.Buffer, 
				(int) blockOfData.OffsetWithinBuffer, (int) Base2.SizeOf.UInt16);
			blockOfData.consume(Base2.SizeOf.UInt16);
		}

		public static ushort loadUInt16(Base2.IBlockOfData blockOfData)
		{
			ushort num = BitConverter.ToUInt16(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
			blockOfData.consume(Base2.SizeOf.UInt16);
			return num;
		}

		public static void saveBool(bool num, Base2.IBlockOfData blockOfData)
		{
			Buffer.BlockCopy(BitConverter.GetBytes(num), 0, blockOfData.Buffer, 
				(int) blockOfData.OffsetWithinBuffer, (int) Base2.SizeOf.Bool);
			blockOfData.consume(Base2.SizeOf.Bool);
		}

		public static bool loadBool(Base2.IBlockOfData blockOfData)
		{
			bool num = BitConverter.ToBoolean(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
			blockOfData.consume(Base2.SizeOf.Bool);
			return num;
		}

		public static void saveByte(byte num, Base2.IBlockOfData blockOfData)
		{
			blockOfData.Buffer[blockOfData.OffsetWithinBuffer] = num;
			blockOfData.consume(1);
		}

		public static byte loadByte(Base2.IBlockOfData blockOfData)
		{
			byte num = blockOfData.Buffer[blockOfData.OffsetWithinBuffer];
			blockOfData.consume(1);
			return num;
		}

		public static void saveIPAddress(System.Net.IPAddress address, Base2.IBlockOfData blockOfData)
		{
			byte[] buffer = address.GetAddressBytes();
			
			Debug.Assert(buffer.Length == Base2.SizeOf.IPAddress, "GetAddressBytes returned " + 
				buffer.Length.ToString() + " bytes instead of " + Base2.SizeOf.IPAddress.ToString() + ".");

			Buffer.BlockCopy(buffer, 0, blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, buffer.Length);
			blockOfData.consume((uint) buffer.Length);
		}

		public static System.Net.IPAddress loadIPAddress(Base2.IBlockOfData blockOfData)
		{
			IPAddress address = IPAddress.Parse(
				blockOfData.Buffer[blockOfData.OffsetWithinBuffer + 0] + "." + 
				blockOfData.Buffer[blockOfData.OffsetWithinBuffer + 1] + "." + 
				blockOfData.Buffer[blockOfData.OffsetWithinBuffer + 2] + "." + 
				blockOfData.Buffer[blockOfData.OffsetWithinBuffer + 3]);

			/* 
						byte[] addressBytes = new byte[Base2.SizeOf.IPAddress];
						Buffer.BlockCopy(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, 
							addressBytes, 0, (int) Base2.SizeOf.IPAddress);
						IPAddress address = new System.Net.IPAddress(addressBytes);
			*/			

			blockOfData.consume(Base2.SizeOf.IPAddress);
			return address;
		}
	}
}
