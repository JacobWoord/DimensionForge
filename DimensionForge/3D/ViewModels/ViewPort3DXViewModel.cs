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
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DimensionForge._3D.ViewModels
{

    public partial class ViewPort3DXViewModel : ObservableObject
    {

        public HelixToolkit.Wpf.SharpDX.PerspectiveCamera Camera { get; set; }
        public AmbientLight3D AmbientLight { get; set; }
        public DirectionalLight3D DirectionalLight { get; set; }
        public  EffectsManager EffectsManager { get; set; }

        
        public ViewPort3DXViewModel()
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


            AddShape();
            AmbientLight = new AmbientLight3D
            {
                Color = Color.FromRgb(64, 64, 64)
            };

            DirectionalLight = new DirectionalLight3D
            {
                Color = Color.FromRgb(240, 240, 240),
                Direction = new Vector3D(-1, -1, -1)
            };
        }

        [ObservableProperty]
        ObservableCollection<IShape3D> shapes = new();


        public void AddShape()
        {
            shapes.Add(new Sphere3D());
        }
    }
}
