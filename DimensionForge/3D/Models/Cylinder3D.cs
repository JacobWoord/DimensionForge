using DimensionForge._3D.Data;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct3D9;
using Material =  HelixToolkit.Wpf.SharpDX.Material;


namespace DimensionForge._3D.Models
{
    public partial class Cylinder3D : Shape3D
    {

    
        [ObservableProperty]
        Vector3 p1;

        [ObservableProperty]
        Vector3 p2; 

        [ObservableProperty]
        float radius;

        [ObservableProperty]
        float lentgh;



        public Cylinder3D()
        {
                Draw();
        }


        public override void Draw()
        {
            MeshBuilder meshbuilder = new MeshBuilder();
            meshbuilder.AddCylinder(p1,p2,radius,32);
            Material = SetMaterial();
            Geometry = meshbuilder.ToMeshGeometry3D();
        }

    }
}
