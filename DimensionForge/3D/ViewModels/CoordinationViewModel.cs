using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using SharpDX;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using Newtonsoft.Json;
using System;
using HelixToolkit.Wpf.SharpDX;
using DimensionForge._2D.ViewModels;
using System.Linq;
using HelixToolkit.SharpDX.Core.Model.Scene;

namespace DimensionForge._3D.ViewModels
{
    public partial class CoordinationViewModel : ObservableObject
    {

        private Canvas3DViewModel canvasViewModel;

        [ObservableProperty]
        ObservableCollection<string> itemsControl = new ObservableCollection<string>();

        public CoordinationViewModel()
        {
            canvasViewModel = Ioc.Default.GetService<Canvas3DViewModel>();

            itemsControl.Add("Axis view propperties");
            itemsControl.Add("Angle calc");
        }


     
        [RelayCommand]
        [property: JsonIgnore]
        void ScaleDown()
        {
            foreach (var shape in canvasViewModel.Shapes)
            {
               //shape.ScaleModel(0.1);
            }
        }

     


           
         


        [RelayCommand]
        [property: JsonIgnore]
        public void ZoomTo()
        {
            //double offset = 1;
            //var model = canvasViewModel.Shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            //var bb = model.GetBoundingBox();
            //var center = model.GetLocation();

            //var rect3d = new Rect3D(center.ToPoint3D(), new Size3D(bb.Width + offset, bb.Height + offset, bb.Depth + offset));

            //canvasViewModel.MyViewPort.ZoomExtents(rect3d, 500);
            //canvasViewModel.MyViewPort.Reset();
        }


    }
}
