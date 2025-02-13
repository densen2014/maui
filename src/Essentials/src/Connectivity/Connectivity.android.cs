using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using Android.OS;
using Microsoft.Maui.ApplicationModel;
using Debug = System.Diagnostics.Debug;

namespace Microsoft.Maui.Networking
{
	public partial class ConnectivityImplementation : IConnectivity
	{
		static ConnectivityBroadcastReceiver conectivityReceiver;
		static Intent connectivityIntent = new Intent(Platform.EssentialsConnectivityChanged);
		static EssentialsNetworkCallback networkCallback;

		public void StartListeners()
		{
			Permissions.EnsureDeclared<Permissions.NetworkState>();

			var filter = new IntentFilter();

			if (Platform.HasApiLevelN)
			{
				RegisterNetworkCallback();
				filter.AddAction(Platform.EssentialsConnectivityChanged);
			}
			else
			{
#pragma warning disable CS0618 // Type or member is obsolete
				filter.AddAction(ConnectivityManager.ConnectivityAction);
#pragma warning restore CS0618 // Type or member is obsolete
			}

			conectivityReceiver = new ConnectivityBroadcastReceiver(Connectivity.OnConnectivityChanged);

			Platform.AppContext.RegisterReceiver(conectivityReceiver, filter);
		}

		public void StopListeners()
		{
			if (conectivityReceiver == null)
				return;

			try
			{
				UnregisterNetworkCallback();
			}
			catch
			{
				Debug.WriteLine("Connectivity receiver already unregistered. Disposing of it.");
			}

			try
			{
				Platform.AppContext.UnregisterReceiver(conectivityReceiver);
			}
			catch (Java.Lang.IllegalArgumentException)
			{
				Debug.WriteLine("Connectivity receiver already unregistered. Disposing of it.");
			}
			conectivityReceiver.Dispose();
			conectivityReceiver = null;
		}
		void RegisterNetworkCallback()
		{
			if (!Platform.HasApiLevelN)
				return;

			var manager = Platform.ConnectivityManager;
			if (manager == null)
				return;

			var request = new NetworkRequest.Builder().Build();
			networkCallback = new EssentialsNetworkCallback();
			manager.RegisterNetworkCallback(request, networkCallback);
		}

		void UnregisterNetworkCallback()
		{
			if (!Platform.HasApiLevelN)
				return;

			var manager = Platform.ConnectivityManager;
			if (manager == null || networkCallback == null)
				return;

			manager.UnregisterNetworkCallback(networkCallback);

			networkCallback?.Dispose();
			networkCallback = null;
		}

		class EssentialsNetworkCallback : ConnectivityManager.NetworkCallback
		{
			public override void OnAvailable(Network network) => Platform.AppContext.SendBroadcast(connectivityIntent);

			public override void OnLost(Network network) => Platform.AppContext.SendBroadcast(connectivityIntent);

			public override void OnCapabilitiesChanged(Network network, NetworkCapabilities networkCapabilities) => Platform.AppContext.SendBroadcast(connectivityIntent);

			public override void OnUnavailable() => Platform.AppContext.SendBroadcast(connectivityIntent);

			public override void OnLinkPropertiesChanged(Network network, LinkProperties linkProperties) => Platform.AppContext.SendBroadcast(connectivityIntent);

			public override void OnLosing(Network network, int maxMsToLive) => Platform.AppContext.SendBroadcast(connectivityIntent);
		}

		static NetworkAccess IsBetterAccess(NetworkAccess currentAccess, NetworkAccess newAccess) =>
			newAccess > currentAccess ? newAccess : currentAccess;

