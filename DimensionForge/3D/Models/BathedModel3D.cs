using DimensionForge._3D.interfaces;
using HelixToolkit.SharpDX.Core;
using RapiD.Geometry;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Newtonsoft.Json;
using DimensionForge._3D.Data;
using Quaternion = System.Windows.Media.Media3D.Quaternion;
using HelixToolkit.Wpf.SharpDX;
using DimensionForge.HelperTools;
using System.Windows.Media;
using System.Windows;

namespace DimensionForge._3D.Models
{

    public partial class BathedModel3D : ObservableObject, IShape3D
    {

        public string ID = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IList<TransformData> TransformDatas { get; set; }

        public BathedModel3D()
        {
            batchedMeshes = new List<BatchedMeshGeometryConfig>();
            modelMaterials = new List<HelixToolkit.Wpf.SharpDX.Material>();
            TransformDatas = new List<TransformData>();
            Nodes = new List<Vector3>();
            transform = new();
        }

        [ObservableProperty]
        [property: JsonIgnore]
        IList<HelixToolkit.SharpDX.Core.BatchedMeshGeometryConfig> batchedMeshes;

        [ObservableProperty]
        [property: JsonIgnore]
        IList<HelixToolkit.Wpf.SharpDX.Material> modelMaterials;

        [ObservableProperty]
        [property: JsonIgnore]
        HelixToolkit.Wpf.SharpDX.Material baseMaterial = PhongMaterials.Blue;

        [property: JsonIgnore]
        public List<Vector3> Nodes { get; set; }

        [ObservableProperty]
        [property: JsonIgnore]
        Transform3DGroup transform;

    

       
        public void Rotate(Vector3D Axis, double Angle)
        {
            var trans = new RotateTransform3D(
                 new AxisAngleRotation3D(
                     axis: Axis,
                     angle: Angle));

            transform.Children.Add(trans);
        }
        public void ScaleModel(double scaleFactor)
        {
            ScaleTransform3D scaleTransform = new ScaleTransform3D(scaleFactor, scaleFactor, scaleFactor);

            if (transform == null)
            {
                transform = new Transform3DGroup();
            }
            transform.Children.Add(scaleTransform);
        }
        public void ConvertTransform3DGroupToTransformData()
        {
            // Initialize default values
            Vector3D translation = new Vector3D(0, 0, 0);
            Vector3D scale = new Vector3D(1, 1, 1);
            Quaternion rotation = new Quaternion(0, 0, 0, 1);


            // Iterate through the children of the Transform3DGroup and extract the relevant data
            if (transform is not null)
                foreach (var transform in transform.Children)
                {
                    if (transform is TranslateTransform3D translateTransform)
                    {
                        translation = new Vector3D(translateTransform.OffsetX, translateTransform.OffsetY, translateTransform.OffsetZ);
                    }
                    else if (transform is ScaleTransform3D scaleTransform)
                    {
                        scale = new Vector3D(scaleTransform.ScaleX, scaleTransform.ScaleY, scaleTransform.ScaleZ);
                    }
                    else if (transform is RotateTransform3D rotateTransform)
                    {
                        if (rotateTransform.Rotation is AxisAngleRotation3D axisAngleRotation)
                        {
                            Vector3D axis = new Vector3D(
                                axisAngleRotation.Axis.X,
                                axisAngleRotation.Axis.Y,
                                axisAngleRotation.Axis.Z);
                            double angle = axisAngleRotation.Angle;
                            rotation = new Quaternion(
                                axis.X * Math.Sin(angle / 2),
                                axis.Y * Math.Sin(angle / 2),
                                axis.Z * Math.Sin(angle / 2),
                                Math.Cos(angle / 2));
                        }

                    }
                    TransformDatas.Add(new TransformData(translation, scale, rotation));
                }

        }
        public void ConvertTransformDataToTransform3DGroup()
        {
            Transform3DGroup transformGroup = new Transform3DGroup();

            foreach (var data in TransformDatas)
            {
                // Add the translation transformation
                TranslateTransform3D translation = new TranslateTransform3D(
                    data.Translation.X,
                    data.Translation.Y,
                    data.Translation.Z);
                transformGroup.Children.Add(translation);

                // Add the scale transformation
                ScaleTransform3D scale = new ScaleTransform3D(
                    data.Scale.X,
                    data.Scale.Y,
                    data.Scale.Z);
                transformGroup.Children.Add(scale);

                // Add the rotation transformation
                Quaternion rotation = data.Rotation;
                Vector3D axis = new Vector3D(rotation.X, rotation.Y, rotation.Z);
                double angle = 2 * Math.Acos(rotation.W);
                AxisAngleRotation3D axisAngleRotation = new AxisAngleRotation3D(axis, angle);
                RotateTransform3D rotateTransform = new RotateTransform3D(axisAngleRotation);
                transformGroup.Children.Add(rotateTransform);
            }
            transform = transformGroup;
            log.Info("Transform3DGroup created");
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
                Nodes.Add(item.Location);
            }

           
    



        }
        //public BoundingBox GetBoundingBox()
        //{
        //    Vector3 min = new Vector3(float.MaxValue);
        //    Vector3 max = new Vector3(float.MinValue);

