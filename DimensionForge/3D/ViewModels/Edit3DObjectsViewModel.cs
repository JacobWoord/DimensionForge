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

        [RelayCommand]
        [property: JsonIgnore]
        void RandomSpere()
        {
            float minRadius = 1;
            float maxRadius = 20;
            Color[] colors = { Color.DarkRed, Color.Yellow, Color.Green, Color.Blue };
            float radius = 5;// (float)(rand.NextDouble() * (maxRadius - minRadius) + minRadius);
            Color color = colors[rand.Next(colors.Length)];
            Vector3 position = new Vector3(rand.Next(2,10), rand.Next(2,10) , rand.Next(2,50));

            var sphere = new Sphere3D { Radius = radius, Color = color, Position = position };
            canvasViewModel.Shapes.Add(sphere);
            sphere.Draw();
        }

        [RelayCommand]
        [property: JsonIgnore]
        void RandomCylinder()
        {

            float minRadius = 1;
            float maxRadius = 20;
            Random rand = new Random();
            Color[] colors = { Color.DarkRed, Color.Yellow, Color.Green, Color.Blue };
            float radius = (float)(rand.NextDouble() * (maxRadius - minRadius) + minRadius);
            Color color = colors[rand.Next(colors.Length)];
            Vector3 startPos = new Vector3((float)rand.NextDouble() * 50 - 25, (float)rand.NextDouble() * 200 - 50, (float)rand.NextDouble() * 0 - 25);
            Vector3 endPos = new Vector3((float)rand.NextDouble() * 50 - 25, (float)rand.NextDouble() * 200 - 50, (float)rand.NextDouble() * 0 - 25);
            float length = Vector3.Distance(startPos, endPos);
            var cylinder = new Cylinder3D { Radius = radius, Color = color, P1 = startPos, P2 = endPos };
            canvasViewModel.Shapes.Add(cylinder);

       
            cylinder.Draw();
        }



        [RelayCommand]
        [property: JsonIgnore]
        async Task Import()
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".obj";
            dialog.Filter = "OBJ files (*.obj)|*.obj";

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                // Get the selected file path and call your existing import function
                string filePath = dialog.FileName;
                // Call your existing import function with the file path
                await Import(filePath);
            }

            var model = canvasViewModel.Shapes.FirstOrDefault(x => x is ImportedModel) as ImportedModel;
            model.ScaleModel(0.1);
            model.SetCornerList();

           // model.cornerNodes.ForEach(x => canvasViewModel.Shapes.Add(new CornerPoint3D { NodePosition = x, Color = Color.Blue , Radius = 1}));

            canvasViewModel.Draw();
        }
          
            

        async Task Import(string filePath)
        {
            var batchedmodel = new ImportedModel(filePath);
            await batchedmodel.OpenFile();
            canvasViewModel.Shapes.Add(batchedmodel);

            var model = canvasViewModel.Shapes.FirstOrDefault(x => x is BathedModel3D);

            model.ScaleModel(0.1);
            
        }
         

    }
}
