using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using RALIBRARY_V2;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.ViewManagement;
using Windows.Storage;
using System.Text;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.Storage.Streams;
using Windows.System;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.Media.Core;
using System.Net;
using System.Threading;
using Windows.Storage.FileProperties;
using System.Net.Sockets;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x416

namespace OnlineRA
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    /// 


    public sealed partial class MainPage : Page
    {
        int time = 4000;
        List<Image> imagens = new List<Image>();
        List<Produtos> produtos = new List<Produtos>();
        CanvasDraw canvasDraw = new CanvasDraw();
        Image imagem_atual = new Image();
        Windows.UI.Color cor = Colors.White;
        private bool s_socket = false, s_video = false, ferramentas = false,s_local = false,desenho = false, linha = false, circulo = false, quadrado = false, retangulo = false, color = false, texto = false;
        private double size = 10;
        List<StorageFile> videos = new List<StorageFile>();
        List<double> duracao = new List<double>();

        public MainPage()
        {
            this.InitializeComponent();
            canvasDraw.HideComponent(imtexto, imlinha, imcirculo, imquadrado, imretangulo,imtamanho, imcor, imferramentas, imferramentas, imlimpar, imsalvar);
            ApplicationView view = ApplicationView.GetForCurrentView();
            view.TryEnterFullScreenMode();
            AtualizarImagemVideo();
            var t = Task.Run(() => ShowThreadInfo());
            var socket = Task.Run(() => SocketListenerAsync());

        }

        private async void SocketListenerAsync()
        {

            Socket listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSock.Bind(new IPEndPoint(IPAddress.Any, 1209));
            listenSock.Listen(1);// quantidade de conexoes
            while (true)
            {
                if (s_socket)
                {
                    byte[] recebida = new byte[1024];

                    using (Socket newConnection = listenSock.Accept())
                    {
                        newConnection.Receive(recebida);
                        string string_msg = Encoding.UTF8.GetString(recebida).Substring(0, Encoding.UTF8.GetString(recebida).IndexOf("\0"));
                        if (string_msg.Equals("handshake"))
                        {
                            byte[] msg_send = Encoding.UTF8.GetBytes("hi");
                            newConnection.Send(msg_send);
                        }
                        else
                        {
                            // verificar se possui a imagem
                            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                            {
                                if (imvideo.Text.Equals("Imagens"))
                                {
                                    if (imlocal.Text.Equals("ONLINE"))
                                    {
                                        for (int i = 0; i < produtos.Count; i++)
                                        {
                                            if ("selectbyname_" + produtos[i].Name == string_msg)
                                            {
                                                canvas.Children.Clear();
                                                canvas.Children.Add(imagens[i]);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for(int i = 0; i < imagens.Count; i++)
                                        {
                                            Debug.WriteLine(string_msg);
                                            Debug.WriteLine(imagens[i].Name.Substring(0, imagens[i].Name.Length - 4));
                                            if ("selectbyname_" + imagens[i].Name.Substring(0, imagens[i].Name.Length-4) == string_msg)
                                            {
                                                canvas.Children.Clear();
                                                canvas.Children.Add(imagens[i]);
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    }
                }
            }
        }

        private void AtualizarImagemVideo()
        {
            var t1 = Task.Run(async () => {

                while (true)
                {
                    StorageFolder folder_imagens = KnownFolders.PicturesLibrary;
                    folder_imagens = await folder_imagens.GetFolderAsync("RALIBRARY");
                    GetImagens(folder_imagens);

                    FileControl fileControl = new FileControl();
                    VideoControl videoControl = new VideoControl();
                    videos.Clear();
                    duracao.Clear();
                    videos = await fileControl.GetDiskFileAsync(KnownFolders.VideosLibrary);
                    foreach (StorageFile vid in videos)
                    {
                        duracao.Add(await videoControl.getDurationVideoAsync(vid));
                    }
                    await Task.Delay(10000);
                }
            });
        }

        public async void GetImagens(StorageFolder picturesFolder)
        {
            ImageControl fileControl = new ImageControl();

            imagens.Clear();
            // s_local = false = local ,,, true = nuvem
            if (!s_local)
            {
                imagens = await fileControl.GetDiskImagesAsync(picturesFolder);
            }
            else
            {
                ProdutosDao dao = new ProdutosDao();
                produtos = await dao.getListAsync();
                List<string> prod_name = new List<string>();
                if (produtos != null)
                    foreach (Produtos prod in produtos)
                    {
                        prod_name.Add(prod.PictureMap);
                    }
                imagens = await fileControl.GetImageWebAsync("http://julianoblanco-001-site3.ctempurl.com/Images/MapsAR/", prod_name);
            }
            
        }

    
        private async void ShowThreadInfo()
        {
            int i = 0;
            bool vidimage = false; // falso = imagem
            while (true)
            {
                if (!s_socket)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        canvas.Children.Clear();
                        mediaelement.Source = null;
                    });
                    if (!s_video)
                    {
                        if (vidimage) i = 0;
                        vidimage = false;
                        //imagens
                        int val = imagens.Count;
                        if (val != imagens.Count) i = 0;
                        if (imagens.Count > 0 && !desenho)
                        {
                            if (i >= imagens.Count) i = 0;
                            ImageControl imageControle = new ImageControl();
                            imagem_atual = imagens[i];
                            await imageControle.AddImgtoCanvasAsync(canvas, imagens[i]);
                            await Task.Delay(time);
                            i++;
                        }
                    }
                    else
                    {
                        if (!vidimage) i = 0;
                        vidimage = true;
                        //videos
                        //imagens
                        int val = videos.Count;
                        if (val != videos.Count) i = 0;
                        if (videos.Count > 0 && !desenho)
                        {
                            if (i >= videos.Count) i = 0;
                            VideoControl videoControl = new VideoControl();
                            videoControl.StartVideo(videos[i], mediaelement);
                            await Task.Delay(Convert.ToInt32(duracao[i]));
                            i++;
                        }
                    }
                }
            }
        }

        private async void troca_time(object sender, RoutedEventArgs e)
        {
            ContentDialog1 dialog = new ContentDialog1();
            await dialog.ShowAsync();
            time = dialog.time;
        }

        private void menu_texto(object sender, RoutedEventArgs e)
        {
            linha = false;
            circulo = false;
            quadrado = false;
            retangulo = false;
            color = false;
            texto = true;
        }

        private void menu_linha(object sender, RoutedEventArgs e)
        {
            linha = true;
            circulo = false;
            quadrado = false;
            retangulo = false;
            color = false;
            texto = false;
        }

        private void menu_circulo(object sender, RoutedEventArgs e)
        {
            linha = false;
            circulo = true;
            quadrado = false;
            retangulo = false;
            color = false;
            texto = false;
        }

        private void menu_quadrado(object sender, RoutedEventArgs e)
        {
            linha = false;
            circulo = false;
            quadrado = true;
            retangulo = false;
            color = false;
            texto = false;
        }

        private void menu_retangulo(object sender, RoutedEventArgs e)
        {
            linha = false;
            circulo = false;
            quadrado = false;
            retangulo = true;
            color = false;
            texto = false;
        }

        private void menu_selecionarcor(object sender, RoutedEventArgs e)
        {
            color = !color;
            if (color)
            {
                colorpicker.Visibility = Visibility.Visible;
                canvas.Children.Remove(colorpicker);
                canvas.Children.Add(colorpicker);
            }
            else
            {
                colorpicker.Visibility = Visibility.Collapsed;
                canvas.Children.Remove(colorpicker);
            }
        }

        private void menu_fullscreen(object sender, RoutedEventArgs e)
        {
            linha = false;
            circulo = false;
            quadrado = false;
            retangulo = false;
            color = false;
            texto = false;
            ApplicationView view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
            }
            else
            {
                view.TryEnterFullScreenMode();
            }
        }

        private void menu_ferramentas(object sender, RoutedEventArgs e)
        {
            linha = false;
            circulo = false;
            quadrado = false;
            retangulo = false;
            color = false;
            texto = false;
        }

        private void colorpicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            cor = args.NewColor;
        }

        private void SizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            size = e.NewValue;
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if(!ferramentas && !color)
            {
                if (circulo)
                {
                    canvasDraw.Cursor_Circle(canvas, size, e, cor);
                }
                else if (quadrado)
                {
                    canvasDraw.Cursor_Square(canvas, size, e, cor);
                }
                else if (linha)
                {
                    canvasDraw.Line_Cursor(canvas, size, e, cor);
                }
            }
        }

        private async void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                var properties = e.GetCurrentPoint(this).Properties;
                if (properties.IsLeftButtonPressed)
                {
                    if (quadrado)
                    {
                        canvasDraw.CreateSquare(canvas, e, size, cor);
                    }
                    else if (retangulo)
                    {
                        canvasDraw.Create_Rectangle(canvas, e, cor);
                    }
                    else if (circulo)
                    {
                        canvasDraw.CreateCircle(canvas, e, size, cor);
                    }
                    else if (linha)
                    {
                        canvasDraw.CreateLine(canvas, size, e, cor);
                    }
                    else if (texto)
                    {
                        Tuple<string, double, double> vals = await InputTextDialogAsync("Digite o Texto", e);
                        if (vals != null)
                            canvasDraw.CreateText(vals.Item1, canvas, size, e, cor, vals.Item2, vals.Item3);
                    }}
                else
                {
                    canvasDraw.DisableClick();
                }}
        }

        private async Task<Tuple<string, double, double>> InputTextDialogAsync(string title, PointerRoutedEventArgs e)
        {
            double x = e.GetCurrentPoint(canvas).Position.X;
            double y = e.GetCurrentPoint(canvas).Position.Y;
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;
            ContentDialog dialog = new ContentDialog();
            dialog.Content = inputTextBox;
            dialog.Title = title;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Ok";
            dialog.SecondaryButtonText = "Cancelar";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                return new Tuple<string, double, double>(inputTextBox.Text, x, y);
            }
            return null;
        }

        private void menu_desenho(object sender, RoutedEventArgs e)
        {
            desenho = !desenho;
            if (desenho)
            {
                canvasDraw.VisibleComponent(imtexto,imlinha,imtamanho,imcirculo,imquadrado,imretangulo,imcor,imferramentas,imferramentas,imlimpar,imsalvar);
            }
            else
            {
                canvasDraw.HideComponent(imtexto, imlinha,imtamanho, imcirculo, imquadrado, imretangulo, imcor, imferramentas, imferramentas, imlimpar, imsalvar);
            }
        }

        private void menu_local(object sender, RoutedEventArgs e)
        {
            if (imlocal.Text == "ONLINE")
            {
                imlocal.Text = "LOCAL";
                s_local = false;
            }
            else
            {
                imlocal.Text = "ONLINE";
                s_local = true;
            }
        }

        private void menu_socket(object sender, RoutedEventArgs e)
        {
            s_socket = !s_socket;
            if (s_socket)
            {
                imsocket.Text = "Socket Ativo";
                canvas.Children.Clear();
            }
            else
            {
                imsocket.Text = "Socket Inativo";
            }
        }

        private void menu_video(object sender, RoutedEventArgs e)
        {
            desenho = false;
            linha = false;
            circulo = false;
            quadrado = false;
            retangulo = false;
            color = false;
            texto = false;

            s_video = !s_video;
            if (s_video)
            {
                imvideo.Text = "Video Executando";
                canvasDraw.HideComponent(imlocal, imtrocar_time, imtexto, imlinha, 
                    imcirculo, imquadrado, imretangulo, imcor, imferramentas, imlimpar, imsalvar, imtamanho, imdesfazer);
            }
            else
            {
                imvideo.Text = "Imagens";
                canvasDraw.VisibleComponent(imlocal, imtrocar_time, imtexto, imlinha,
                                    imcirculo, imquadrado, imretangulo, imcor, imferramentas, imlimpar, imsalvar, imtamanho, imdesfazer);
            }
        }

        private void menu_limpartela(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
        }

        private async void menu_salvarimagem(object sender, RoutedEventArgs e)
        {

            FileControl fileControl = new FileControl();
            StorageFolder folder = KnownFolders.PicturesLibrary;
            folder = await folder.GetFolderAsync("RALIBRARY");
            await fileControl.saveImageAsync(canvas, folder, imagem_atual.Name);
            if (s_local)
            {
                StorageFile file = await folder.GetFileAsync(imagem_atual.Name);
                SmallUpload1(file);
            }
        }

        public void SmallUpload1(StorageFile file)
        {
            string uri = "ftp://ftp.site4now.net/MapsAR/";
            string login = "SistemaWebMatheus";
            string password = "docwebrest2018";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri + file.Name);
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = true;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(login, password);

            new Thread(async () =>
            {
                try
                {
                    byte[] fileContents;
                    using (Stream stream = await file.OpenStreamForReadAsync())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            fileContents = memoryStream.ToArray();
                        }
                    }
                    request.ContentLength = fileContents.Length;

                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    response.Close();
                }
                catch
                {
                    Debug.WriteLine("ERRO");
                }
            }).Start();
        }

        private void menu_tamanho10(object sender, RoutedEventArgs e)
        {
            size = 10;
        }

        private void menu_tamanho20(object sender, RoutedEventArgs e)
        {
            size = 20;
        }

        private void menu_tamanho30(object sender, RoutedEventArgs e)
        {
            size = 30;
        }

        private void menu_tamanho50(object sender, RoutedEventArgs e)
        {
            size = 50;
        }

        private void menu_desfazer(object sender, RoutedEventArgs e)
        {
            canvasDraw.Undo_Draw(canvas);
        }
    }
}
