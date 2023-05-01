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
using DimensionForge._3D.ViewModels;
using DimensionForge.Common;
using System.Windows.Controls;

namespace DimensionForge._3D.Models
{

    public partial class BatchedModel3D : ObservableObject, IShape3D
    {


        public List<Node3D> cornerNodes = new();


        public string ID = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public Vector3 Location { get; set; }
        public VerletData3D VerletData = new VerletData3D();
        public IList<TransformData> TransformDatas { get; set; }
        public BatchedModel3D()
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

        private TranslateTransform3D translateTransform;

        //TRANSLATIONS
        public void TranslateTo(Vector3 newlocation)
        {
            var dif = -Location + newlocation;
            var trans = new TranslateTransform3D(dif.ToVector3D());
            Transform.Dispatcher.Invoke(() =>
            {
                Transform.Children.Add(trans);
            });

            Location = newlocation;
            cornerNodes.ForEach(x => x.Position += dif);
        }
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
        public void Rotate(Vector3D Axis, double Angle)
        {

            if (double.IsNaN(Angle))
            {
                Angle = 0;
            }

            var trans = new RotateTransform3D(
                 new AxisAngleRotation3D(
                     axis: Axis,
                     angle: Angle));

            transform.Children.Add(trans);


            for (int i = 0; i < cornerNodes.Count; i++)
            {
                var node = cornerNodes[i];
                var transformedPoint = trans.Transform(new Point3D(node.Position.X, node.Position.Y, node.Position.Z));
                cornerNodes[i] = new Node3D(new Vector3((float)transformedPoint.X, (float)transformedPoint.Y, (float)transformedPoint.Z)) { NodePos = node.NodePos, Color = node.Color };
            }

        }
        public void ScaleModel(double scaleFactor)
        {
            ScaleTransform3D scaleTransform = new ScaleTransform3D(scaleFactor, scaleFactor, scaleFactor);

            if (transform == null)
            {
                transform = new Transform3DGroup();
            }
            transform.Children.Add(scaleTransform);

            Nodes.ForEach(n => n *= (float)scaleFactor);


        }

        //TRANSFORMDATAS
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

        }

