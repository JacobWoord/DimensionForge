using CommunityToolkit.Mvvm.ComponentModel;
using DimensionForge._2D.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge.Main.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableObject currentViewModel;

        public MainViewModel()
        {
           CurrentViewModel = Ioc.Default.GetService<Canvas2DViewModel>();
        }   

    }
}
