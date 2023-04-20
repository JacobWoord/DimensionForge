using DimensionForge._3D.Data;
using HelixToolkit.SharpDX.Core;

using SharpDX;
using System;

namespace DimensionForge._3D.Models
{

    public class Sphere3D : Shape3D
    {
        public float Radius { get; set; }
       

        public Sphere3D()
        {       

        }

        public override void Draw()
        {        
            MeshBuilder meshbuilder = new MeshBuilder();
            meshbuilder.AddSphere(Position, Radius, 10, 10);
            Material = SetMaterial();
            Geometry = meshbuilder.ToMeshGeometry3D();
        }


    }
}
