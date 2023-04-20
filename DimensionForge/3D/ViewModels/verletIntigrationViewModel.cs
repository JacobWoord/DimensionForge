using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using DimensionForge._2D.interfaces;
using DimensionForge._2D.Models;
using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using HelixToolkit.Wpf.SharpDX;
using Newtonsoft.Json;
using SharpDX;

namespace DimensionForge._3D.ViewModels
{
    public class verletIntigrationViewModel : ObservableObject
    {
        private Canvas3DViewModel canvasViewModel { get; set; }


        public verletIntigrationViewModel()
        {
            canvasViewModel = Ioc.Default.GetService<Canvas3DViewModel>();
       
        }


      


        
      
      




    }
}
