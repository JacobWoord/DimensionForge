using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RapiD.Geometry
{
    public static class Importer3D
    {
        
        public static List<ImportedConfig> OpenFile(string path)
        {

            if (File.Exists(path) == false)
            {
                MessageBox.Show($"bestand ({path}) bestaat niet.");
                return null;
            }

            HelixToolkitScene scene = null;
            var config = new ImporterConfiguration();
            config.ImportMaterialType = MaterialType.BlinnPhong;
            var importer = new Importer();
            importer.Configuration.AssimpPostProcessSteps |= Assimp.PostProcessSteps.CalculateTangentSpace;
            scene = importer.Load(path, config);
            var importedmesh = new List<ImportedConfig>();


            if (scene != null)
            {
                if (scene.Root != null)
                {
                    int count = 0;
                    foreach (var node in scene.Root.Traverse())
                    {
                        if (node is HelixToolkit.SharpDX.Core.Model.Scene.MeshNode m)
                        {
                            ImportedConfig importedconfig = new()
                            {
                                Name = m.Name,
                                Location = m.Geometry.Bound.Center,
                                MaterialCore = m.Material,
                                BatchedMeshGeometryConfig = new BatchedMeshGeometryConfig(m.Geometry, Matrix.Identity, count)
                            };
                            importedmesh.Add(importedconfig);
                            count++;
                        }
                    }
                }
            }
            return importedmesh;

        }


        public class ImportedConfig
        {
            public string Name { get; set; }
            public Vector3 Location { get; set; }
            public MaterialCore MaterialCore { get; set; }

            public BatchedMeshGeometryConfig BatchedMeshGeometryConfig { get; set; }
        }
    }

}
