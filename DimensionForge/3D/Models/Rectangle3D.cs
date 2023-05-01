using HelixToolkit.SharpDX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._3D.Models
{
    public partial class Rectangle3D : Shape3D
    {


        public Rectangle3D()
        {
            
        }

        public override void Draw()
        {
            MeshBuilder meshbuilder = new MeshBuilder();
       
            Material = SetMaterial();
            Geometry = meshbuilder.ToMeshGeometry3D();
        }


        public override List<verletElement3D> GetElements()
        {
            return base.GetElements();
        }
    }
}
