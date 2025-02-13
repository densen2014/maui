using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace Microsoft.Maui.Devices.Sensors
{
	/// <include file="../../docs/Microsoft.Maui.Essentials/Geocoding.xml" path="Type[@FullName='Microsoft.Maui.Essentials.Geocoding']/Docs" />
	public class GeocodingImplementation : IGeocoding
	{
		public Task<IEnumerable<Placemark>> GetPlacemarksAsync(double latitude, double longitude) =>
			throw ExceptionUtils.NotSupportedOrImplementedException;

		public Task<IEnumerable<Location>> GetLocationsAsync(string address) =>
			throw ExceptionUtils.NotSupportedOrImplementedException;
	}
}
