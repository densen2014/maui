﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.ApplicationModel;

namespace Microsoft.Maui.Accessibility
{
	public partial class SemanticScreenReaderImplementation : ISemanticScreenReader
	{
		public void Announce(string text) =>
			throw ExceptionUtils.NotSupportedOrImplementedException;
	}
}
