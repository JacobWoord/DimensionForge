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
    public partial  class ItemsListViewModel : ObservableObject
    {
        private Canvas3DViewModel canvasViewModel { get; set; }
       
        
        [ObservableProperty]
        ObservableCollection<ObjModel3D>objItems = new();

        public ItemsListViewModel()
        {
            canvasViewModel = Ioc.Default.GetService<Canvas3DViewModel>();
            
            
            
            canvasViewModel.Shapes.ToList().ForEach(obj =>
            {
                if (obj is ObjModel3D obj3D)
                {
                    objItems.Add(obj3D);
                }
            });




        }





        [RelayCommand]
        void CloseWindow()
        {
            canvasViewModel.Navigate("Close");
        }





    }
}
