rem | Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.
rem | 
rem | Redistribution and use in source and binary forms,
rem | with or without modification, are permitted provided that the following conditions
rem | are met:
rem | 
rem | 1. Redistributions of source code must retain the above copyright
rem |    notice, this list of conditions and the following disclaimer.
rem | 
rem | 2. Redistributions in binary form must reproduce the above
rem |    copyright notice, this list of conditions and the following
rem |    disclaimer in the documentation and/or other materials provided
rem |    with the distribution.
rem | 
rem | THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S)
rem | AND ALL OTHER CONTRIBUTORS AND ANY EXPRESS OR IMPLIED WARRANTIES,
rem | INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
rem | MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
rem | IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDER(S) OR ANY OTHER
rem | CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
rem | SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
rem | LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
rem | USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
rem | ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
rem | OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
rem | OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
rem | SUCH DAMAGE.
rem | 
@echo on
xcopy /y /r %2liveobjects_1.dll C:\liveobjects\bin\
xcopy /y /r %2liveobjects_1.pdb C:\liveobjects\bin\
xcopy /y /r %2liveobjects_2.dll C:\liveobjects\bin\
xcopy /y /r %2liveobjects_2.pdb C:\liveobjects\bin\
xcopy /y /r %2liveobjects_3.dll C:\liveobjects\bin\
xcopy /y /r %2liveobjects_3.pdb C:\liveobjects\bin\
xcopy /y /r %2liveobjects_4.dll C:\liveobjects\bin\
xcopy /y /r %2liveobjects_4.pdb C:\liveobjects\bin\
xcopy /y /r %2liveobjects_5.dll C:\liveobjects\bin\
xcopy /y /r %2liveobjects_5.pdb C:\liveobjects\bin\
xcopy /y /r %2liveobjects_6.dll C:\liveobjects\bin\
xcopy /y /r %2liveobjects_6.pdb C:\liveobjects\bin\
xcopy /y /r %2liveobjects_7.dll C:\liveobjects\bin\
xcopy /y /r %2liveobjects_7.pdb C:\liveobjects\bin\
xcopy /y /r %2liveobjects_8.dll C:\liveobjects\bin\
xcopy /y /r %2liveobjects_8.pdb C:\liveobjects\bin\
xcopy /y /r %2liveobjects_9.dll C:\liveobjects\bin\
xcopy /y /r %2liveobjects_9.pdb C:\liveobjects\bin\
xcopy /y /r %2liveobjects.exe C:\liveobjects\bin\
xcopy /y /r %2liveobjects.pdb C:\liveobjects\bin\
rem - C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe C:\liveobjects\bin\liveobjects_1.dll /tlb /codebase
rem - C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe C:\liveobjects\bin\liveobjects_4.dll /tlb /codebase
rem - C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe C:\liveobjects\bin\liveobjects_5.dll /tlb /codebase
rem - C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe C:\liveobjects\bin\liveobjects_6.dll /tlb /codebase
rem - C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe C:\liveobjects\bin\liveobjects_7.dll /tlb /codebase
rem - C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe C:\liveobjects\bin\liveobjects_8.dll /tlb /codebase
rem - C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe C:\liveobjects\bin\liveobjects_9.dll /tlb /codebase
xcopy /y /r %1liveobjects.reg C:\liveobjects\bin\
if not exist C:\liveobjects\examples mkdir C:\liveobjects\examples
xcopy /y /r %1objects\*.liveobject C:\liveobjects\examples\
if not exist C:\liveobjects\scenarios mkdir C:\liveobjects\scenarios
xcopy /y /r %1scenarios\*.xml C:\liveobjects\scenarios\
if not exist C:\liveobjects\dataflows mkdir C:\liveobjects\dataflows
xcopy /y /r %1dataflows\*.ruleset C:\liveobjects\dataflows\