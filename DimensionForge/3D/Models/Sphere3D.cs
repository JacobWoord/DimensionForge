using DimensionForge._3D.Data;
using HelixToolkit.SharpDX.Core;
using Color = SharpDX.Color;
using SharpDX;
using System;
using System.Collections.Generic;

namespace DimensionForge._3D.Models
{

    public class Sphere3D : Shape3D
    {
        public float Radius { get; set; } = 0.8f;
       
        public Sphere3D()
        {       
            Color = SharpDX.Color.Red;
        }

        public override void Draw()
        {        
            MeshBuilder meshbuilder = new MeshBuilder();
            meshbuilder.AddSphere(Position.Position, Radius, 10, 10);
            Material = SetMaterial();
            Geometry = meshbuilder.ToMeshGeometry3D();
            Color = SharpDX.Color.Red;
        }
        public override List<verletElement3D> GetElements()
        {
            return base.GetElements();
        }

    }
}
