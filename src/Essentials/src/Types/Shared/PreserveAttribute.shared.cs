using System;
using System.ComponentModel;

namespace Microsoft.Maui.ApplicationModel
{
	[AttributeUsage(AttributeTargets.All)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	sealed class PreserveAttribute : Attribute
	{
#pragma warning disable SA1401 // Fields should be private
		public bool AllMembers;
		public bool Conditional;
#pragma warning restore SA1401 // Fields should be private

		public PreserveAttribute(bool allMembers, bool conditional)
		{
			AllMembers = allMembers;
			Conditional = conditional;
		}

		public PreserveAttribute()
		{
		}
	}
}
