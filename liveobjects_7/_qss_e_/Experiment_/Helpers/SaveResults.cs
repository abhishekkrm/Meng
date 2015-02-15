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

namespace QS._qss_e_.Experiment_.Helpers
{
    public static class SaveResults
    {
        public static void Save(string repositoryPath, QS.Fx.Logging.ILogger logger,
            object experimentClass, QS._core_c_.Components.IAttributeSet arguments, QS._core_c_.Components.IAttributeSet results)
        {
            QS._core_e_.Repository.IRepository repository = new QS._qss_e_.Repository_.Repository(repositoryPath);
            if (repository != null)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    string elementName = "Results_" + now.ToString("yyyyMMddHHmmssfff");

                    QS._core_c_.Components.AttributeSet _attributeSet = new QS._core_c_.Components.AttributeSet();
                    _attributeSet["Time Stamp"] = now.ToString("MM/dd/yyyy HH:mm");
                    _attributeSet["Experiment Class"] = experimentClass.GetType().ToString();
                    _attributeSet["Arguments"] = arguments;
                    _attributeSet["Collected Results"] = results;
                    repository.Add(elementName, _attributeSet);

                    logger.Log(experimentClass, "Results saved as \"" + elementName + "\".");

                    // results["saved"] = "http://" + environment. elementName
                }
                catch (Exception exc)
                {
                    logger.Log(experimentClass, "Could not save results.\n" + exc.ToString());
                }
            }
            else
                logger.Log(experimentClass, "Could not access repository \"" + repositoryPath + "\".");
        }
    }
}
