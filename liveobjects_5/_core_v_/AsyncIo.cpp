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
#include "AsyncIo.h"

CAsyncIo::CAsyncIo(CAsyncStream *pStream)
:	m_hThread(NULL), 
	m_evWork(TRUE),
	m_evDone(TRUE),
	m_evStop(TRUE),
	m_listWork(NAME("Work list")),
	m_listDone(NAME("Done list")),
	m_bFlushing(FALSE),
	m_cItemsOut(0),
	m_bWaiting(FALSE),
	m_pStream(pStream)
{
}

CAsyncIo::~CAsyncIo()
{
    BeginFlush();
    CloseThread();
    POSITION pos = m_listDone.GetHeadPosition();
    while (pos)
    {
        CAsyncRequest* pRequest = m_listDone.GetNext(pos);
        delete pRequest;
    }
    m_listDone.RemoveAll();
}

HRESULT CAsyncIo::AsyncActive()
{
    return StartThread();
}

HRESULT CAsyncIo::AsyncInactive()
{
    return CloseThread();
}

HRESULT CAsyncIo::Request(
	LONGLONG llPos, LONG lLength, BOOL bAligned, BYTE * pBuffer, LPVOID pContext, DWORD_PTR dwUser)
{
    if (bAligned)
    {
        if (!IsAligned(llPos) || !IsAligned(lLength) || !IsAligned((LONG_PTR) pBuffer))
			return VFW_E_BADALIGN;
    }

    CAsyncRequest* pRequest = new CAsyncRequest;
    if (!pRequest)
        return E_OUTOFMEMORY;

    HRESULT hr = pRequest->Request(this, m_pStream, llPos, lLength, bAligned, pBuffer, pContext, dwUser);
    if (SUCCEEDED(hr))
        hr = PutWorkItem(pRequest);

    if(FAILED(hr))
        delete pRequest;

    return hr;
}


HRESULT CAsyncIo::WaitForNext(DWORD dwTimeout, LPVOID *ppContext, DWORD_PTR *pdwUser, LONG *pcbActual)
{
    CheckPointer(ppContext, E_POINTER);
    CheckPointer(pdwUser, E_POINTER);
    CheckPointer(pcbActual, E_POINTER);

    *ppContext = NULL;

    for (;;)
    {
        if (!m_evDone.Wait(dwTimeout))
            return VFW_E_TIMEOUT;

        CAsyncRequest* pRequest = GetDoneItem();
        if (pRequest)
        {
            HRESULT hr = pRequest->GetHResult();
            if (hr == S_FALSE)
            {
                if ((pRequest->GetActualLength() + pRequest->GetStart()) == Size())
                {
                    hr = S_OK;
                }
                else
                {
                    hr = E_FAIL;
                }
            }

            *pcbActual = pRequest->GetActualLength();

            *ppContext = pRequest->GetContext();
            *pdwUser = pRequest->GetUser();

            delete pRequest;
            return hr;
        }
        else
        {
            CAutoLock lck(&m_csLists);
            if (m_bFlushing && !m_bWaiting)
            {
                return VFW_E_WRONG_STATE;
            }
        }
    }
}

HRESULT CAsyncIo::SyncReadAligned(LONGLONG llPos, LONG lLength, BYTE * pBuffer, LONG * pcbActual, PVOID pvContext)
{
    CheckPointer(pcbActual,E_POINTER);

    if (!IsAligned(llPos) || !IsAligned(lLength) || !IsAligned((LONG_PTR) pBuffer))
        return VFW_E_BADALIGN;

    CAsyncRequest request;

    HRESULT hr = request.Request(this, m_pStream, llPos, lLength, TRUE, pBuffer, pvContext, 0);
    if (FAILED(hr))
        return hr;

    hr = request.Complete();

    *pcbActual = request.GetActualLength();
    return hr;
}

HRESULT CAsyncIo::SyncRead(LONGLONG llPos, LONG lLength, BYTE * pBuffer)
{
    if (IsAligned(llPos) && IsAligned(lLength) && IsAligned((LONG_PTR) pBuffer))
    {
        LONG cbUnused;
        return SyncReadAligned(llPos, lLength, pBuffer, &cbUnused, NULL);
    }

    CAsyncRequest request;

    HRESULT hr = request.Request(this, m_pStream, llPos, lLength, FALSE, pBuffer, NULL, 0);
    if (FAILED(hr))
        return hr;

    return request.Complete();
}

HRESULT CAsyncIo::Length(LONGLONG *pllTotal, LONGLONG *pllAvailable)
{
    CheckPointer(pllTotal,E_POINTER);

    *pllTotal = m_pStream->Size(pllAvailable);
    return S_OK;
}

HRESULT CAsyncIo::Alignment(LONG *pAlignment)
{
    CheckPointer(pAlignment,E_POINTER);

    *pAlignment = Alignment();
    return S_OK;
}

