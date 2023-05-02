using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimensionForge.Common;
using Plane = SharpDX.Plane;
using System.Security.Cryptography.Pkcs;
using Net_Designer_MVVM;

namespace DimensionForge._3D.Models
{
    public partial class VerletData3D : ObservableObject
    {
        [ObservableProperty]
        List<Node3D> cornerList = new List<Node3D>();

        [ObservableProperty]
        List<verletElement3D> elementList = new();

        [ObservableProperty]
        Plane verletPlane = new Plane();

        public Vector3 Normal { get; set; }

        public float Angle { get; set; }
        public Vector3 P1 { get; set; }
        public Vector3 P2 { get; set; }

        public VerletData3D()
        {
           
        }

        






      








        public Vector3 GetCenter()
        {


            // Find the minimum and maximum coordinates in each dimension
            float minX = cornerList.Min(p => p.Position.X);
            float minY = cornerList.Min(p => p.Position.Y);
            float minZ = cornerList.Min(p => p.Position.Z);
            float maxX = cornerList.Max(p => p.Position.X);
            float maxY = cornerList.Max(p => p.Position.Y);
            float maxZ = cornerList.Max(p => p.Position.Z);

            // Calculate the center point of the square
            Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);


            return center;
        }
    }
}
