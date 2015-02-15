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
#include "AsyncRequest.h"

HRESULT CAsyncRequest::Request(CAsyncIo *pIo, CAsyncStream *pStream, LONGLONG llPos,
    LONG lLength, BOOL bAligned, BYTE* pBuffer, LPVOID pContext, DWORD_PTR dwUser)
{
    m_pIo = pIo;
    m_pStream = pStream;
    m_llPos = llPos;
    m_lLength = lLength;
    m_bAligned = bAligned;
    m_pBuffer = pBuffer;
    m_pContext = pContext;
    m_dwUser = dwUser;
    m_hr = VFW_E_TIMEOUT;

    return S_OK;
}

HRESULT CAsyncRequest::Complete()
{
    m_pStream->Lock();

    m_hr = m_pStream->SetPointer(m_llPos);
    if (S_OK == m_hr)
    {
        DWORD dwActual;

        m_hr = m_pStream->Read(m_pBuffer, m_lLength, m_bAligned, &dwActual);
        if (m_hr == OLE_S_FIRST)
        {
            if (m_pContext)
            {
                IMediaSample *pSample = reinterpret_cast<IMediaSample *>(m_pContext);
                pSample->SetDiscontinuity(TRUE);
                m_hr = S_OK;
            }
        }

        if (FAILED(m_hr))
        {
        }
        else if (dwActual != (DWORD) m_lLength)
        {
            m_lLength = (LONG) dwActual;
            m_hr = S_FALSE;
        }
        else
        {
            m_hr = S_OK;
        }
    }

    m_pStream->Unlock();
    return m_hr;
}

HRESULT CAsyncRequest::Cancel()
{
    return S_OK;
};

LPVOID CAsyncRequest::GetContext()
{
    return m_pContext;
};

DWORD_PTR CAsyncRequest::GetUser()
{
    return m_dwUser;
};

HRESULT CAsyncRequest::GetHResult() 
{
    return m_hr;
};

LONG CAsyncRequest::GetActualLength() 
{
    return m_lLength;
};

LONGLONG CAsyncRequest::GetStart() 
{
    return m_llPos;
};
