using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;
using System.IO;
using Windows.Media.MediaProperties;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace CameraCaptureService
{
    public sealed class CameraCaptureService
    {
        //Private Variables 
        MediaCapture _mediaCapture;
        bool _isPreviewing;
        /// <summary>
        /// Constructors
        /// </summary>
        public CameraCaptureService()
        {
            //Init();
        }
        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            
        }


        public IAsyncAction Init()
        {
            return _Init().AsAsyncAction(); 
        }
        private async Task _Init()
        {
            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync();
            _mediaCapture.Failed += MediaCapture_Failed;
        }

        public IAsyncOperation<string> Capture()
        {
            return _Capture().AsAsyncOperation();
        }
        //Should return a base64 encoded string
        private async Task<string> _Capture()
        {
            // Prepare and capture photo
            var lowLagCapture = await _mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreatePng());
            var capturedPhoto = await lowLagCapture.CaptureAsync();

            byte[] bytes = new byte[capturedPhoto.Frame.Size];
            await capturedPhoto.Frame.AsStream().ReadAsync(bytes, 0, bytes.Length);
            //var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;

            await lowLagCapture.FinishAsync();
            var response = Convert.ToBase64String(bytes);
            return response;
        }
    }
}
