namespace Microsoft.Maui.ApplicationModel.Communication
{
	public partial class PhoneDialerImplementation : IPhoneDialer
	{
		public bool IsSupported =>
			throw ExceptionUtils.NotSupportedOrImplementedException;

		public void Open(string number) =>
			throw ExceptionUtils.NotSupportedOrImplementedException;
	}
}
