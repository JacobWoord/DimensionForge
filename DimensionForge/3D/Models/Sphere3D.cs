using HelixToolkit.SharpDX.Core;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace DimensionForge._3D.Models
{

    public interface IModelData
    {
       public Color Color { get; set; }  
    }
    public class CylinderData : IModelData
    {
        public float Radius { get; set; }
        public float Length { get; set; }
        public Color Color { get; set; }
        public CylinderData()
        {

        }
    }
    public class SphereData : IModelData
    {
        public float Radius { get; set; }
        public Color Color { get; set; }
        public SphereData()
        {

        }
    }

    public class Sphere3D : Shape3D
    {

        public SphereData Data { get; set; }
        public Sphere3D(SphereData data)
        {
            Data = data;
            Draw();
        }


        public override void Draw()
        {
            MeshBuilder meshbuilder = new MeshBuilder();
            meshbuilder.AddSphere(new SharpDX.Vector3(0, 0, 0), Data.Radius, 10, 10);
            Geometry = meshbuilder.ToMeshGeometry3D();
        }


    }
}
