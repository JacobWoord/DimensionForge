using DimensionForge._2D.interfaces;
using DimensionForge._3D.interfaces;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using HelixToolkit.SharpDX.Core;
using DimensionForge._3D.Data;
using Newtonsoft.Json;

namespace DimensionForge._3D.Models
{
    public partial class Shape3D : ObservableObject, IShape3D
    {
        public Shape3D()
        {
            transformGroup = new System.Windows.Media.Media3D.Transform3DGroup();

            Id = Guid.NewGuid().ToString();

            material = SetMaterial();

        }
        public string Id { get; set; }



        public Color4 Color { get; set; }
        public Vector3 position { get; set; }

        [ObservableProperty]
        [property: JsonIgnore]
        Material material;

        [ObservableProperty]
        [property: JsonIgnore]
        Geometry3D geometry;

        [ObservableProperty]
        [property: JsonIgnore]
        System.Windows.Media.Media3D.Transform3DGroup transformGroup;




        public virtual void Draw()
        {


        }

        public Material SetMaterial()
        {
            var material = PhongMaterials.Silver;
            material.DiffuseColor = Color;
            return material;
        }
    }
}
