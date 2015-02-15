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

/*
using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;


/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://niaong.cluskong.cs.cornell.edu/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class RepositoryService : System.Web.Services.WebService
{

    public RepositoryService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 

        repository = new QS.TMS.Repository.Repository("C:\\.QuickSilver\\.Repository");
    }

    private QS.TMS.Repository.IRepository repository;

    [WebMethod]
    public byte[] Get(string link)
    {
        return new QS.CMS.Base3.RegularSO(((QS.TMS.Repository.ScalarAttribute)repository.AttributeOf(link)).Object).BytesOf();
    }

/-*
    [WebMethod]
    public string Add(string[] path, QS.CMS.Base3.RegularSO dataObject)
    {
        return ((QS.TMS.Repository.IAttribute)QS.TMS.Repository.Repository.Save(repository, path, dataObject.EncapsulatedObject)).Ref;
    }
*-/ 
}
*/
