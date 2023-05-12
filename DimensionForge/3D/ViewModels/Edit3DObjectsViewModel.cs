using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using SharpDX;
using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Linq;
using MaterialDesignThemes.Wpf;
using System.Globalization;

namespace DimensionForge._3D.ViewModels
{
    public partial class Edit3DObjectsViewModel : ObservableObject
    {
       

        private Canvas3DViewModel canvasViewModel;
     
        public Edit3DObjectsViewModel()
        {
            canvasViewModel = Ioc.Default.GetService<Canvas3DViewModel>();
        }

    
        Random rand = new Random();

     




         

    }
}
