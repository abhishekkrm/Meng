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
set patch=C:\Cygwin\bin\patch.exe
set babel=%VSSDK90Install%VisualStudioIntegration\Common\Source\CSharp\Babel
set here=.
"%patch%" -o "%here%\IScanner.cs" "%babel%\IScanner.cs" "%here%\IScanner.patch"
"%patch%" -o "%here%\ParserStack.cs" "%babel%\ParserStack.cs" "%here%\ParserStack.patch"
"%patch%" -o "%here%\Rule.cs" "%babel%\Rule.cs" "%here%\Rule.patch"
"%patch%" -o "%here%\ShiftReduceParser.cs" "%babel%\ShiftReduceParser.cs" "%here%\ShiftReduceParser.patch"
"%patch%" -o "%here%\State.cs" "%babel%\State.cs" "%here%\State.patch"
