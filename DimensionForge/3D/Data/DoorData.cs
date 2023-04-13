using DimensionForge._3D.interfaces;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._3D.Data
{
    public class DoorData : IModelData
    {

        public Vector3 Position { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public Color Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
