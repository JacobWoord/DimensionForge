using DimensionForge._2D.interfaces;
using DimensionForge._3D.interfaces;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using HelixToolkit.SharpDX.Core;

namespace DimensionForge._3D.Models
{
    public partial class Shape3D : ObservableObject, IShape3D
    {

        [ObservableProperty]
        Material material;
        public string Id { get; set; }
        public Shape3D()
        {
            transformGroup = new System.Windows.Media.Media3D.Transform3DGroup();
            material = PhongMaterials.Red;
            Id = Guid.NewGuid().ToString();
        }



        [ObservableProperty]
        Geometry3D geometry;

        [ObservableProperty]
        System.Windows.Media.Media3D.Transform3DGroup transformGroup;



        public virtual void Draw()
        {


        }


    }
}