		public NetworkAccess NetworkAccess
		{
			get
			{
				Permissions.EnsureDeclared<Permissions.NetworkState>();

				try
				{
					var currentAccess = NetworkAccess.None;
					var manager = Platform.ConnectivityManager;

#pragma warning disable CS0618 // Type or member is obsolete
					var networks = manager.GetAllNetworks();
#pragma warning restore CS0618 // Type or member is obsolete

					// some devices running 21 and 22 only use the older api.
					if (networks.Length == 0 && !OperatingSystem.IsAndroidVersionAtLeast(23))
					{
						ProcessAllNetworkInfo();
						return currentAccess;
					}

					foreach (var network in networks)
					{
						try
						{
							var capabilities = manager.GetNetworkCapabilities(network);

							if (capabilities == null)
								continue;

#pragma warning disable CS0618 // Type or member is obsolete
							var info = manager.GetNetworkInfo(network);
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
							if (info == null || !info.IsAvailable)
#pragma warning restore CS0618 // Type or member is obsolete
								continue;

							// Check to see if it has the internet capability
							if (!capabilities.HasCapability(NetCapability.Internet))
							{
								// Doesn't have internet, but local is possible
								currentAccess = IsBetterAccess(currentAccess, NetworkAccess.Local);
								continue;
							}

							ProcessNetworkInfo(info);
						}
						catch
						{
							// there is a possibility, but don't worry
						}
					}

					void ProcessAllNetworkInfo()
					{
#pragma warning disable CS0618 // Type or member is obsolete
						foreach (var info in manager.GetAllNetworkInfo())
#pragma warning restore CS0618 // Type or member is obsolete
						{
							ProcessNetworkInfo(info);
						}
					}

#pragma warning disable CS0618 // Type or member is obsolete
					void ProcessNetworkInfo(NetworkInfo info)
					{
						if (info == null || !info.IsAvailable)
							return;
						if (info.IsConnected)
							currentAccess = IsBetterAccess(currentAccess, NetworkAccess.Internet);
						else if (info.IsConnectedOrConnecting)
							currentAccess = IsBetterAccess(currentAccess, NetworkAccess.ConstrainedInternet);
#pragma warning restore CS0618 // Type or member is obsolete
					}

					return currentAccess;
				}
				catch (Exception e)
				{
					Debug.WriteLine("Unable to get connected state - do you have ACCESS_NETWORK_STATE permission? - error: {0}", e);
					return NetworkAccess.Unknown;
				}
			}
		}

		public IEnumerable<ConnectionProfile> ConnectionProfiles
		{
			get
			{
				Permissions.EnsureDeclared<Permissions.NetworkState>();

				var manager = Platform.ConnectivityManager;
#pragma warning disable CS0618 // Type or member is obsolete
				var networks = manager.GetAllNetworks();
#pragma warning restore CS0618 // Type or member is obsolete
				foreach (var network in networks)
				{
#pragma warning disable CS0618 // Type or member is obsolete
					NetworkInfo info = null;
					try
					{
						info = manager.GetNetworkInfo(network);
					}
					catch
					{
						// there is a possibility, but don't worry about it
					}
#pragma warning restore CS0618 // Type or member is obsolete

					var p = ProcessNetworkInfo(info);
					if (p.HasValue)
						yield return p.Value;
				}

#pragma warning disable CS0618 // Type or member is obsolete
				static ConnectionProfile? ProcessNetworkInfo(NetworkInfo info)
				{
					if (info == null || !info.IsAvailable || !info.IsConnectedOrConnecting)
						return null;

					return GetConnectionType(info.Type, info.TypeName);
				}
#pragma warning restore CS0618 // Type or member is obsolete
			}
		}

		internal static ConnectionProfile GetConnectionType(ConnectivityType connectivityType, string typeName)
		{
			switch (connectivityType)
			{
				case ConnectivityType.Ethernet:
					return ConnectionProfile.Ethernet;
				case ConnectivityType.Wifi:
					return ConnectionProfile.WiFi;
				case ConnectivityType.Bluetooth:
					return ConnectionProfile.Bluetooth;
				case ConnectivityType.Wimax:
				case ConnectivityType.Mobile:
				case ConnectivityType.MobileDun:
				case ConnectivityType.MobileHipri:
				case ConnectivityType.MobileMms:
					return ConnectionProfile.Cellular;
				case ConnectivityType.Dummy:
					return ConnectionProfile.Unknown;
				default:
					if (string.IsNullOrWhiteSpace(typeName))
						return ConnectionProfile.Unknown;

					if (typeName.Contains("mobile", StringComparison.OrdinalIgnoreCase))
						return ConnectionProfile.Cellular;

					if (typeName.Contains("wimax", StringComparison.OrdinalIgnoreCase))
						return ConnectionProfile.Cellular;

					if (typeName.Contains("wifi", StringComparison.OrdinalIgnoreCase))
						return ConnectionProfile.WiFi;

					if (typeName.Contains("ethernet", StringComparison.OrdinalIgnoreCase))
						return ConnectionProfile.Ethernet;

					if (typeName.Contains("bluetooth", StringComparison.OrdinalIgnoreCase))
						return ConnectionProfile.Bluetooth;

					return ConnectionProfile.Unknown;
			}
		}
	}

	[BroadcastReceiver(Enabled = true, Exported = false, Label = "Essentials Connectivity Broadcast Receiver")]
	class ConnectivityBroadcastReceiver : BroadcastReceiver
	{
		Action onChanged;

		public ConnectivityBroadcastReceiver()
		{
		}

		public ConnectivityBroadcastReceiver(Action onChanged) =>
			this.onChanged = onChanged;

		public override async void OnReceive(Context context, Intent intent)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			if (intent.Action != ConnectivityManager.ConnectivityAction && intent.Action != Platform.EssentialsConnectivityChanged)
#pragma warning restore CS0618 // Type or member is obsolete
				return;

			// await 1500ms to ensure that the the connection manager updates
			await Task.Delay(1500);
			onChanged?.Invoke();
		}
	}
}
