using CommunityToolkit.Mvvm.ComponentModel;
using DimensionForge._2D.ViewModels;
using DimensionForge._3D.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DimensionForge.Main.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ICommand NavigationCommand { get; private set; }

        [ObservableProperty]
        ObservableObject currentViewModel;

        public MainViewModel()
        {
            NavigationCommand = new RelayCommand<string>(Navigate);
            CurrentViewModel = Ioc.Default.GetService<Canvas2DViewModel>();
        }

      
        
        
        private void Navigate(string destination)
        {
            switch (destination)
            {
                case "3DViewer":
                    CurrentViewModel = Ioc.Default.GetService<ViewPort3DXViewModel>();
                    break;
                case "2DViewer":
                    CurrentViewModel = Ioc.Default.GetService<Canvas2DViewModel>();
                    break;
                    // Add more cases for other destinations
            }
        }

    }
}
