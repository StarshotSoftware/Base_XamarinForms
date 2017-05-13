using System;
using Xamarin.Forms.Platform.iOS;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using System.Threading;

namespace Base1902
{
	public sealed class ImageLoaderSourceHandler : IImageSourceHandler
	{
		private static readonly Dictionary<string, TaskCompletionSource<object>> CurrentCalls = new Dictionary<string, TaskCompletionSource<object>>();

		private static readonly object LockObj = new object();

		public async Task<UIImage> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = new CancellationToken(), float scale = 1)
		{
			var uriImageSource = imagesource as UriImageSource;

			if (uriImageSource != null && uriImageSource.Uri != null)
			{
				var key = uriImageSource.Uri.ToString();
				TaskCompletionSource<object> existingTask = null;

				lock (LockObj)
				{
					if (CurrentCalls.ContainsKey(key))
						existingTask = CurrentCalls[key];
				}

				if (existingTask != null)
					await existingTask.Task;

				var task = new TaskCompletionSource<object>();

				lock (LockObj)
					CurrentCalls.Add(key, task);

				var originalImageLoaderSourceHandler = new Xamarin.Forms.Platform.iOS.ImageLoaderSourceHandler();
				var uiImage = await originalImageLoaderSourceHandler.LoadImageAsync(imagesource, cancelationToken, scale);

				lock (LockObj)
					CurrentCalls.Remove(key);

				task.SetResult(null);

				return uiImage;
			}

			return null;
		}
	}
}

