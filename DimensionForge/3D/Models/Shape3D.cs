using DimensionForge._2D.interfaces;
using DimensionForge._3D.interfaces;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;

using DimensionForge._3D.Data;
using Newtonsoft.Json;
using Transform3DGroup = System.Windows.Media.Media3D.Transform3DGroup;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using ScaleTransform3D = System.Windows.Media.Media3D.ScaleTransform3D;
using RotateTransform3D = System.Windows.Media.Media3D.RotateTransform3D;
using Quaternion = System.Windows.Media.Media3D.Quaternion;

using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Material = HelixToolkit.Wpf.SharpDX.Material;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using DimensionForge.Common;
using DimensionForge.HelperTools;

namespace DimensionForge._3D.Models
{
    public partial class Shape3D : ObservableObject, IShape3D
    {
        public string Name { get; set; }    
        public string Id { get; set; }
        public Color4 Color { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 OldPosition { get; set; }
        public bool IsSelected { get; set; }

        public Shape3D()
        {
        
            Id = Guid.NewGuid().ToString();

            material = SetMaterial();

            TransformDatas = new List<TransformData>();

        }

        public IList<TransformData> TransformDatas { get; set; }
        public UseCase UseCase { get; set; }

        [ObservableProperty]
        [property: JsonIgnore]
        Material material;

        [ObservableProperty]
        [property: JsonIgnore]
        HelixToolkit.SharpDX.Core.Geometry3D geometry;
        
        [ObservableProperty]
        Node3D linkedNode;





        public virtual void Draw()
        {


        }
       


        public Material SetMaterial()
        {

            if (Color == SharpDX.Color.Transparent)
            {
                return null;
            }

            var material = PhongMaterials.Red;
            material.DiffuseColor = Color;

          
            return material;
        }


        [RelayCommand]
        public void Select()
        {
          IsSelected = !IsSelected;

            if (IsSelected)
            {

                Color = SharpDX.Color.Yellow;
            }
            else
            {
                Color = SharpDX.Color.Red;
            }
            Draw();
          
        }

        public void Deselect()
        {
           
        }

        public virtual List<VerletElement3D> GetElements()
        {

            var elements = new List<VerletElement3D>();
            return elements;
        }
    }
}
