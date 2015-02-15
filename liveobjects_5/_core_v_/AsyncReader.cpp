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

#include "StdAfx.h"
#include "AsyncReader.h"

#pragma warning(disable:4355)

CAsyncReader::CAsyncReader(TCHAR *pName, LPUNKNOWN pUnk, CAsyncStream *pStream, HRESULT *phr)
:	CBaseFilter(pName, pUnk, &m_csFilter, CLSID_AsyncSample, NULL),
    m_OutputPin(phr, this, &m_Io, &m_csFilter),
    m_Io(pStream)
{
}

CAsyncReader::~CAsyncReader()
{
}

int CAsyncReader::GetPinCount()
{
    return 1;
}

CBasePin * CAsyncReader::GetPin(int n)
{
    if ((GetPinCount() > 0) && (n == 0))
        return &m_OutputPin;
    else
        return NULL;
}

const CMediaType *CAsyncReader::LoadType() const
{
    return &m_mt;
}

HRESULT CAsyncReader::Connect(IPin * pReceivePin, const AM_MEDIA_TYPE *pmt)
{
    return m_OutputPin.CBasePin::Connect(pReceivePin, pmt);
}
