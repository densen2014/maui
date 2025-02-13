using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Views;
using Java.Nio;
using Microsoft.Maui.ApplicationModel;

namespace Microsoft.Maui.Media
{
	public static partial class Screenshot
	{
		public static byte[] RenderAsJPEG(this View view, int quality = 100) => view?.RenderAsImage(Bitmap.CompressFormat.Jpeg, quality);

		public static byte[] RenderAsPNG(this View view, int quality = 100) => view?.RenderAsImage(Bitmap.CompressFormat.Png, quality);

		public static byte[] RenderAsBMP(this View view)
		{
			using (var bitmap = view.Render())
			{
				var byteBuffer = ByteBuffer.AllocateDirect(bitmap.ByteCount);
				bitmap.CopyPixelsToBuffer(byteBuffer);
				byte[] byt = new byte[bitmap.ByteCount];
				Marshal.Copy(byteBuffer.GetDirectBufferAddress(), byt, 0, bitmap.ByteCount);
				return byt;
			}
		}

		public static byte[] RenderAsImage(this View view, Bitmap.CompressFormat format, int quality = 100)
		{
			byte[] imageBytes = null;

			using (var bitmap = view.Render())
			{
				if (bitmap != null)
				{
					imageBytes = bitmap.AsImageBytes(format, quality);
					if (!bitmap.IsRecycled)
						bitmap.Recycle();
				}
			}

			return imageBytes;
		}

		public static byte[] AsImageBytes(this Bitmap bitmap, Bitmap.CompressFormat format, int quality = 100)
		{
			byte[] byteArray = null;
			using (var mem = new MemoryStream())
			{
				bitmap.Compress(format, quality, mem);
				byteArray = mem.ToArray();
			}

			return byteArray;
		}

		public static Bitmap RenderUsingCanvasDrawing(this View view)
		{
			try
			{
				if (view?.LayoutParameters == null || Bitmap.Config.Argb8888 == null)
					return null;
				var width = view.Width;
				var height = view.Height;

				var bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
				if (bitmap == null)
					return null;

				using (var canvas = new Canvas(bitmap))
					view.Draw(canvas);

				return bitmap;
			}
			catch (Exception)
			{
				return null;
			}
		}

		static Bitmap RenderUsingDrawingCache(this View view)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			try
			{
				var enabled = view.DrawingCacheEnabled;
				view.DrawingCacheEnabled = true;
				view.BuildDrawingCache();
				var cachedBitmap = view.DrawingCache;
				if (cachedBitmap == null)
					return null;
				var bitmap = Bitmap.CreateBitmap(cachedBitmap);
				view.DrawingCacheEnabled = enabled;
				return bitmap;
			}
			catch (Exception)
			{
				return null;
			}
#pragma warning restore CS0618 // Type or member is obsolete

		}

		public static Bitmap Render(this View view)
		{
			var bitmap = view.RenderUsingCanvasDrawing();

			if (bitmap == null)
				bitmap = view.RenderUsingDrawingCache();

			return bitmap;
		}
	}
}

namespace Microsoft.Maui.Media
{
	public partial class ScreenshotImplementation : IScreenshot
	{
		public bool IsCaptureSupported =>
			true;

		public Task<IScreenshotResult> CaptureAsync()
		{
			if (Platform.WindowManager?.DefaultDisplay?.Flags.HasFlag(DisplayFlags.Secure) == true)
				throw new UnauthorizedAccessException("Unable to take a screenshot of a secure window.");

			var view = Platform.GetCurrentActivity(true)?.Window?.DecorView?.RootView;
			if (view == null)
				throw new InvalidOperationException("Unable to find the main window.");

			var result = new ScreenshotResult(view.Render());

			return Task.FromResult<IScreenshotResult>(result);
		}

	}

	internal partial class ScreenshotResult
	{
		readonly Bitmap bmp;

		internal ScreenshotResult(Bitmap bmp)
			: base()
		{
			this.bmp = bmp;

			Width = bmp.Width;
			Height = bmp.Height;
		}

		internal Task<Stream> PlatformOpenReadAsync(ScreenshotFormat format)
		{
			var f = format switch
			{
				ScreenshotFormat.Jpeg => Bitmap.CompressFormat.Jpeg,
				_ => Bitmap.CompressFormat.Png,
			};

			var result = new MemoryStream(bmp.AsImageBytes(f, 100)) as Stream;
			return Task.FromResult(result);
		}
	}
}
