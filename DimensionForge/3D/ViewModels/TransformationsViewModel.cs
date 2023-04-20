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

namespace DimensionForge._3D.ViewModels
{
    public partial class TransformationsViewModel : ObservableObject
    {

        private Canvas3DViewModel canvasViewModel;



        public TransformationsViewModel()
        {
            canvasViewModel = Ioc.Default.GetService<Canvas3DViewModel>();
        }

     


        [RelayCommand]
        [property: JsonIgnore]
        void ScaleDown()
        {
            foreach (var shape in canvasViewModel.Shapes)
            {
                shape.ScaleModel(0.1);
            }
        }
        [RelayCommand]
        [property: JsonIgnore]
        void Translate()
        {
            foreach (var shape in canvasViewModel.Shapes)
            {
                shape.Translate(new Vector3(1, 0, 0));

            }
        }

        [RelayCommand]
        [property: JsonIgnore]
        void Rotate()
        {
            foreach (var shape in canvasViewModel.Shapes)
                shape.Rotate(new Vector3D(1, 0, 0), 90);
        }


        [RelayCommand]
        [property: JsonIgnore]
        public void ZoomTo()
        {
            double offset = 1;
            var model = canvasViewModel.Shapes.FirstOrDefault(x => x is BathedModel3D) as BathedModel3D;
            var bb = model.GetBoundingBox();
            var center = model.GetLocation();

            var rect3d = new Rect3D(center.ToPoint3D(), new Size3D(bb.Width + offset, bb.Height + offset, bb.Depth + offset));

            canvasViewModel.MyViewPort.ZoomExtents(rect3d, 500);
            canvasViewModel.MyViewPort.Reset();
        }


    }
}
