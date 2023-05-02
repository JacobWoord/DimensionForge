using HelixToolkit.SharpDX.Core;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._3D.Models
{
    public partial class DrawPlane3D : Shape3D
    {
        public Vector3 P1 { get; set; }

        public Vector3 P2 { get; set; }

        public Vector3 P3 { get; set; }

        public List<Vector3> Points { get; set; } = new();


        public DrawPlane3D(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;

            Points.Add(p1);
            Points.Add(p2);
            Points.Add(p3);

        }

        public override void Draw()
        {
           MeshBuilder meshbuilder = new MeshBuilder(); 

            meshbuilder.AddPolygon(Points);
            Color = SharpDX.Color.Purple;
            Material = SetMaterial();
            Geometry = meshbuilder.ToMeshGeometry3D();
        }   
            
        

    }
}
