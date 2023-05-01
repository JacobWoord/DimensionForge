using DimensionForge._3D.Data;
using DimensionForge._3D.interfaces;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace DimensionForge._3D.Models
{
    public partial class CornerPoint3D : Shape3D
    {
        [ObservableProperty]
        Node3D linkedNode;

        public float Radius { get; set; } = 1f;

        public CornerPoint3D()
        {
            
        }

        public override void Draw()
        {
            MeshBuilder meshbuilder = new MeshBuilder();
            meshbuilder.AddSphere(linkedNode.Position, Radius, 10, 10);
            Color = SharpDX.Color.Red;
            Material = SetMaterial();
            
            Geometry = meshbuilder.ToMeshGeometry3D();
        }

        public override List<verletElement3D> GetElements()
        {
            return base.GetElements();
        }



    }
}
