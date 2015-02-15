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

namespace QS.Fx.Reflection
{
    public static class InterfaceClasses
    {
        #region Constants

        public const string Aggregator = "A3D29E8514B643659C09F2C121D44BB7";
        public const string AggregatorClient = "7DB93073FAFB412F94FC0B821C7B5E7D";
        public const string AggUpdater = "AC3D58C597EA46adAB1082116F0E3B98";
        public const string AggUpdaterClient = "366A648AE9384b0f96F3E6A581E7AFFD";
        public const string Authenticating = "DB3EBAAF2915455F9092725738D11737";
        public const string AuthenticatingClient = "742FD5C060B6418DB5B7FF1B7F2AF34D";
        public const string Channel = "38ED402266E4488E8F29334D9D70368A";
        public const string ChannelClient = "27A09420C5644EA09F7BAED58511371A";
        public const string CheckpointedCommunicationChannel = "B9A2529373DA42F899617A19AC542AB3";
        public const string CheckpointedCommunicationChannelClient = "80A2A7BD6D3F46DEBBCC4B6405951C54";
        public const string CheckpointingChannel = "247889E36E6E4B59A0390FC8A25424A4";
        public const string CheckpointingChannelClient = "B695F74118984E02AEFF42D188681280";
        public const string CommitChannel = "FE8137908A974AB09CB193C32E1A5F2F";
        public const string CommitChannelClient = "618EABC67FE747679325CB6B42769BAB";
        public const string CommunicationChannel = "E861B6B2741842C8A6375B990EECB4F4";
        public const string CommunicationChannel_1_ = "1E275C2A1EF045CB8AEEF062FB46F289";
        public const string CommunicationChannelClient_1_ = "90CCA3B9282B44D38C232FCE10DE0E98";
        public const string Controller = "1FD7D58F7C5C48CF93767CE237CD015E";
        public const string ControllerClient = "11BDFDB0737843C9BC89377101718C61";
        public const string DataFlow = "AFA3A01D481645a5A76B4311357C91C0";
        public const string DataFlowClient = "8C07BE01E93E42e7B568C75F167DA3AA";
        public const string DelegationChannel = "C26BE357945C419CBF8E763CBD06C7D7";
        public const string DelegationChannelClient = "9ECFF217FF7D4F1D992C494292A46310"; 
        public const string Deserializer = "91BDCF33307C4F498F28E4C976D686F0";
        public const string Dictionary = "FBB2E5890D0E48CF86A9EAAB234A608E";
        public const string DictionaryClient = "BB6DB6E2E205433E9588E4DB1F7F067A";
        public const string Discovery = "C8A81FECC9614044A502D1E6DC65BEF5";
        public const string Factory = "0FE28DC88DA84D05B9FEF9C079CA8FDC";
        public const string Factory2 = "8749A6D5747943C0931E3861B6D33944";
        public const string File_1 = "5F8E5923907244ECAA857ECDA04E1F4B";
        public const string FileClient_1 = "78130C048AAD41B087D0AE7BBA7C2DF2";
        public const string Group = "1B6E98ED19144AF29EF6BFEF32F1929F";
        public const string GroupClient = "E485CF70D8C94A89864A3D35851B3EBE";
        public const string Infrastructure = "DDDF18C1E28E482C90B2A012E5E2CF6F";
        public const string InfrastructureClient = "83BE9F4D3BD54528889C18F1338111E0";
        public const string Interface = "2E4BFFC602B64F4BBDD226942884B24D";
        public const string Library = "2D14726DD3B8419F954FD1101DA16FD3";
        public const string Library2 = "F169D4594A9143CD8792045F620A7D14";
        public const string LibraryClient = "3CAB083F643F47D1885312E33BD99F50";
        public const string Loader = "E31A038ECF1C40149DC485CF25B3BDC6";
        public const string Log = "9DA35FB1AC4E4C43AD99162660383A0A";
        public const string Membership = "91C7AB83070541EB8879AFCF7367911E";
        public const string MembershipClient = "A82D10C53B204FE1BA3158F7F4A3B984";
        public const string MembershipChannel = "A640312755074B42BE6EB6BB141F0056";
        public const string MembershipChannelClient = "F387922A940340298F8E0FDF1BDAD69B";
        public const string Order = "CD34295CE19C42cc8BF5D97579F81328";
        public const string OrderClient = "EF8B75DCA0484f24B650FE8A79B71447";
        public const string Properties = "926F1A0AD14D4E3DB316AACCF408960A";
      public const string Metadata = "62FFAC3FFA054113A165C2D0A15800DB";
      public const string MetadataClient = "001C9A51D6AD4f95BE26A53BF20D74B7";
      public const string Reliable = "1B27B8D6E54544c9997D2CB052D2FFD8";
      public const string ReliableClient = "2D190081A0854724A1B58F3A0C9666B2";
        public const string Ring = "67589B84023644389F01B16852B609A6";
        public const string RingClient = "011A9494F9C24768B7778055B5450591";
        public const string Serializer = "5DF338BE6A844F03A1E3D0194FB06D4F";
        public const string SynchronizationChannel = "6DD74360FEAE4BB7AC73BCE9DDC7E4CC";
        public const string SynchronizationChannelClient = "8B38A5A1617F4BDE90A96F5F7B5B52CE";
        public const string Transport = "EF8EE8160D3340B88EA576C16AD95D36";
        public const string TransportClient = "20EEACC61142459E8BC143B48CE1098A";
        public const string Transport_1_ = "A8A4D2F7CA1C473484C67AF1454DE983";
        public const string TransportClient_1_ = "1A145694790546C48E29E8944A1EA04B";
        public const string Tree = "8C3E0A69B52E4ed88DE2BFC5220F3C28";
        public const string TreeClient = "6BA64328AB684adeAFF22C1377FE3485";
        public const string Value = "8B11DD1680DB45F7A79C98742E621F82";
        public const string ValueClient = "E3DCFD24970E47E5A433FF0877824CDC";
        public const string View = "70EA6B25FA6B4F749B70CC4A3F60B04C";
        public const string Checkpointed = "58BB38C7CC1F41A386F78B2E53D94F5A";
        public const string ExplorableProperty = "2C0AA1793AC2418684B06568A74D111F";
/*
        public const string Control = "52B69BFDAA944C9E8F7750E6C3A665F2";
        public const string ControlClient = "5C5DD4DA-5C43-4b37-821E-39F35656D9FF";
        public const string Foo = "F69F978C1D2F42688C6667ECC81CFCCC";
        public const string ParameterizedObjectFactory = "EC546E04-F3F7-4ee2-BBC2-7084B81983EA";
        public const string Serializable = "AE334B6F-29A4-4BFE-A1FE-6D67AB8A3A30";
*/

        public const string InterfaceClass_01 = "44DC6C37286A4051B34CCAE4514629D0";
        public const string InterfaceClass_02 = "3C236A8D9B774186B242417DD1B6DEA8";
        public const string InterfaceClass_03 = "E55F24920A5B4DD4921538F8CC74AB0F";
        public const string InterfaceClass_04 = "06797C4722484CAA8932F92D00541C3D";
        public const string InterfaceClass_05 = "80D743E317CE4B228527981E81A40BD4";
        public const string InterfaceClass_06 = "6F257154E8724D3EBAB3FBAE61680EE1";

        public const string Ping = "BCA4C4AB819B4B11AC8880999CC9502B";
        public const string Pong = "5E1228C2E7E14F028AB165AE038FFF2C";

        #endregion
    }
}
