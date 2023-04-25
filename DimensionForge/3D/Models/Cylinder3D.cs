using DimensionForge._3D.Data;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct3D9;
using Material = HelixToolkit.Wpf.SharpDX.Material;


namespace DimensionForge._3D.Models
{
    public partial class Cylinder3D : Shape3D
    {

        [ObservableProperty]
        Node3D p1;

        [ObservableProperty]
        Node3D p2;

        [ObservableProperty]
        float radius = 0.8f;

        [ObservableProperty]
        float lentgh;

        public Cylinder3D()
        {

        }


        public override void Draw()
        {
            

            MeshBuilder meshbuilder = new MeshBuilder();
            meshbuilder.AddCylinder(P1.Position, P2.Position, Radius, 32);
            Material = SetMaterial();
            Geometry = meshbuilder.ToMeshGeometry3D();
        }

    }
}
