using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace RALIBRARY_V2
{
    public class FileControl
    {
        public async Task<List<StorageFile>> GetDiskFileAsync(StorageFolder filefolder)
        {
            IReadOnlyList<StorageFile> itemsList = null;
            itemsList = await filefolder.GetFilesAsync();
            List<StorageFile> file = new List<StorageFile>(itemsList);
            return file;
        }

        public async Task saveImageAsync(Canvas canvas, StorageFolder folder, string nome)
        {
            StorageFile file;

            file = await folder.CreateFileAsync(nome,CreationCollisionOption.ReplaceExisting);
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(canvas);
            var pixels = await renderTargetBitmap.GetPixelsAsync();

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await
                    BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                byte[] bytes = pixels.ToArray();
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                        BitmapAlphaMode.Ignore,
                                        (uint)canvas.ActualWidth, (uint)canvas.ActualHeight,
                                        96, 96, bytes);

                await encoder.FlushAsync();
            }
        }

    }
}
