using DimensionForge._3D.interfaces;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._3D.Data
{
    public class CylinderData : IModelData
    {
        public float Radius { get; set; }
        public float Length { get; set; }
        public Color Color { get; set; }
        public Vector3 StarPos { get; set; }
        public Vector3 EndPos { get; set; }

        public CylinderData()
        {

        }
    }
}
