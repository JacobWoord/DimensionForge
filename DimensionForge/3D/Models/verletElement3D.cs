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

        public verletElement3D(Node3D pos1, Node3D pos2)
        {

            P1 = pos1;
            P2 = pos2;

        }

        public override void Draw()
        {
            MeshBuilder meshbuilder = new MeshBuilder();
            meshbuilder.AddCylinder(p1.Position, p2.Position, 0.2, 32);
            Material = SetMaterial();
            Geometry = meshbuilder.ToMeshGeometry3D();
        }
    }

}

