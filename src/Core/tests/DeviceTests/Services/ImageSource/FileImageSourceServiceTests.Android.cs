using System;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Microsoft.Maui.DeviceTests.Stubs;
using Xunit;
using Color = Microsoft.Maui.Graphics.Color;

namespace Microsoft.Maui.DeviceTests
{
	public partial class FileImageSourceServiceTests
	{
		[Theory]
		[InlineData(typeof(FontImageSourceStub))]
		[InlineData(typeof(StreamImageSourceStub))]
		[InlineData(typeof(UriImageSourceStub))]
		public async Task ThrowsForIncorrectTypes(Type type)
		{
			var service = new FileImageSourceService();

			var imageSource = (ImageSourceStub)Activator.CreateInstance(type);

			await Assert.ThrowsAsync<InvalidCastException>(() => service.GetDrawableAsync(MauiProgram.DefaultContext, imageSource));
		}

		[Theory]
		[InlineData("red.png", "#FF0000")]
		[InlineData("green.png", "#00FF00")]
		[InlineData("black.png", "#000000")]
		public async Task GetDrawableAsyncWithResource(string filename, string colorHex)
		{
			var service = new FileImageSourceService();

			var imageSource = new FileImageSourceStub(filename);

			using var result = await service.GetDrawableAsync(MauiProgram.DefaultContext, imageSource);

			var bitmapDrawable = Assert.IsType<BitmapDrawable>(result.Value);

			var bitmap = bitmapDrawable.Bitmap;

			var expectedColor = Color.FromArgb(colorHex);
			bitmap.AssertColorAtCenter(expectedColor.ToPlatform());
		}

		[Theory]
		[InlineData("#FF0000")]
		[InlineData("#00FF00")]
		[InlineData("#000000")]
		public async Task GetDrawableAsyncWithFile(string colorHex)
		{
			var expectedColor = Color.FromArgb(colorHex);

			var service = new FileImageSourceService();

			var filename = CreateBitmapFile(100, 100, expectedColor);
			var imageSource = new FileImageSourceStub(filename);

			using var result = await service.GetDrawableAsync(MauiProgram.DefaultContext, imageSource);

			var bitmapDrawable = Assert.IsType<BitmapDrawable>(result.Value);

			var bitmap = bitmapDrawable.Bitmap;

			bitmap.AssertColorAtCenter(expectedColor.ToPlatform());
		}

		async Task<Drawable> GetDrawable(IImageSourceService service, IImageSource imageSource)
		{
			var tcsDrawable = new TaskCompletionSource<Drawable>();

			// get an image
			var result1 = await service.GetDrawableAsync(MauiProgram.DefaultContext, imageSource).ConfigureAwait(false);

			return await tcsDrawable.Task.ConfigureAwait(false);
		}
	}
}