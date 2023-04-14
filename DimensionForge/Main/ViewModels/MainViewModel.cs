using DimensionForge._2D.ViewModels;
using DimensionForge._3D.ViewModels;
using HelixToolkit.Wpf.SharpDX;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DimensionForge.Main.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableObject currentViewModel;

        public MainViewModel()
        {
            CurrentViewModel = Ioc.Default.GetService<Canvas3DViewModel>();
        }

        [RelayCommand]
        void Navigate(string destination)
        {
            switch (destination)
            {
                case "3DViewer":
                    CurrentViewModel = Ioc.Default.GetService<Canvas3DViewModel>();
                    break;
                case "2DViewer":
                    CurrentViewModel = Ioc.Default.GetService<Canvas2DViewModel>();
                    break;
                    // Add more cases for other destinations
            }
        }


        [RelayCommand]
        void Save()
        {
            var dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                // Save file         
                if (currentViewModel is Canvas2DViewModel canvas2d)
                {
                    Serializer.WriteToJsonFile(dialog.FileName, canvas2d);
                }

                else if (currentViewModel is Canvas3DViewModel canvas3d)
                {
                   canvas3d.ConvertTranformations();
                   Serializer.WriteToJsonFile(dialog.FileName, canvas3d);
                   Debug.WriteLine($"Memory address main: {System.Runtime.InteropServices.GCHandle.ToIntPtr(System.Runtime.InteropServices.GCHandle.Alloc(this))}");

                }


            }
        }

        [RelayCommand]
        async Task Load()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                // Load file
                if (currentViewModel is Canvas2DViewModel canvas2d)
                    Serializer.PopulateFromJsonFile(canvas2d, dialog.FileName);
                else if (currentViewModel is Canvas3DViewModel canvas3d)
                {
                    canvas3d.Shapes.Clear();
                    Serializer.PopulateFromJsonFile(canvas3d, dialog.FileName);
                    canvas3d.ConvertTransformationsBack();
                    await  canvas3d.Draw();
                }

            }
        }

    

    }
}
