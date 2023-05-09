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
               //shape.ScaleModel(0.1);
            }
        }

        [RelayCommand]
        void RotateDoor(string axis)
        {
            Vector3D definedVector;

            switch (axis)
            {
                case "X":
                    definedVector = new Vector3D(1, 0, 0);
                    break;
                case "Y":
                    definedVector = new Vector3D(0, 1, 0);
                    break;
                case "Z":
                    definedVector = new Vector3D(0, 0, 1);
                    break;
            } 

            var door = canvasViewModel.Shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            door.RotateAroundCenter(definedVector, 20);
        }



        [RelayCommand]
        void SetCornerNodes()
        {
            var door =canvasViewModel.Shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            door.Bbcorners.ForEach(x =>canvasViewModel.Shapes.Add(new CornerPoint3D() { LinkedNode = x , Radius=10, Color= Color.Green}));
            canvasViewModel.Draw();
        }
            
           
        [RelayCommand]
        [property: JsonIgnore]
        void Translate()
        {

           var door = canvasViewModel.Shapes.FirstOrDefault(x=> x is BatchedModel3D) as BatchedModel3D;

            door.MoveCenterToPosition(new Vector3(0, 0, 0));
          
        }

        [RelayCommand]
        [property: JsonIgnore]
        void CalcDoorTrans()
        { 

            //var door = canvasViewModel.Shapes.FirstOrDefault(x => x is BathedModel3D) as BathedModel3D;
            //var center = canvasViewModel.VerletNodes;

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
            var model = canvasViewModel.Shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            var bb = model.GetBoundingBox();
            var center = model.GetLocation();

            var rect3d = new Rect3D(center.ToPoint3D(), new Size3D(bb.Width + offset, bb.Height + offset, bb.Depth + offset));

            canvasViewModel.MyViewPort.ZoomExtents(rect3d, 500);
            canvasViewModel.MyViewPort.Reset();
        }


    }
}
