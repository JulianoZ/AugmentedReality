using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace RALIBRARY_V2
{
    public class ImageControl
    {

        public async Task<List<Image>> GetDiskImagesAsync(StorageFolder picturesFolder)
        {
            List<Image> imagens = new List<Image>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
            CoreDispatcherPriority.Normal, async () => {
                IReadOnlyList<IStorageItem> itemsList = await picturesFolder.GetFilesAsync();
                foreach (var item in itemsList)
                {
                    await picturesFolder.TryGetItemAsync(item.Name);
                    if (item.Path.Contains(".jpg") || item.Path.Contains(".png"))
                    {
                        var uri = new System.Uri(item.Path);
                        var converted = uri.AbsoluteUri;
                        StorageFile file = await picturesFolder.GetFileAsync(item.Name);
                        BitmapImage bitmapImage = new BitmapImage();
                        FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);
                        bitmapImage.SetSource(stream);
                        Image image = new Image();
                        image.Name = item.Name;
                        image.Source = bitmapImage;
                        imagens.Add(image);
                    }
                }
            });
            return imagens;
        }

        public async Task AddImgtoCanvasAsync(Canvas canvas, Image image_)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
            CoreDispatcherPriority.Normal, () =>
            {
                canvas.Children.Clear();
                var bounds = Window.Current.Bounds;
                double height = bounds.Height;
                double width = bounds.Width;

                Image image = new Image()
                {
                    Height = height,
                    Width = width,
                    Source = image_.Source
                };
                Canvas.SetLeft(image, 0);
                Canvas.SetTop(image, 0);
                canvas.Children.Add(image);
            });
        }

        public async Task<List<Image>> GetImageWebAsync(string url, List<string> path)
        {
            List<Image> imagens = new List<Image>();
            try
            {
                try
                {
                    foreach (string art in path)
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal, async () =>
                            {
                                System.Net.HttpStatusCode result = default(System.Net.HttpStatusCode);
                                var request = HttpWebRequest.Create(url + art);
                                request.Method = "HEAD";

                                using (var response = await request.GetResponseAsync() as HttpWebResponse)
                                {
                                    if (response != null)
                                    {
                                        result = response.StatusCode;
                                        if (result == System.Net.HttpStatusCode.OK)
                                        {
                                            Image image = new Image();
                                            BitmapImage bit = new BitmapImage(
                                        new Uri(url + art, UriKind.RelativeOrAbsolute));
                                            bit.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                                            image.Source = bit;
                                            image.Name = art;
                                            // posteriormente fazer uma lista com binders para nao alterar todos elementos
                                            // assim removendo / alterando som
                                            imagens.Add(image);
                                        }
                                    }
                                }
                            });
                    }
                }
                catch
                {
                    Debug.WriteLine("IMAGEM ERRO");
                }
            }
            catch { Debug.WriteLine("ERROR"); }
            return imagens;
        }
    }
}
