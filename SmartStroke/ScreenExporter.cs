﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Input.Inking;
using Windows.UI.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.Devices.Input;
using Windows.UI.ApplicationSettings;
using System.Diagnostics;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using System.Runtime.InteropServices.WindowsRuntime;

namespace SmartStroke
{
    public class ScreenExporter
    {
        // This is the canvas that will actually be saved.
        Canvas canvas;

        // Function that gets called to start the saving process.
        // The Canvas passed in is the part of the screen that is saved.
        async public Task<ScreenExporter> StartAsync(Canvas c)
        {
            canvas = c;
            await SaveScreenshotAsync(c);
            return this;
        }

        // Does two things: Creates a file picker (which is a new page). Then saves to the file
        // chosen from the filepicker.
        async Task<RenderTargetBitmap> SaveScreenshotAsync(FrameworkElement uielement)
        {
            var file = await PickSaveImageAsync();

            return await SaveToFileAsync(uielement, file);
        }

        // Creates the filepicker and allows dropdown options for .bmp, .jpg, .gif, .png, etc.
        // Returns the choice and the filepicker saving.
        async Task<StorageFile> PickSaveImageAsync()
        {
            var filePicker = new FileSavePicker();
            filePicker.FileTypeChoices.Add("Bitmap", new List<string>() { ".bmp" });
            filePicker.FileTypeChoices.Add("JPEG format", new List<string>() { ".jpg" });
            filePicker.FileTypeChoices.Add("Compuserve format", new List<string>() { ".gif" });
            filePicker.FileTypeChoices.Add("Portable Network Graphics", new List<string>() { ".png" });
            filePicker.FileTypeChoices.Add("Tagged Image File Format", new List<string>() { ".tif" });
            filePicker.DefaultFileExtension = ".jpg";
            filePicker.SuggestedFileName = "screenshot";
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.SettingsIdentifier = "picture picker";
            filePicker.CommitButtonText = "Save picture";

            return await filePicker.PickSaveFileAsync();
        }

        async Task<RenderTargetBitmap> SaveToFileAsync(FrameworkElement uielement, StorageFile file)
        {
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);

                Guid encoderId = GetBitmapEncoder(file.FileType);

                try
                {
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        return await CaptureToStreamAsync(uielement, stream, encoderId);
                    }
                }
                catch (Exception ex)
                {
                    DisplayMessage(ex.Message);
                }

                var status = await CachedFileManager.CompleteUpdatesAsync(file);
            }

            return null;
        }

        Guid GetBitmapEncoder(string fileType)
        {
            Guid encoderId = BitmapEncoder.JpegEncoderId;
            switch (fileType)
            {
                case ".bmp":
                    encoderId = BitmapEncoder.BmpEncoderId;
                    break;
                case ".gif":
                    encoderId = BitmapEncoder.GifEncoderId;
                    break;
                case ".png":
                    encoderId = BitmapEncoder.PngEncoderId;
                    break;
                case ".tif":
                    encoderId = BitmapEncoder.TiffEncoderId;
                    break;
            }

            return encoderId;
        }

        async Task<RenderTargetBitmap> CaptureToStreamAsync(FrameworkElement uielement, IRandomAccessStream stream, Guid encoderId)
        {
            try
            {
                var renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(uielement);

                var pixels = await renderTargetBitmap.GetPixelsAsync();

                var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
                var encoder = await BitmapEncoder.CreateAsync(encoderId, stream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    logicalDpi,
                    logicalDpi,
                    pixels.ToArray());

                await encoder.FlushAsync();

                return renderTargetBitmap;
            }
            catch (Exception ex)
            {
                DisplayMessage(ex.Message);
            }

            return null;
        }

        async void DisplayMessage(string error)
        {
            var dialog = new MessageDialog(error);

            await dialog.ShowAsync();
        }
    }
    
}
