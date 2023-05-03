using DimensionForge._3D.Data;
using DimensionForge.Common;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct3D9;
using System.Collections.Generic;
using Material = HelixToolkit.Wpf.SharpDX.Material;


namespace DimensionForge._3D.Models
{
    public partial class Cylinder3D : Shape3D
    {

        [ObservableProperty]
        Node3D p1 = new Node3D(Vector3.Zero);

        [ObservableProperty]
        Node3D p2 = new Node3D(Vector3.Zero);

        [ObservableProperty]
        float radius = 0.8f;

        [ObservableProperty]
        float lentgh;

        public UseCase UseCase { get; set; } = UseCase.verlet;

        public Cylinder3D()
        {
            Color = SharpDX.Color.Green;
        }


        public override void Draw()
        {
            

            MeshBuilder meshbuilder = new MeshBuilder();
            meshbuilder.AddCylinder(P1.Position, P2.Position, Radius, 32);
            Material = SetMaterial();
            Geometry = meshbuilder.ToMeshGeometry3D();
        }

        public override List<verletElement3D> GetElements()
        {
            return base.GetElements();
        }

    }
}
