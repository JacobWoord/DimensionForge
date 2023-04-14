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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Xaml;
using IModelData = DimensionForge._3D.interfaces.IModelData;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Microsoft.Xaml.Behaviors;
namespace DimensionForge._3D.ViewModels
{

    [Serializable]
    public partial class Canvas3DViewModel : ObservableObject
    {
        [JsonIgnore]
        public HelixToolkit.Wpf.SharpDX.OrthographicCamera Camera { get; set; }

        [JsonIgnore]
        public EffectsManager EffectsManager { get; set; }

        [JsonIgnore]
        public Viewport3DX MyViewPort { get; set; }



        public Canvas3DViewModel()
        {
            this.EffectsManager = new DefaultEffectsManager();

            // Create and set up the camera
            Camera = new HelixToolkit.Wpf.SharpDX.OrthographicCamera
            {
                Position = new Point3D(0, 0, 10),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 100000,
                NearPlaneDistance = -10000,
                Width = 1000000,


            };

            Camera.CreateViewMatrix();
        }



        public async Task Draw()
        {
            foreach (var s in shapes)
            {
                if (s is Shape3D shape3D)
                    shape3D.Draw();
                else if (s is ImportedModel b)
                {
                    await b.Import();
                }
            }
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

            var sphere = new Sphere3D { Radius = radius, Color = color, Position = position };
            Shapes.Add(sphere);
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
            Vector3 startPos = new Vector3((float)rand.NextDouble() * 50 - 25, (float)rand.NextDouble() * 200 - 50, (float)rand.NextDouble() * 50 - 25);
            Vector3 endPos = new Vector3((float)rand.NextDouble() * 50 - 25, (float)rand.NextDouble() * 200 - 50, (float)rand.NextDouble() * 50 - 25);
            float length = Vector3.Distance(startPos, endPos);
            var cylinder = new Cylinder3D { Radius = radius, Color = color, P1 = startPos, P2 = endPos };
            Shapes.Add(cylinder);
            cylinder.Draw();
        }

        [RelayCommand]
        [property: JsonIgnore]
        void ScaleDown()
        {
            foreach (var shape in shapes)
            {
                shape.ScaleModel(0.1);
            }
        }
        [RelayCommand]
        [property: JsonIgnore]
        void Translate()
        {
            foreach (var shape in shapes)
            {
                shape.Translate(new Vector3(1, 0, 0));

            }
        }

        [RelayCommand]
        [property: JsonIgnore]
        void Rotate()
        {
            foreach (var shape in shapes)
                shape.Rotate(new Vector3D(1, 0, 0), 90);
        }

        [RelayCommand]
        [property: JsonIgnore]
        public void ConvertTranformations()
        {
            foreach (var shape in shapes)
                shape.ConvertTransform3DGroupToTransformData();
        }


        [RelayCommand]
        [property: JsonIgnore]
        public void ConvertTransformationsBack()
        {
            foreach (var shape in shapes)
            {
                shape.ConvertTransform3DGroupToTransformData();
            }
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

        }

        async Task Import(string filePath)
        {
            var batchedmodel = new ImportedModel(filePath);
            await batchedmodel.OpenFile();
            shapes.Add(batchedmodel);





         

        }


        [RelayCommand]
        [property: JsonIgnore]  
        public void ZoomTo()
        {
            double offset = 1;
            var model = Shapes.FirstOrDefault(x => x is BathedModel3D) as BathedModel3D;
            var bb = model.GetBoundingBox();
            var center = model.GetLocation();

            var rect3d = new Rect3D(center.ToPoint3D(), new Size3D(bb.Width + offset, bb.Height + offset, bb.Depth + offset));


            MyViewPort.ZoomExtents(rect3d, 500);
            MyViewPort.Reset();
        }

        public void OnViewportInitialized(Viewport3DX viewport)
        {
            MyViewPort = viewport;
        }


        [ObservableProperty]
        ObservableCollection<IShape3D> shapes = new();

       



    }
}
