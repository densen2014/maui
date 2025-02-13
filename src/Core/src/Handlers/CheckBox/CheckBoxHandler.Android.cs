﻿using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;

namespace Microsoft.Maui.Handlers
{
	public partial class CheckBoxHandler : ViewHandler<ICheckBox, AppCompatCheckBox>
	{
		protected override AppCompatCheckBox CreatePlatformView()
		{
			var platformCheckBox = new AppCompatCheckBox(Context)
			{
				SoundEffectsEnabled = false
			};

			platformCheckBox.SetClipToOutline(true);
			return platformCheckBox;
		}

		protected override void ConnectHandler(AppCompatCheckBox platformView)
		{
			platformView.CheckedChange += OnCheckedChange;
		}

		protected override void DisconnectHandler(AppCompatCheckBox platformView)
		{
			platformView.CheckedChange -= OnCheckedChange;
		}

		// This is an Android-specific mapping
		public static void MapBackground(ICheckBoxHandler handler, ICheckBox check)
		{
			handler.PlatformView?.UpdateBackground(check);
		}

		public static void MapIsChecked(ICheckBoxHandler handler, ICheckBox check)
		{
			handler.PlatformView?.UpdateIsChecked(check);
		}

		public static void MapForeground(ICheckBoxHandler handler, ICheckBox check)
		{
			handler.PlatformView?.UpdateForeground(check);
		}

		void OnCheckedChange(object? sender, CompoundButton.CheckedChangeEventArgs e)
		{
			if (VirtualView != null)
				VirtualView.IsChecked = e.IsChecked;
		}
	}
}