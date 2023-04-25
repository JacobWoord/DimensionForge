using HelixToolkit.SharpDX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._3D.Models
{
    public partial class verletElement3D : Shape3D
    {

        [ObservableProperty]
        Node3D p1;

        [ObservableProperty]
        Node3D p2;

        [ObservableProperty]
        float length;

        [ObservableProperty]
        float radius = 0.8f;

        public verletElement3D()
        {

           

        }

        public override void Draw()
        {
            MeshBuilder meshbuilder = new MeshBuilder();
            meshbuilder.AddCylinder(p1.Position, p2.Position, 0.2, 32);
            meshbuilder.AddSphere(p1.Position, 0.5);
            meshbuilder.AddSphere(p2.Position, 0.5);
            Material = SetMaterial();
            Geometry = meshbuilder.ToMeshGeometry3D();
        }
    }

}