        //IMPORT SERVICE
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
            this.ScaleModel(0.1);
            this.ScaleModel(0.1);

        }
        public async Task Import()
        {
            //    await OpenFile();        
            // var bb = this.GetBoundingBox();
            //  var corners = bb.GetCorners();
            //corners.ToList().ForEach(c => this.cornerNodes.Add(new Node3D(c)));
            //corners.ToList().ForEach(c => this.cornerNodes.Add(new Node3D(c)));
        }



        public HelixToolkit.Wpf.SharpDX.Material SetMaterial()
        {
            return null;
        }
        public void SetCornerNodes()
        {
            VerletData = new VerletData3D();
            var bb = this.GetBoundingBox();
            var eightCorners = bb.GetCorners();


            var fourCorners = ConvertBoundingBoxToSquare(eightCorners.ToList());

            //CornerNodes of the door
            cornerNodes.Add(new Node3D(fourCorners[0].Position) { NodePos = fourCorners[0].NodePos, Color = fourCorners[0].Color, IsDoorNode = true });
            cornerNodes.Add(new Node3D(fourCorners[1].Position) { NodePos = fourCorners[1].NodePos, Color = fourCorners[1].Color, IsDoorNode = true });
            cornerNodes.Add(new Node3D(fourCorners[2].Position) { NodePos = fourCorners[2].NodePos, Color = fourCorners[2].Color, IsDoorNode = true });
            cornerNodes.Add(new Node3D(fourCorners[3].Position) { NodePos = fourCorners[3].NodePos, Color = fourCorners[3].Color, IsDoorNode = true });

            //fourCorners.ForEach(corner => cornerNodes.Add(new Node3D(corner.Position) { NodePos = corner.NodePos}));

        }

       
        public void RotateAroundCenter(Vector3D axis, double angle)
        {
            // Get the center point of the model
            Vector3 center = this.GetModelCenter();

            // Translate the model to the origin
            Transform3D translateToOrigin = new TranslateTransform3D(-center.X, -center.Y, -center.Z);

            // Apply the rotation transform
            Transform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(axis, angle));

            // Translate the model back to its original position
            Transform3D translateBack = new TranslateTransform3D(center.X, center.Y, center.Z);


            Transform.Children.Add(translateToOrigin);
            Transform.Children.Add(rotateTransform);
            Transform.Children.Add(translateBack);

            if (cornerNodes is not null)
            {
                cornerNodes.ForEach(cornerNode => cornerNode.transform = this.transform);
            }


        }
        public void MoveCenterToPosition(Vector3 position)
        {
            // Get the center point of the model
            Vector3 center = GetModelCenter();

            // Calculate the translation vector
            Vector3 translation = position - center;

            // Apply the translation transform
            Transform3D translateTransform = new TranslateTransform3D(translation.X, translation.Y, translation.Z);

            // Combine the transforms
            // Transform3DGroup transform = new Transform3DGroup();
            Transform.Children.Add(translateTransform);

            if (cornerNodes is not null)
            {
                cornerNodes.ForEach(cornerNode => cornerNode.transform = this.transform);
            }
            // Apply the transform to the model

        }
        public void Select()
        {
            throw new NotImplementedException();
        }
        public void Deselect()
        {
            throw new NotImplementedException();
        }
        public void Translate(Vector3 translation)
        {

            transform.Children.Add(new TranslateTransform3D(translation.X, translation.Y, translation.Z));

        }
        public Vector3 GetModelCenter()
        {
            BoundingBox bounds = this.GetBoundingBox();
            Vector3 center = bounds.Center;

            return center;
        }
        public List<Node3D> ConvertBoundingBoxToSquare(List<Vector3> boundingBox)
        {
            // Find the minimum and maximum coordinates in each dimension
            float minX = boundingBox.Min(p => p.X);
            float minY = boundingBox.Min(p => p.Y);
            float minZ = boundingBox.Min(p => p.Z);
            float maxX = boundingBox.Max(p => p.X);
            float maxY = boundingBox.Max(p => p.Y);
            float maxZ = boundingBox.Max(p => p.Z);

            // Determine the half-depth of the square
            float halfDepth = (maxZ - minZ);

            // Calculate the 4 corner points of the square
            List<Node3D> square = new List<Node3D>();
            square.Add(new Node3D(new Vector3(minX, minY, minZ)) { NodePos = NodePosition.BottomLeft, Color = Color.Red });
            square.Add(new Node3D(new Vector3(maxX, minY, minZ + halfDepth)) { NodePos = NodePosition.LeftTop, Color = Color.Green });
            square.Add(new Node3D(new Vector3(maxX, maxY, minZ + halfDepth)) { NodePos = NodePosition.RightTop, Color = Color.Yellow });
            square.Add(new Node3D(new Vector3(minX, maxY, minZ)) { NodePos = NodePosition.BottomRight, Color = Color.Purple });

            // Calculate the center point of the square
            Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);

            // Add a center node to the list
            square.Add(new Node3D(center) { NodePos = NodePosition.Center, Color = Color.White });

            return square;
        }
        public BoundingBox GetBoundingBox()
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (var batchedMesh in batchedMeshes)
            {
                var geometry = batchedMesh.Geometry;
                var transform = batchedMesh.ModelTransform;

                // Combine the model transform and the current transform group
                var combinedTransform = Transform3DHelper.Combine(new MatrixTransform3D(new Matrix3D(transform.M11, transform.M12, transform.M13, transform.M14,
                                                                                            transform.M21, transform.M22, transform.M23, transform.M24,
                                                                                            transform.M31, transform.M32, transform.M33, transform.M34,
                                                                                            0, 0, 0, 1)),
                                                                   this.transform);

                for (int i = 0; i < geometry.Positions.Count; i++)
                {
                    Vector4 position4 = Vector3.Transform(geometry.Positions[i], combinedTransform.Value.ToSharpDX());
                    Vector3 position = new Vector3(position4.X, position4.Y, position4.Z);

                    min = Vector3.Min(min, position);
                    max = Vector3.Max(max, position);
                }
            }

            return new BoundingBox(min, max);
        }

        public  List<verletElement3D> GetElements()
        {
            var elements = new List<verletElement3D>();
           
            elements.Add(new verletElement3D() { Start = new Node3D(new Vector3(cornerNodes[0].Position.X, cornerNodes[0].Position.Y, cornerNodes[0].Position.Z)), End = new Node3D(new Vector3(cornerNodes[1].Position.X, cornerNodes[1].Position.Y, cornerNodes[1].Position.Z)) { IsDoorNode = true } });
            elements.Add(new verletElement3D() { Start = new Node3D(new Vector3(cornerNodes[1].Position.X, cornerNodes[1].Position.Y, cornerNodes[1].Position.Z)), End = new Node3D(new Vector3(cornerNodes[2].Position.X, cornerNodes[2].Position.Y, cornerNodes[2].Position.Z)) { IsDoorNode = true } });
            elements.Add(new verletElement3D() { Start = new Node3D(new Vector3(cornerNodes[2].Position.X, cornerNodes[2].Position.Y, cornerNodes[2].Position.Z)), End = new Node3D(new Vector3(cornerNodes[3].Position.X, cornerNodes[3].Position.Y, cornerNodes[3].Position.Z)) { IsDoorNode = true } });
            elements.Add(new verletElement3D() { Start = new Node3D(new Vector3(cornerNodes[3].Position.X, cornerNodes[3].Position.Y, cornerNodes[3].Position.Z)), End = new Node3D(new Vector3(cornerNodes[0].Position.X, cornerNodes[0].Position.Y, cornerNodes[0].Position.Z)) { IsDoorNode = true } });


            var leftTop = cornerNodes.FirstOrDefault(x => x.NodePos == NodePosition.LeftTop).Position;
            var bottomRight = cornerNodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomRight).Position;
            var rightTop = cornerNodes.FirstOrDefault(x => x.NodePos == NodePosition.RightTop).Position;
            var bottomLeft = cornerNodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft).Position;
            
            elements.Add(new verletElement3D() { Start = new Node3D(new Vector3(leftTop.X, leftTop.Y,leftTop.Z)), End = new Node3D(new Vector3(bottomRight.X,bottomRight.Y,bottomRight.Z))});
            elements.Add(new verletElement3D() { Start = new Node3D(new Vector3(rightTop.X,rightTop.Y,rightTop.Z)), End = new Node3D(new Vector3(bottomLeft.X,bottomLeft.Y,bottomLeft.Z))});    
         

            return elements;
        }
    }
}
