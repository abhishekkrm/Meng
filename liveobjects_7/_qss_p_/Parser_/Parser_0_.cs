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
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;

namespace QS._qss_p_.Parser_
{
	public partial class Parser
	{
		public void MBWInit(ParseRequest request)
		{
			this.request = request;
			braces = new List<TextSpan[]>();
		}

		ParseRequest request;
		IList<TextSpan[]> braces;

		public IList<TextSpan[]> Braces
		{
			get { return this.braces; }
		}

		public ParseRequest Request
		{
			get { return this.request; }
		}

		public AuthoringSink Sink
		{
			get { return this.request.Sink; }
		}

		// brace matching, pairs and triples
		public void DefineMatch(int priority, params TextSpan[] locations)
		{			
			if (locations.Length == 2)
				braces.Add(new TextSpan[] { locations[0], 
					locations[1]});

			else if (locations.Length >= 3)
				braces.Add(new TextSpan[] { locations[0], 
					locations[1],
					locations[2]});
		}

		public void DefineMatch(params TextSpan[] locations)
		{
			DefineMatch(0, locations);
		}

		// hidden regions - not working?
		public void DefineRegion(TextSpan span)
		{
			Sink.AddHiddenRegion(span);
		}

		// auto hidden?
		// public void DefineHiddenRegion
		// etc. see NewHiddenRegion structure


		// error reporting
		public void ReportError(TextSpan span, string message, Severity severity)
		{
			Sink.AddError(request.FileName, message, span, severity);
		}

		#region Error Overloads (Severity)
		public void ReportError(TextSpan location, string message)
		{
			ReportError(location, message, Severity.Error);
		}

		public void ReportFatal(TextSpan location, string message)
		{
			ReportError(location, message, Severity.Fatal);
		}

		public void ReportWarning(TextSpan location, string message)
		{
			ReportError(location, message, Severity.Warning);
		}

		public void ReportHint(TextSpan location, string message)
		{
			ReportError(location, message, Severity.Hint);
		}
		#endregion

		#region TextSpan Conversion
		public TextSpan TextSpan(int startLine, int startIndex, int endIndex)
		{
			return TextSpan(startLine, startIndex, startLine, endIndex);
		}

		public TextSpan TextSpan(int startLine, int startIndex, int endLine, int endIndex)
		{
			TextSpan ts;
			ts.iStartLine = startLine - 1;
			ts.iStartIndex = startIndex;
			ts.iEndLine = endLine - 1;
			ts.iEndIndex = endIndex;
			return ts;
		}
		#endregion
	}
}
*/
