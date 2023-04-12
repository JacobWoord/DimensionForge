using DimensionForge._2D.interfaces;
using DimensionForge._3D.interfaces;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;

namespace DimensionForge._3D.Models
{
    public partial class Shape3D : ObservableObject, IShape3D
    {

        private HelixToolkit.Wpf.SharpDX.Material originalMaterial = PhongMaterials.Red;


        public Shape3D()
        {
            var sphereMesh = new MeshBuilder();
            sphereMesh.AddSphere(new SharpDX.Vector3(0, 0, 0), 20);
            this.geometry3D = sphereMesh.ToMeshGeometry3D();
        }


        [ObservableProperty]
        HelixToolkit.SharpDX.Core.Geometry3D geometry3D;

    }
}
