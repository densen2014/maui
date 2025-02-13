﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics.Drawables;

namespace Microsoft.Maui.DeviceTests.Stubs
{
	class CustomImageSourceServiceStub : IImageSourceService<ICustomImageSourceStub>
	{
		readonly CustomImageCacheStub _cache;

		public CustomImageSourceServiceStub(CustomImageCacheStub cache)
		{
			_cache = cache;
		}

		public Task<IImageSourceServiceResult> LoadDrawableAsync(IImageSource imageSource, Android.Widget.ImageView imageView, CancellationToken cancellationToken = default)
		{
			if (imageSource is not ICustomImageSourceStub imageSourceStub)
				return Task.FromResult<IImageSourceServiceResult>(new ImageSourceServiceLoadResult());

			var color = imageSourceStub.Color;

			var drawable = _cache.Get(color);

			imageView.SetImageDrawable(drawable);

			var result = new ImageSourceServiceLoadResult(() => _cache.Return(color));

			return Task.FromResult<IImageSourceServiceResult>(result);
		}

		public Task<IImageSourceServiceResult<Drawable>> GetDrawableAsync(Context context, IImageSource imageSource, CancellationToken cancellationToken = default)
		{
			if (imageSource is not ICustomImageSourceStub imageSourceStub)
				return Task.FromResult<IImageSourceServiceResult<Drawable>>(null);

			var color = imageSourceStub.Color;

			var drawable = _cache.Get(color);

			var result = new ImageSourceServiceResult(drawable, () => _cache.Return(color));

			return Task.FromResult<IImageSourceServiceResult<Drawable>>(result);
		}
	}
}