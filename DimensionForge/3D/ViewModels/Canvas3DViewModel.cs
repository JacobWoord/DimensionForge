using Assimp;
using DimensionForge._3D.Data;
using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using Microsoft.Win32;
using Newtonsoft.Json;
using RapiD.Geometry;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Xaml;
using IModelData = DimensionForge._3D.interfaces.IModelData;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace DimensionForge._3D.ViewModels
{

    [Serializable]
    public partial class Canvas3DViewModel : ObservableObject
    {
        [JsonIgnore]
        public HelixToolkit.Wpf.SharpDX.PerspectiveCamera Camera { get; set; }

        [JsonIgnore]
        public EffectsManager EffectsManager { get; set; }


             
        public Canvas3DViewModel()
        {
            this.EffectsManager = new DefaultEffectsManager();

            // Create and set up the camera
            Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera
            {
                Position = new Point3D(0, 0, 10),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0),
                FieldOfView = 45

            };
                   
            Camera.CreateViewMatrix();
        }

                      
             
        public async void Draw()
        {
            shapes.ToList().ForEach(x => x.Draw());


        }

        Random rand = new Random();

        [RelayCommand]
        [property: JsonIgnore]
        void RandomSpere()
        {
            float minRadius = 1;
            float maxRadius = 20;

       
            Color[] colors = { Color.DarkRed, Color.Yellow, Color.Green, Color.Blue };

            float radius = (float)(rand.NextDouble() * (maxRadius - minRadius) + minRadius);

            Color color = colors[rand.Next(colors.Length)];
            Vector3 position = new Vector3((float)rand.NextDouble() * 50 - 25, (float)rand.NextDouble() * 50 - 25, (float)rand.NextDouble() * 50 - 25);
            Shapes.Add(new Sphere3D { Radius=radius,Color=color, Position=position });
           // ModelData.Add(new SphereData { Radius = radius, Color = color, Position = position });
            Draw();
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
            Vector3 startPos = new Vector3((float)rand.NextDouble() * 50 - 25, (float)rand.NextDouble() * 200 - 50, (float)rand.NextDouble() * 50 - 25);
            Vector3 endPos = new Vector3((float)rand.NextDouble() * 50 - 25, (float)rand.NextDouble() * 200 - 50, (float)rand.NextDouble() * 50 - 25);
            float length = Vector3.Distance(startPos, endPos);

           // ModelData.Add(new CylinderData { StarPos = startPos, EndPos = endPos, Length = length, Radius = radius, Color = color });
            Draw();
        }




        [RelayCommand]
        void Import()
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
                Import(filePath);
            }

        }

        void Import(string filePath)
        {
            //ModelData.Add(new DoorData { Position = new Vector3(0, 0, 0), FilePath = filePath });
            Draw();

        }




        [ObservableProperty]
        ObservableCollection<IShape3D> shapes = new();


       // public List<IModelData> ModelData { get; set; } 

    }
}
