using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Media;
using HelixToolkit.Wpf.SharpDX;

namespace DimensionForge._3D.Data
{


    [Serializable]
    public class PhongMaterialData
    {
        public Color DiffuseColor { get; set; }
        public Color SpecularColor { get; set; }
        public float SpecularShininess { get; set; }

        public PhongMaterialData() { }

        
        public Material ToPhongMaterial()
        {
            return new PhongMaterial
            {
                DiffuseColor = DiffuseColor.ToColor4(),
                SpecularColor = SpecularColor.ToColor4(),
                SpecularShininess = SpecularShininess
            };
        }

        public static PhongMaterialData FromPhongMaterial(PhongMaterial material)
        {
            return new PhongMaterialData
            {
                DiffuseColor = material.DiffuseColor.ToColor(),
                SpecularColor = material.SpecularColor.ToColor(),
                SpecularShininess = material.SpecularShininess
            };
        }
    }

}
