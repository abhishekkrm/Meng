﻿/*

Copyright (c) 2004-2009 Bo Peng. All rights reserved.

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
using System.IO;
using System.Text;
using System.Net;
using System.Collections.Generic;

namespace Quilt.HostDetector.NATCheck
{
    /// <summary>
    /// Implements NATChecker message. Defined in RFC 3489.
    /// </summary>
    public class CheckMessage
    {
        #region private attributes

        /// <summary>
        /// Specifies NATChecker attribute type.
        /// </summary>
        private enum AttributeType
        {
            MappedAddress = 0x0001,
            ResponseAddress = 0x0002,
            ChangeRequest = 0x0003,
            SourceAddress = 0x0004,
            ChangedAddress = 0x0005,
            Username = 0x0006,
            Password = 0x0007,
            MessageIntegrity = 0x0008,
            ErrorCode = 0x0009,
            UnknownAttribute = 0x000A,
            ReflectedFrom = 0x000B,
            XorMappedAddress = 0x8020,
            XorOnly = 0x0021,
            ServerName = 0x8022,
        }

        /// <summary>
        /// Specifies IP address family.
        /// </summary>
        private enum IPFamily
        {
            IPv4 = 0x01,
            IPv6 = 0x02,
        }

        private MessageType m_Type = MessageType.BindingRequest;
        private Guid m_pTransactionID = Guid.Empty;
        private IPEndPoint m_pMappedAddress = null;
        private IPEndPoint m_pResponseAddress = null;
        private ChangeRequest m_pChangeRequest = null;
        private IPEndPoint m_pSourceAddress = null;
        private IPEndPoint m_pChangedAddress = null;
        private string m_UserName = null;
        private string m_Password = null;
        private ErrorCode m_pErrorCode = null;
        private IPEndPoint m_pReflectedFrom = null;
        private string m_ServerName = null;

        #endregion

        #region constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CheckMessage()
        {
            m_pTransactionID = Guid.NewGuid();
        }

        #endregion

        #region public methods

        #region method Parse

        /// <summary>
        /// Parses NATChecker message from raw data packet.
        /// </summary>
        /// <param name="data">Raw NATChecker message.</param>
        public void Parse(byte[] data)
        {
            /* RFC 3489 11.1.             
                All NATChecker messages consist of a 20 byte header:

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |      NATChecker Message Type        |         Message Length        |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                                        Transaction ID
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                                                                               |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
              
               The message length is the count, in bytes, of the size of the
               message, not including the 20 byte header.
            */

            if (data.Length < 20)
            {
                throw new ArgumentException("Invalid NATChecker message value !");
            }

            int offset = 0;

            //--- message header --------------------------------------------------

            // NATChecker Message Type
            int messageType = (data[offset++] << 8 | data[offset++]);
            if (messageType == (int)MessageType.BindingErrorResponse)
            {
                m_Type = MessageType.BindingErrorResponse;
            }
            else if (messageType == (int)MessageType.BindingRequest)
            {
                m_Type = MessageType.BindingRequest;
            }
            else if (messageType == (int)MessageType.BindingResponse)
            {
                m_Type = MessageType.BindingResponse;
            }
            else if (messageType == (int)MessageType.SharedSecretErrorResponse)
            {
                m_Type = MessageType.SharedSecretErrorResponse;
            }
            else if (messageType == (int)MessageType.SharedSecretRequest)
            {
                m_Type = MessageType.SharedSecretRequest;
            }
            else if (messageType == (int)MessageType.SharedSecretResponse)
            {
                m_Type = MessageType.SharedSecretResponse;
            }
            else
            {
                throw new ArgumentException("Invalid NATChecker message type value !");
            }

            // Message Length
            int messageLength = (data[offset++] << 8 | data[offset++]);

            // Transaction ID
            byte[] guid = new byte[16];
            Array.Copy(data, offset, guid, 0, 16);
            m_pTransactionID = new Guid(guid);
            offset += 16;

            //--- Message attributes ---------------------------------------------
            while ((offset - 20) < messageLength)
            {
                ParseAttribute(data, ref offset);
            }
        }

        #endregion

        #region method ToByteData

        /// <summary>
        /// Converts this to raw NATChecker packet.
        /// </summary>
        /// <returns>Returns raw NATChecker packet.</returns>
        public byte[] ToByteData()
        {
            /* RFC 3489 11.1.             
                All NATChecker messages consist of a 20 byte header:

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |      NATChecker Message Type        |         Message Length        |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                                        Transaction ID
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                                                                               |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
             
               The message length is the count, in bytes, of the size of the
               message, not including the 20 byte header.

            */

            // We allocate 512 for header, that should be more than enough.
            byte[] msg = new byte[512];

            int offset = 0;

            //--- message header -------------------------------------

            // NATChecker Message Type (2 bytes)
            msg[offset++] = (byte)((int)this.Type >> 8);
            msg[offset++] = (byte)((int)this.Type & 0xFF);

            // Message Length (2 bytes) will be assigned at last.
            msg[offset++] = 0;
            msg[offset++] = 0;

            // Transaction ID (16 bytes)
            Array.Copy(m_pTransactionID.ToByteArray(), 0, msg, offset, 16);
            offset += 16;

            //--- Message attributes ------------------------------------

            /* RFC 3489 11.2.
                After the header are 0 or more attributes.  Each attribute is TLV
                encoded, with a 16 bit type, 16 bit length, and variable value:

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |         Type                  |            Length             |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |                             Value                             ....
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            */

            if (this.MappedAddress != null)
            {
                StoreEndPoint(AttributeType.MappedAddress, this.MappedAddress, msg, ref offset);
            }
            else if (this.ResponseAddress != null)
            {
                StoreEndPoint(AttributeType.ResponseAddress, this.ResponseAddress, msg, ref offset);
            }
            else if (this.ChangeRequest != null)
            {
                /*
                    The CHANGE-REQUEST attribute is used by the client to request that
                    the server use a different address and/or port when sending the
                    response.  The attribute is 32 bits long, although only two bits (A
                    and B) are used:

                     0                   1                   2                   3
                     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 A B 0|
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                    The meaning of the flags is:

                    A: This is the "change IP" flag.  If true, it requests the server
                       to send the Binding Response with a different IP address than the
                       one the Binding Request was received on.

                    B: This is the "change port" flag.  If true, it requests the
                       server to send the Binding Response with a different port than the
                       one the Binding Request was received on.
                */

                // Attribute header
                msg[offset++] = (int)AttributeType.ChangeRequest >> 8;
                msg[offset++] = (int)AttributeType.ChangeRequest & 0xFF;
                msg[offset++] = 0;
                msg[offset++] = 4;

                msg[offset++] = 0;
                msg[offset++] = 0;
                msg[offset++] = 0;
                msg[offset++] = (byte)(Convert.ToInt32(this.ChangeRequest.ChangeIP) << 2 | Convert.ToInt32(this.ChangeRequest.ChangePort) << 1);
            }
            else if (this.SourceAddress != null)
            {
                StoreEndPoint(AttributeType.SourceAddress, this.SourceAddress, msg, ref offset);
            }
            else if (this.ChangedAddress != null)
            {
                StoreEndPoint(AttributeType.ChangedAddress, this.ChangedAddress, msg, ref offset);
            }
            else if (this.UserName != null)
            {
                byte[] userBytes = Encoding.ASCII.GetBytes(this.UserName);

                // Attribute header
                msg[offset++] = (int)AttributeType.Username >> 8;
                msg[offset++] = (int)AttributeType.Username & 0xFF;
                msg[offset++] = (byte)(userBytes.Length >> 8);
                msg[offset++] = (byte)(userBytes.Length & 0xFF);

                Array.Copy(userBytes, 0, msg, offset, userBytes.Length);
                offset += userBytes.Length;
            }
            else if (this.Password != null)
            {
                byte[] userBytes = Encoding.ASCII.GetBytes(this.UserName);

                // Attribute header
                msg[offset++] = (int)AttributeType.Password >> 8;
                msg[offset++] = (int)AttributeType.Password & 0xFF;
                msg[offset++] = (byte)(userBytes.Length >> 8);
                msg[offset++] = (byte)(userBytes.Length & 0xFF);

                Array.Copy(userBytes, 0, msg, offset, userBytes.Length);
                offset += userBytes.Length;
            }
            else if (this.ErrorCode != null)
            {
                /* 3489 11.2.9.
                    0                   1                   2                   3
                    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |                   0                     |Class|     Number    |
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |      Reason Phrase (variable)                                ..
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                */

                byte[] reasonBytes = Encoding.ASCII.GetBytes(this.ErrorCode.ReasonText);

                // Header
                msg[offset++] = 0;
                msg[offset++] = (int)AttributeType.ErrorCode;
                msg[offset++] = 0;
                msg[offset++] = (byte)(4 + reasonBytes.Length);

                // Empty
                msg[offset++] = 0;
                msg[offset++] = 0;
                // Class
                msg[offset++] = (byte)Math.Floor((double)(this.ErrorCode.Code / 100));
                // Number
                msg[offset++] = (byte)(this.ErrorCode.Code & 0xFF);
                // ReasonPhrase
                Array.Copy(reasonBytes, msg, reasonBytes.Length);
                offset += reasonBytes.Length;
            }
            else if (this.ReflectedFrom != null)
            {
                StoreEndPoint(AttributeType.ReflectedFrom, this.ReflectedFrom, msg, ref offset);
            }

            // Update Message Length. NOTE: 20 bytes header not included.
            msg[2] = (byte)((offset - 20) >> 8);
            msg[3] = (byte)((offset - 20) & 0xFF);

            // Make reatval with actual size.
            byte[] retVal = new byte[offset];
            Array.Copy(msg, retVal, retVal.Length);

            return retVal;
        }

        #endregion

        #region method ParseAttribute

        /// <summary>
        /// Parses attribute from data.
        /// </summary>
        /// <param name="data">SIP message data.</param>
        /// <param name="offset">Offset in data.</param>
        private void ParseAttribute(byte[] data, ref int offset)
        {
            /* RFC 3489 11.2.
                Each attribute is TLV encoded, with a 16 bit type, 16 bit length, and variable value:

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |         Type                  |            Length             |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |                             Value                             ....
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+                            
            */

            // Type
            AttributeType type = (AttributeType)(data[offset++] << 8 | data[offset++]);

            // Length
            int length = (data[offset++] << 8 | data[offset++]);

            // MAPPED-ADDRESS
            if (type == AttributeType.MappedAddress)
            {
                m_pMappedAddress = ParseEndPoint(data, ref offset);
            }
            // RESPONSE-ADDRESS
            else if (type == AttributeType.ResponseAddress)
            {
                m_pResponseAddress = ParseEndPoint(data, ref offset);
            }
            // CHANGE-REQUEST
            else if (type == AttributeType.ChangeRequest)
            {
                /*
                    The CHANGE-REQUEST attribute is used by the client to request that
                    the server use a different address and/or port when sending the
                    response.  The attribute is 32 bits long, although only two bits (A
                    and B) are used:

                     0                   1                   2                   3
                     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 A B 0|
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                    The meaning of the flags is:

                    A: This is the "change IP" flag.  If true, it requests the server
                       to send the Binding Response with a different IP address than the
                       one the Binding Request was received on.

                    B: This is the "change port" flag.  If true, it requests the
                       server to send the Binding Response with a different port than the
                       one the Binding Request was received on.
                */

                // Skip 3 bytes
                offset += 3;

                m_pChangeRequest = new ChangeRequest((data[offset] & 4) != 0, (data[offset] & 2) != 0);
                offset++;
            }
            // SOURCE-ADDRESS
            else if (type == AttributeType.SourceAddress)
            {
                m_pSourceAddress = ParseEndPoint(data, ref offset);
            }
            // CHANGED-ADDRESS
            else if (type == AttributeType.ChangedAddress)
            {
                m_pChangedAddress = ParseEndPoint(data, ref offset);
            }
            // USERNAME
            else if (type == AttributeType.Username)
            {
                m_UserName = Encoding.Default.GetString(data, offset, length);
                offset += length;
            }
            // PASSWORD
            else if (type == AttributeType.Password)
            {
                m_Password = Encoding.Default.GetString(data, offset, length);
                offset += length;
            }
            // MESSAGE-INTEGRITY
            else if (type == AttributeType.MessageIntegrity)
            {
                offset += length;
            }
            // ERROR-CODE
            else if (type == AttributeType.ErrorCode)
            {
                /* 3489 11.2.9.
                    0                   1                   2                   3
                    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |                   0                     |Class|     Number    |
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |      Reason Phrase (variable)                                ..
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                */

                int errorCode = (data[offset + 2] & 0x7) * 100 + (data[offset + 3] & 0xFF);

                m_pErrorCode = new ErrorCode(errorCode, Encoding.Default.GetString(data, offset + 4, length - 4));
                offset += length;
            }
            // UNKNOWN-ATTRIBUTES
            else if (type == AttributeType.UnknownAttribute)
            {
                offset += length;
            }
            // REFLECTED-FROM
            else if (type == AttributeType.ReflectedFrom)
            {
                m_pReflectedFrom = ParseEndPoint(data, ref offset);
            }
            // XorMappedAddress
            // XorOnly
            // ServerName
            else if (type == AttributeType.ServerName)
            {
                m_ServerName = Encoding.Default.GetString(data, offset, length);
                offset += length;
            }
            // Unknown
            else
            {
                offset += length;
            }
        }

        #endregion

        #region method ParseEndPoint

        /// <summary>
        /// Pasrses IP endpoint attribute.
        /// </summary>
        /// <param name="data">NATChecker message data.</param>
        /// <param name="offset">Offset in data.</param>
        /// <returns>Returns parsed IP end point.</returns>
        private IPEndPoint ParseEndPoint(byte[] data, ref int offset)
        {
            /*
                It consists of an eight bit address family, and a sixteen bit
                port, followed by a fixed length value representing the IP address.

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |x x x x x x x x|    Family     |           Port                |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |                             Address                           |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            */

            // Skip family
            offset++;
            offset++;

            // Port
            int port = (data[offset++] << 8 | data[offset++]);

            // Address
            byte[] ip = new byte[4];
            ip[0] = data[offset++];
            ip[1] = data[offset++];
            ip[2] = data[offset++];
            ip[3] = data[offset++];

            return new IPEndPoint(new IPAddress(ip), port);
        }

        #endregion

        #region method StoreEndPoint

        /// <summary>
        /// Stores ip end point attribute to buffer.
        /// </summary>
        /// <param name="type">Attribute type.</param>
        /// <param name="endPoint">IP end point.</param>
        /// <param name="message">Buffer where to store.</param>
        /// <param name="offset">Offset in buffer.</param>
        private void StoreEndPoint(AttributeType type, IPEndPoint endPoint, byte[] message, ref int offset)
        {
            /*
                It consists of an eight bit address family, and a sixteen bit
                port, followed by a fixed length value representing the IP address.

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |x x x x x x x x|    Family     |           Port                |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |                             Address                           |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+             
            */

            // Header
            message[offset++] = (byte)((int)type >> 8);
            message[offset++] = (byte)((int)type & 0xFF);
            message[offset++] = 0;
            message[offset++] = 8;

            // Unused
            message[offset++] = 0;
            // Family
            message[offset++] = (byte)IPFamily.IPv4;
            // Port
            message[offset++] = (byte)(endPoint.Port >> 8);
            message[offset++] = (byte)(endPoint.Port & 0xFF);
            // Address
            byte[] ipBytes = endPoint.Address.GetAddressBytes();
            message[offset++] = ipBytes[0];
            message[offset++] = ipBytes[0];
            message[offset++] = ipBytes[0];
            message[offset++] = ipBytes[0];
        }

        #endregion

        #region Properties Implementation

        /// <summary>
        /// Gets NATChecker message type.
        /// </summary>
        public MessageType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        /// <summary>
        /// Gets transaction ID.
        /// </summary>
        public Guid TransactionID
        {
            get { return m_pTransactionID; }
        }

        /// <summary>
        /// Gets or sets IP end point what was actually connected to NATChecker server. Returns null if not specified.
        /// </summary>
        public IPEndPoint MappedAddress
        {
            get { return m_pMappedAddress; }

            set { m_pMappedAddress = value; }
        }

        /// <summary>
        /// Gets or sets IP end point where to NATChecker client likes to receive response.
        /// Value null means not specified.
        /// </summary>
        public IPEndPoint ResponseAddress
        {
            get { return m_pResponseAddress; }
            set { m_pResponseAddress = value; }
        }

        /// <summary>
        /// Gets or sets how and where NATChecker server must send response back to NATChecker client.
        /// Value null means not specified.
        /// </summary>
        public ChangeRequest ChangeRequest
        {
            get { return m_pChangeRequest; }
            set { m_pChangeRequest = value; }
        }

        /// <summary>
        /// Gets or sets NATChecker server IP end point what sent response to NATChecker client. Value null
        /// means not specified.
        /// </summary>
        public IPEndPoint SourceAddress
        {
            get { return m_pSourceAddress; }
            set { m_pSourceAddress = value; }
        }

        /// <summary>
        /// Gets or sets IP end point where NATChecker server will send response back to NATChecker client 
        /// if the "change IP" and "change port" flags had been set in the ChangeRequest.
        /// </summary>
        public IPEndPoint ChangedAddress
        {
            get { return m_pChangedAddress; }
            set { m_pChangedAddress = value; }
        }

        /// <summary>
        /// Gets or sets user name. Value null means not specified.
        /// </summary>          
        public string UserName
        {
            get { return m_UserName; }
            set { m_UserName = value; }
        }

        /// <summary>
        /// Gets or sets password. Value null means not specified.
        /// </summary>
        public string Password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }

        //public MessageIntegrity

        /// <summary>
        /// Gets or sets error info. Returns null if not specified.
        /// </summary>
        public ErrorCode ErrorCode
        {
            get { return m_pErrorCode; }
            set { m_pErrorCode = value; }
        }


        /// <summary>
        /// Gets or sets IP endpoint from which IP end point NATChecker server got NATChecker client request.
        /// Value null means not specified.
        /// </summary>
        public IPEndPoint ReflectedFrom
        {
            get { return m_pReflectedFrom; }
            set { m_pReflectedFrom = value; }
        }

        /// <summary>
        /// Gets or sets server name.
        /// </summary>
        public string ServerName
        {
            get { return m_ServerName; }
            set { m_ServerName = value; }
        }

        #endregion

        #endregion

    }
}
