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

#pragma once

#include "AsyncStream.h"
#include "AsyncRequest.h"

class CAsyncIo
{
public:

	CAsyncIo(CAsyncStream *pStream);
	~CAsyncIo();

	HRESULT Open(LPCTSTR pName);
    HRESULT AsyncActive();
    HRESULT AsyncInactive();
    HRESULT Request(LONGLONG llPos, LONG lLength, BOOL bAligned, BYTE* pBuffer, LPVOID pContext, DWORD_PTR dwUser);
    HRESULT WaitForNext(DWORD dwTimeout, LPVOID *ppContext, DWORD_PTR * pdwUser, LONG * pcbActual);
    HRESULT SyncReadAligned(LONGLONG llPos, LONG lLength, BYTE* pBuffer, LONG* pcbActual, PVOID pvContext);
    HRESULT SyncRead(LONGLONG llPos, LONG lLength, BYTE* pBuffer);
    HRESULT Length(LONGLONG *pllTotal, LONGLONG* pllAvailable);
    HRESULT Alignment(LONG* pl);
    HRESULT BeginFlush();
    HRESULT EndFlush();

    LONG Alignment();
    BOOL IsAligned(LONG l);
    BOOL IsAligned(LONGLONG ll);
    HANDLE StopEvent() const;

private:

    CCritSec m_csReader;
    CAsyncStream *m_pStream;
    CCritSec m_csLists;
    BOOL m_bFlushing;
    CRequestList m_listWork;
    CRequestList m_listDone;
    CAMEvent m_evWork;
    CAMEvent m_evDone;
    LONG m_cItemsOut;
    BOOL m_bWaiting;
    CAMEvent m_evAllDone;
    CAMEvent m_evStop;
    HANDLE m_hThread;

    LONGLONG Size();
    HRESULT StartThread();
    HRESULT CloseThread();
    CAsyncRequest* GetWorkItem();
    CAsyncRequest* GetDoneItem();
    HRESULT PutWorkItem(CAsyncRequest* pRequest);
    HRESULT PutDoneItem(CAsyncRequest* pRequest);
    void ProcessRequests();
    static DWORD WINAPI InitialThreadProc(LPVOID pv);
    DWORD ThreadProc();
};
