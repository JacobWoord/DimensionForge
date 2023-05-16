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
        Node3D start = new Node3D(Vector3.Zero);

        [ObservableProperty]
        Node3D end = new Node3D(Vector3.Zero);

        [ObservableProperty]
        float radius = 0.02f;

        [ObservableProperty]
        float lentgh;

        public UseCase UseCase { get; set; } = UseCase.verlet;

        public Cylinder3D()
        {
            //Color = SharpDX.Color.Transparent;
        }


        public override void Draw()
        {
            

            MeshBuilder meshbuilder = new MeshBuilder();

            if(float.IsNaN(start.Position.X)||float.IsNaN(start.Position.Y)|| float.IsNaN(start.Position.Z))
            {
                start.Position = Vector3.Zero;
            }
            if (float.IsNaN(End.Position.X) || float.IsNaN(End.Position.Y) || float.IsNaN(End.Position.Z))
            {
                End.Position = Vector3.Zero;
            }
            meshbuilder.AddCylinder(Start.Position, End.Position, Radius, 32);
            
            Material = SetMaterial();
            Geometry = meshbuilder.ToMeshGeometry3D();
        }

        public override List<VerletElement3D> GetElements()
        {
            return base.GetElements();
        }

    }
}
