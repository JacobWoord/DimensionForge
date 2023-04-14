using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace DimensionForge._3D.Data
{
    public class TransformData
    {
        public Vector3D Translation { get; set; }
        public Vector3D Scale { get; set; }
        public Quaternion Rotation { get; set; }

        public TransformData()
        {
            
        }

        public TransformData(Vector3D translation, Vector3D scale, Quaternion rotation)
        {
            Translation = translation;
            Scale = scale;
            Rotation = rotation;
        }
    }
}
