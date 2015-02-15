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
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace QS._qss_d_.ClusterAdministrator_
{
	[Serializable]
	public class Configuration
	{
		public Configuration()
		{
		}

		public QS._qss_c_.Base1_.Subnet Subnet;

		public static Configuration Load(string path)
		{
			Configuration configuration;
			try
			{
				using (TextReader reader = new StreamReader(path, Encoding.Unicode))
				{
					configuration = (Configuration)(new XmlSerializer(typeof(Configuration))).Deserialize(reader);
				}
			}
			catch (Exception)
			{
				configuration = new Configuration();

				configuration.Subnet = new QS._qss_c_.Base1_.Subnet("x.x.x.x");

				configuration.Save(path);
			}
			return configuration;
		}

		public void Save(string path)
		{
			using (TextWriter writer = new StreamWriter(path, false, Encoding.Unicode))
			{
				(new XmlSerializer(typeof(Configuration))).Serialize(writer, this);
			}
		}
	}
}
