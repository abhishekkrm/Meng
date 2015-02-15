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
#include "MemStream.h"

CMemStream::CMemStream() : m_llPosition(0)
{
}

CMemStream::CMemStream(LPBYTE pbData, LONGLONG llLength, DWORD dwKBPerSec) 
:	m_pbData(pbData),
    m_llLength(llLength),
    m_llPosition(0),
    m_dwKBPerSec(dwKBPerSec)
{
    m_dwTimeStart = timeGetTime();
}

void CMemStream::Init(LPBYTE pbData, LONGLONG llLength, DWORD dwKBPerSec)
{
    m_pbData = pbData;
    m_llLength = llLength;
    m_dwKBPerSec = dwKBPerSec;
    m_dwTimeStart = timeGetTime();
}

HRESULT CMemStream::SetPointer(LONGLONG llPos)
{
    if (llPos < 0 || llPos > m_llLength) 
	{
        return S_FALSE;
    } 
	else 
	{
        m_llPosition = llPos;
        return S_OK;
    }
}

HRESULT CMemStream::Read(PBYTE pbBuffer, DWORD dwBytesToRead, BOOL bAlign, LPDWORD pdwBytesRead)
{
    CAutoLock lck(&m_csLock);
    DWORD dwReadLength;

    DWORD dwTime = timeGetTime();

    if (m_llPosition + dwBytesToRead > m_llLength) 
        dwReadLength = (DWORD)(m_llLength - m_llPosition);
	else 
        dwReadLength = dwBytesToRead;
    
	DWORD dwTimeToArrive = ((DWORD)m_llPosition + dwReadLength) / m_dwKBPerSec;

    if (dwTime - m_dwTimeStart < dwTimeToArrive) 
        Sleep(dwTimeToArrive - dwTime + m_dwTimeStart);

    CopyMemory((PVOID)pbBuffer, (PVOID)(m_pbData + m_llPosition), dwReadLength);

    m_llPosition += dwReadLength;
    *pdwBytesRead = dwReadLength;
    return S_OK;
}

LONGLONG CMemStream::Size(LONGLONG *pSizeAvailable)
{
    LONGLONG llCurrentAvailable =
        static_cast <LONGLONG> (UInt32x32To64((timeGetTime() - m_dwTimeStart),m_dwKBPerSec));

   *pSizeAvailable =  min(m_llLength, llCurrentAvailable);
    return m_llLength;
}

DWORD CMemStream::Alignment()
{
    return 1;
}

void CMemStream::Lock()
{
    m_csLock.Lock();
}

void CMemStream::Unlock()
{
    m_csLock.Unlock();
}

LONGLONG CMemStream::_Position() 
{ 
	return m_llPosition; 
}

LONGLONG CMemStream::_Length() 
{ 
	return m_llLength; 
}
