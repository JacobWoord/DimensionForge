using DimensionForge._3D.interfaces;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._3D.Data
{

    [Serializable]
    public class SphereData : IModelData
    {
        public float Radius { get; set; }
        public Color Color { get; set; }
        public Vector3 Position { get; set; }
    
        public SphereData()
        {

        }
    }


}
