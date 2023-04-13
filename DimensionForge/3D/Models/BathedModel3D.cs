using DimensionForge._3D.interfaces;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using RapiD.Geometry;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Material = HelixToolkit.Wpf.SharpDX.Material;

namespace DimensionForge._3D.Models
{

    public partial class BathedModel3D: ObservableObject, IShape3D
    {
        public string ID = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }

        [ObservableProperty]
        CullMode cullMode = CullMode.Back;

        [ObservableProperty]
        IList<BatchedMeshGeometryConfig> batchedMeshes;

        [ObservableProperty]
        IList<Material> modelMaterials;

        [ObservableProperty]
        Material baseMaterial = PhongMaterials.Red;

        [ObservableProperty]
        List<Vector3> nodeList = new();

        [ObservableProperty]
        Transform3DGroup transform;

        public BathedModel3D()
        {
            batchedMeshes = new List<BatchedMeshGeometryConfig>();
            modelMaterials = new List<Material>();
        }
        

        public async Task OpenFile()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                Debug.WriteLine("No file specified");
                return;
            }

            var configs = await Task.Run(() => Importer3D.OpenFile(FileName));
            var models = configs.Where(x => x.Name.Contains("anchor") == false);

            foreach (var m in models)
            {
                BatchedMeshes.Add(m.BatchedMeshGeometryConfig);
                ModelMaterials.Add(m.MaterialCore.ConvertToMaterial());
            }

            var anchors = configs.Where(x => x.Name.Contains("anchor") == true);

            //positions of nodes in door added to vector3 List
            foreach (var item in anchors)
            {
                nodeList.Add(item.Location);
            }


        }


        public Material SetMaterial()
        {
            throw new NotImplementedException();
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
