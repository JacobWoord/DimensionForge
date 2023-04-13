using CommunityToolkit.Mvvm.ComponentModel;
using DimensionForge._2D.ViewModels;
using DimensionForge._3D.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;


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
                if (currentViewModel is Canvas2DViewModel canvas)
                    Serializer.WriteToJsonFile(dialog.FileName, canvas);
                else if (currentViewModel is Canvas3DViewModel viewport)
                    Serializer.WriteToJsonFile(dialog.FileName, viewport);
            }
        }

        [RelayCommand]
        void Load()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                // Load file
            }
        }
    }
}
