using DimensionForge._2D.Models;
using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DimensionForge._3D.ViewModels
{

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
            ModelData = new List<IModelData>();
            Camera.CreateViewMatrix();

            ModelData.Add(new SphereData { Radius = 100 });

            Draw();
        }

        public void Draw()
        {
            foreach (var data in ModelData)
            {
                if (data is SphereData s)
                    Shapes.Add(new Sphere3D(s));
            }
        }


        [ObservableProperty]
        [JsonIgnore]
        ObservableCollection<IShape3D> shapes = new();


        public List<IModelData> ModelData { get; set; } 



        [RelayCommand]
        void AddShape()
        {
            //Shapes.Add(new Sphere3D(100));
        }
    }
}