HRESULT CAsyncIo::BeginFlush()
{
    {
        CAutoLock lock(&m_csLists);

        m_bFlushing = TRUE;

        CAsyncRequest * preq;
        while((preq = GetWorkItem()) != 0)
        {
            preq->Cancel();
            PutDoneItem(preq);
        }

        if(m_cItemsOut > 0)
        {
            ASSERT(!m_bWaiting);

            m_bWaiting = TRUE;
        }
        else
        {
            m_evDone.Set();
            return S_OK;
        }
    }

    ASSERT(m_bWaiting);

    for (;;)
    {
        m_evAllDone.Wait();
        {
            CAutoLock lock(&m_csLists);

            if (m_cItemsOut == 0)
            {
                m_bWaiting = FALSE;
                m_evDone.Set();
                return S_OK;
            }
        }
    }
}


HRESULT CAsyncIo::EndFlush()
{
    CAutoLock lock(&m_csLists);

    m_bFlushing = FALSE;

    ASSERT(!m_bWaiting);

    if(m_listDone.GetCount() > 0)
        m_evDone.Set();
    else
    {
        m_evDone.Reset();
    }

    return S_OK;
}

LONG CAsyncIo::Alignment()
{
    return m_pStream->Alignment();
};

BOOL CAsyncIo::IsAligned(LONG l) 
{
	if ((l & (Alignment() -1)) == 0) 
		return TRUE;
	else
		return FALSE;
};

BOOL CAsyncIo::IsAligned(LONGLONG ll) 
{
    return IsAligned((LONG) (ll & 0xffffffff));
};

HANDLE CAsyncIo::StopEvent() const 
{ 
	return m_evDone; 
}

LONGLONG CAsyncIo::Size() 
{
    ASSERT(m_pStream != NULL);
    return m_pStream->Size();
};

HRESULT CAsyncIo::StartThread()
{
    if (m_hThread)
    {
        return S_OK;
    }

    m_evStop.Reset();

    DWORD dwThreadID;
    m_hThread = CreateThread(NULL, 0, InitialThreadProc, this, 0, &dwThreadID);
    if (!m_hThread)
    {
        DWORD dwErr = GetLastError();
        return HRESULT_FROM_WIN32(dwErr);
    }

    return S_OK;
}

HRESULT CAsyncIo::CloseThread()
{
    m_evStop.Set();

    if (m_hThread)
    {
        WaitForSingleObject(m_hThread, INFINITE);
        CloseHandle(m_hThread);
        m_hThread = NULL;
    }

    return S_OK;
}

CAsyncRequest *CAsyncIo::GetWorkItem()
{
    CAutoLock lck(&m_csLists);
    CAsyncRequest * preq  = m_listWork.RemoveHead();

    if(m_listWork.GetCount() == 0)
    {
        m_evWork.Reset();
    }

    return preq;
}


CAsyncRequest *CAsyncIo::GetDoneItem()
{
    CAutoLock lock(&m_csLists);
    CAsyncRequest * preq  = m_listDone.RemoveHead();

    if (m_listDone.GetCount() == 0 && (!m_bFlushing || m_bWaiting))
    {
        m_evDone.Reset();
    }

    return preq;
}

HRESULT CAsyncIo::PutWorkItem(CAsyncRequest* pRequest)
{
    CAutoLock lock(&m_csLists);
    HRESULT hr;

    if (m_bFlushing)
    {
        hr = VFW_E_WRONG_STATE;
    }
    else if(m_listWork.AddTail(pRequest))
    {
        m_evWork.Set();
        hr = StartThread();
    }
    else
    {
        hr = E_OUTOFMEMORY;
    }

    return(hr);
}


HRESULT CAsyncIo::PutDoneItem(CAsyncRequest* pRequest)
{
    ASSERT(CritCheckIn(&m_csLists));

    if (m_listDone.AddTail(pRequest))
    {
        m_evDone.Set();
        return S_OK;
    }
    else
    {
        return E_OUTOFMEMORY;
    }
}

void CAsyncIo::ProcessRequests()
{
    CAsyncRequest * preq = NULL;

    for (;;)
    {
        {
            CAutoLock lock(&m_csLists);

            preq = GetWorkItem();
            if (preq == NULL)
            {
                return;
            }

            m_cItemsOut++;
        }

        preq->Complete();

        {
            CAutoLock l(&m_csLists);

            PutDoneItem(preq);

            if (--m_cItemsOut == 0)
            {
                if (m_bWaiting)
                    m_evAllDone.Set();
            }
        }
    }
}

DWORD WINAPI CAsyncIo::InitialThreadProc(LPVOID pv) 
{
    CAsyncIo * pThis = (CAsyncIo*) pv;
    return pThis->ThreadProc();
};

DWORD CAsyncIo::ThreadProc()
{
    HANDLE ahev[] = { m_evStop, m_evWork };

    for (;;)
    {
        DWORD dw = WaitForMultipleObjects(2, ahev, FALSE, INFINITE);
        if (dw == WAIT_OBJECT_0 + 1)
        {
            ProcessRequests();
        }
        else
        {
            return 0;
        }
    }
}