        //    foreach (var batchedMesh in batchedMeshes)
        //    {
        //        var geometry = batchedMesh.Geometry;
        //        var transform = batchedMesh.ModelTransform;

        //        // Combine the model transform and the current transform group
        //        var combinedTransform = Transform3DHelper.Combine(new MatrixTransform3D(new Matrix3D(transform.M11, transform.M12, transform.M13, transform.M14,
        //                                                                                    transform.M21, transform.M22, transform.M23, transform.M24,
        //                                                                                    transform.M31, transform.M32, transform.M33, transform.M34,
        //                                                                                    0, 0, 0, 1)),
        //                                                           this.transform);

        //        for (int i = 0; i < geometry.Positions.Count; i++)
        //        {
        //            Vector4 position4 = Vector3.Transform(geometry.Positions[i], combinedTransform.Value.ToSharpDX());
        //            Vector3 position = new Vector3(position4.X, position4.Y, position4.Z);

        //            min = Vector3.Min(min, position);
        //            max = Vector3.Max(max, position);
        //        }
        //    }

        //    return new BoundingBox(min, max);
        //}
        public Vector3 GetLocation()
        {
            Vector3 location = new Vector3();

            foreach (var batchedMesh in batchedMeshes)
            {
                var transform = batchedMesh.ModelTransform;

                // Combine the model transform and the current transform group
                var combinedTransform = Transform3DHelper.Combine(new MatrixTransform3D(new Matrix3D(transform.M11, transform.M12, transform.M13, transform.M14,
                                                                                                transform.M21, transform.M22, transform.M23, transform.M24,
                                                                                                transform.M31, transform.M32, transform.M33, transform.M34,
                                                                                                0, 0, 0, 1)),
                                                                    this.transform);

                // Transform the origin point (0, 0, 0) to get the location of the model
                location = Vector3.TransformCoordinate(new Vector3(0, 0, 0), combinedTransform.Value.ToSharpDX());
            }

            return location;
        }

        public BoundingBox GetBoundingBox()
        {

            // Create a new bounding box
            var boundingBox = new SharpDX.BoundingBox();

            // Iterate over the geometries and expand the bounding box
            foreach (var config in batchedMeshes)
            {
                config.Geometry.UpdateBounds();
                var box = config.Geometry.Bound;
                boundingBox = SharpDX.BoundingBox.Merge(boundingBox, box);
            }
            return boundingBox;
        }


        public HelixToolkit.Wpf.SharpDX.Material SetMaterial()
        {
            return null;
        }
        public async Task Import()
        {
            await OpenFile();
        }

        public void Translate(Vector3 translation)
        {
            transform.Children.Add(new TranslateTransform3D(translation.X, translation.Y, translation.Z));

        }

        public void Select()
        {
            throw new NotImplementedException();
        }

        public void Deselect()
        {
            throw new NotImplementedException();
        }
    }
}
