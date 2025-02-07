﻿using System;

namespace Microsoft.Maui.Handlers
{
	public partial class BorderHandler : ViewHandler<IBorderView, ContentViewGroup>
	{
		protected override ContentViewGroup CreatePlatformView()
		{
			if (VirtualView == null)
			{
				throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a ContentViewGroup");
			}

			var viewGroup = new ContentViewGroup(Context)
			{
				CrossPlatformMeasure = VirtualView.CrossPlatformMeasure,
				CrossPlatformArrange = VirtualView.CrossPlatformArrange
			};

			// We only want to use a hardware layer for the entering view because its quite likely
			// the view will invalidate several times the Drawable (Draw).
			viewGroup.SetLayerType(Android.Views.LayerType.Hardware, null);

			return viewGroup;
		}

		public override void SetVirtualView(IView view)
		{
			base.SetVirtualView(view);
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");

			PlatformView.CrossPlatformMeasure = VirtualView.CrossPlatformMeasure;
			PlatformView.CrossPlatformArrange = VirtualView.CrossPlatformArrange;
		}

		static void UpdateContent(IBorderHandler handler)
		{
			_ = handler.PlatformView ?? throw new InvalidOperationException($"{nameof(PlatformView)} should have been set by base class.");
			_ = handler.MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");
			_ = handler.VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");

			handler.PlatformView.RemoveAllViews();

			if (handler.VirtualView.PresentedContent is IView view)
				handler.PlatformView.AddView(view.ToPlatform(handler.MauiContext));
		}

		public static void MapContent(IBorderHandler handler, IBorderView border)
		{
			UpdateContent(handler);
		}

		protected override void DisconnectHandler(ContentViewGroup platformView)
		{
			// If we're being disconnected from the xplat element, then we should no longer be managing its chidren
			platformView.RemoveAllViews();

			base.DisconnectHandler(platformView);
		}
	}
}
