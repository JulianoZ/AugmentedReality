using RALIBRARY_V2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x416

namespace Exemple2
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<Image> list_imgs;
        ImageControl imageControl = new ImageControl();
        int time = 2000; // tempo de atualizacao das imagens

        public MainPage()
        {
            this.InitializeComponent();

            Thread t = new Thread(AsyncTaksAsync);// Roda as tarefas async
            t.Start(); // inicia a thread
            

        }

        private async void AsyncTaksAsync()
        {
            
            await Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => // cria uma Async para poder manipular o canvas em tempo de execução
            {
            StorageFolder storageFolder = KnownFolders.PicturesLibrary;
            storageFolder = await storageFolder.GetFolderAsync("RALIBRARY");
            list_imgs = await imageControl.GetDiskImagesAsync(storageFolder);
            Debug.WriteLine(storageFolder.Path);
            int contador = 0; // contador da imagem atual
                              // inicia a troca de imagens de maneira permanente
                Image getImg;
                while (true)
                {

                    if (contador < list_imgs.Count) // controle de quantidade de imagens juntamente ao contador
                    {
                        getImg = list_imgs[contador];
                        await imageControl.AddImgtoCanvasAsync(canvas, getImg); // adiciona imagem ao canvas
                    }
                    else
                    {
                        contador = 0;
                        if (contador > 0) await imageControl.AddImgtoCanvasAsync(canvas, list_imgs[contador]); // caso tehna alguma imagem
                    }
                    contador++;

                    // adiciona o tempo de espera na imagem
                    await Task.Delay(time);
                }
            });

        }
    }
}
