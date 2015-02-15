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
#include "PacketStream.h"

CPacketStream::CPacketStream(LONGLONG llLength) // , DWORD videoid, PacketStreamReadCallback packetStreamReadCallback) 
:	m_llFrom(0),
	m_llTo(0),
	m_llLength(llLength),
	m_llPosition(0),
	m_pHead(NULL),
	m_pTail(NULL)
{
	// m_videoid = videoid;
	// m_packetStreamReadCallback = packetStreamReadCallback;
}

CPacketStream::~CPacketStream() 
{
}

HRESULT CPacketStream::SetPointer(LONGLONG llPos)
{
//	CAutoLock lck(&m_csLock);

	if (llPos < m_llFrom || llPos >= m_llTo) 
        return S_FALSE;
	else 
	{
        m_llPosition = llPos;
        return S_OK;
    }
}

HRESULT CPacketStream::Read(PBYTE pbBuffer, DWORD dwBytesToRead, BOOL bAlign, LPDWORD pdwBytesRead)
{
	HRESULT hr = S_OK;
	DWORD _done = 0;	
	if (m_llPosition >= m_llFrom)
	{
		DWORD _todo = (m_llPosition + dwBytesToRead > m_llLength) ? ((DWORD)(m_llLength - m_llPosition)) : dwBytesToRead;
		DWORD _bufferoffset = 0;
		while ((_todo > 0) && (m_pHead != NULL))
		{
			LONGLONG _headposition = m_pHead->GetPosition();
			if (m_llPosition >= _headposition)
			{
				DWORD _headoffset = m_llPosition - _headposition;
				DWORD _headlength = m_pHead->GetLength();
				if (_headoffset >= _headlength)
				{
					CPacket *phead = m_pHead;
					m_pHead = phead->GetNext();
					if (!m_pHead)
						m_pTail = NULL;
					m_llFrom = phead->GetPosition() + phead->GetLength();
					delete phead;
				}
				else
				{					
					DWORD _n = _headlength - _headoffset;
					if (_todo < _n)
						_n = _todo;
					CopyMemory((PVOID) (pbBuffer + _bufferoffset), (PVOID)((m_pHead->GetBuffer()) + _headoffset), _n);
					m_llPosition += _n;
					_bufferoffset += _n;
					_done += _n;
					_todo -= _n;
				}
			}
			else
				break;
		}
		if (_todo > 0)
			hr = E_FAIL;
	}
	else
		hr = E_FAIL;
	*pdwBytesRead = _done;
	return hr;
}

LONGLONG CPacketStream::Size(LONGLONG *pSizeAvailable)
{
	*pSizeAvailable = m_llTo;
    return m_llLength;
}

DWORD CPacketStream::Alignment()
{
    return 1;
}

void CPacketStream::Lock()
{
    m_csLock.Lock();
}

void CPacketStream::Unlock()
{
    m_csLock.Unlock();
}

// void CPacketStream::_SetFrom(LONGLONG llFrom)
// {
//	m_llFrom = llFrom;
// }
//
// void CPacketStream::_SetTo(LONGLONG llTo)
// {
//	m_llTo = llTo;
// }
//
// void CPacketStream::_SetLength(LONGLONG llLength)
// {
//	m_llLength = llLength;
// }

void CPacketStream::Append(PBYTE pbBuffer, DWORD dwLength)
{
    CAutoLock lck(&m_csLock);

	CPacket *pPacket = new CPacket(m_llTo, pbBuffer, dwLength);

	if (m_pTail)
		m_pTail->SetNext(pPacket);
	else
		m_pHead = pPacket;
	m_pTail = pPacket;
	m_llTo = m_llTo + dwLength;
}

LONGLONG CPacketStream::_GetPosition() 
{ 
	return m_llPosition; 
}

LONGLONG CPacketStream::_GetFrom() 
{ 
	return m_llFrom; 
}

LONGLONG CPacketStream::_GetTo() 
{ 
	return m_llTo; 
}

LONGLONG CPacketStream::_GetLength() 
{ 
	return m_llLength; 
}
