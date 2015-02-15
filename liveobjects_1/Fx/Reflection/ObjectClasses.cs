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
    public static class ObjectClasses
    {
        #region Constants

        public const string Aggregator = "8BC279B4EBE74FCA96588D37DE950BD3";
        public const string AuthenticatedObject = "AC3EEF4F190D46ED97E3030EB7D93B24";
        public const string Authenticating1 = "D98B21642A734D02A2D1322B892F4A61";
        public const string Authenticating2 = "345D35118F2B44C18F23B1AE03EBE841";
        public const string Channel = "A2057F07E2894A4490143F99C6DBBE80";
        public const string CheckpointedCommunicationChannel = "0806C71E88E749AA879E52F960F4B881";
        public const string CheckpointedCommunicationChannelClient = "1075C8B30D374668BA71EFF320F94EB2";
        public const string CheckpointedCommunicationChannel_ = "11F4FE84182B4CB3A9505E3DE69B4B77";
        public const string CheckpointingChannel = "9D9E4C00C5D544A39CBDC784C58EC41C";
        public const string CheckpointSynchronizer = "08DB518B387D4973B30657B811CC742A";
        public const string Client = "2C8F54AD9C0D4913AC2E41E57C10BE48";
        public const string CommunicationChannel = "065195BC8845404B8B6B65749E3D9952";
        public const string CommunicationChannel_0 = "4B33107948474648958E8CBB4EAD0B3F";
        public const string CommunicationChannel_1_ = "6F660F58613B4860814DEC0007AE966B";
        public const string CommunicationChannelController_1_ = "3C8FD4956D1F4EE2AA18E2E99491AEBE";
        public const string Controller = "CBD9611C5C9B4311B9574E21EBC9410C";
        public const string DataFlow = "2D714B7C0DB74f45A964FEBEED8CC543";
        public const string DataFlowClient = "64696684DDB74cdd8241970C8BE383D7";
        public const string DataFlowExposed = "BAD74C8946984017B744B3CFBC27D1C5";
        public const string DelegationChannel = "F54961B229A9418496A87A6C58FB7818";
        public const string Dictionary = "D111EEF8754D4EB98ABD90B0260AEF93";
        public const string Discovery = "BDE674E7E4494528A1074829C931211A";
        public const string Factory = "3B0732294A4141BF9519452A24C405D8";
        public const string Factory2 = "AAA94531EF1E4772A533CCEA5A109DC0";
        public const string Folder = "4D172AA9DC9D403498B080220663BBAB";
        public const string Group = "11CB43D8B31E4F7F90B89BABB86F3458";
        public const string Infrastructure = "CEF40930F3DA4E03A8894B7046D38361";
        public const string Logging = "84B4AE09D817432CA2623CDCC437A8CD";
        public const string Logging_UI = "DD0FCDFD36F8403F8CF55EB1FDA22ECB";
        public const string Logging_UI_X = "D94C680492B64920B677F9E6D08B48C9";
        public const string Map = "E6D0685D3D2F44AAAD780FE7161C2B22";
        public const string Membership = "A67F3AFD670C477CB79C684A90059821";
        public const string MembershipChannel = "F90B11BBB95C4F93813CB0442C600CDA";
        public const string Object = "F5225FA26F7C4F6D93C9E5A1A45FCF45";
        public const string Order = "66B45696F0724e0eBD19FA9CBF0A9CFD";
        public const string Properties = "FDD770B290844EB49B3D1B38566D374E";
        public const string Property = "173210ADDD94497bA6568612949F0052";
        public const string Proxy = "005714AEB8DC4195AE8E392575690A09";
        public const string Reliable = "0C1B4AEA5A09436cB184FA895E0B144A";
        public const string ReliableClient = "0D32B1BEDEB7450898DB7B87433DEE41";
        public const string Ring = "B3CFE059A72C40E09A81C7CEFC40C8FD";
        public const string Tree = "899100D842AB4d19A2FDE105824B0F1E";
        public const string SecureUI = "465DC762B5CA4C4A9FCE4536B714DB25";
        public const string Service = "FA968B52DFCF46C8BE7BC07B4068E503";
        public const string Service2 = "057D5E74FAFF4FDBBC566048749CB3BE";
        public const string UnidirectionalChannel = "BD896A195B3B466CB8401C4C947B41E6";
        public const string SynchronizationChannel = "43A78217A65E49CFB83FEFB5CFA48E5A";
        public const string Test1 = "A044B47B0A954EB98E509F69D5596AA6";
        public const string Test2 = "3ADD696F25FE4A8DA60007D5063DF47F";
        public const string Transport = "336CDD8D1B8E42C99295E5D8DD386727";
        public const string Transport_1_ = "803FF2681D704CDBA0439E5579899D65";
        public const string UI = "05211621A78F4CFBB993FA43186A5403";
        public const string Value = "A07FA453F6AB4C56A15EF412E5C28C16";
        public const string ValueClient = "AFD2411CB4884395B4A8907C9704E234";
        public const string Window = "A41B721B2BF84AE9913DA9EDB6696AED";
        public const string Window_X = "8FE149A254F04911A329EFFF028564A7";
        public const string UI_X = "0D80E49C5D5449A0889BD04146CDAA8E";
        public const string Logging_UI_Properties = "7CB3F4E309864a96BDFC7AFBB709DB9D";
        

/*
        public const string Client = "08B628C2E1BC4F58901A97D69553FF75";
        public const string Foo = "066D7A4BB1094018905C7526DD36CE84";
        public const string Control = "DA7EDA44-A026-43a4-B66C-91744EA59C2E";
        public const string Library = "E53D812C06DB427CA78EA2190781AEF8";
        public const string Loader = "34A3F3BFADFF484F9084AA9C6B8186FE";
*/

        public const string _Properties_Root_ = "C92C4BE2D8DC4FF88CB06995C382DD98";
        public const string _Properties_Client_ = "C784455B66D44655817CD977D7059BA2";
        public const string _Properties_Group_ = "FF9655693AB1411C94BBB4375D863F53";

        public const string ObjectClass_01 = "C0CA6F95A6C9482383B78015278E53DC";
        public const string ObjectClass_02 = "15BE4026CDA64179A9B7CCBE1F29F07C";
        public const string ObjectClass_03 = "6EBDF08993A948259FA7EE81C90F043C";
        public const string ObjectClass_04 = "C4A33D89C0D94D09A2D6CC31D220818C";
        public const string ObjectClass_05 = "6D4EF388B69B49F69772BB4BEC70A301";
        public const string ObjectClass_06 = "1C1633EF06D64F13B653F7E46918313A";
        public const string ObjectClass_07 = "383FDB09D4854359B3E559E8316C65DF";
        public const string ObjectClass_08 = "973F8039C2A94129B3E931B2924E47EB";

        public const string Ping = "D7F980CEF692439CA4F14F6E2672C815";
        public const string Pong = "F4FDC3EDB1894D7EA68E76326F1869C2";

        #endregion
    }
}
