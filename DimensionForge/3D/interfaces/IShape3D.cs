using DimensionForge._3D.Data;
using DimensionForge._3D.Models;
using DimensionForge.Common;
using SharpDX;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace DimensionForge._3D.interfaces
{
    public interface IShape3D
    {
        HelixToolkit.Wpf.SharpDX.Material SetMaterial();
        public void ScaleModel(double scaleFactor);
        public IList<TransformData> TransformDatas { get; set; }
        public void Rotate(Vector3D Axis, double Angle);
     
        public void Translate(Vector3 translation);
       
        public List<verletElement3D> GetElements();
        public UseCase UseCase {get; set;}
    }
}
