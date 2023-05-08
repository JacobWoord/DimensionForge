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
using Color = SharpDX.Color;
using HelixToolkit.Wpf.SharpDX;
using Quaternion = SharpDX.Quaternion;
using DimensionForge.HelperTools;
using DimensionForge.Common;
using Vector3 = SharpDX.Vector3;
using Matrix = SharpDX.Matrix;

using System.Windows.Media;
using SharpDX.Direct3D11;
using Net_Designer_MVVM;
using System.Windows.Data;

namespace DimensionForge._3D.Models
{

    public partial class BatchedModel3D : ObservableObject, IShape3D
    {

        public BoundingBox BoundingBox { get; set; }
        public List<Node3D> Bbcorners { get; set; } = new();


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
            Nodes = new List<Node3D>();
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
        public List<Node3D> Nodes { get; set; }
        public UseCase UseCase { get; set; }

        [ObservableProperty]
        [property: JsonIgnore]
        Transform3DGroup transform;

        private TranslateTransform3D translateTransform;

        //Translation method for the 3D Model this method is generating a new tranformGroup 
        public Transform3DGroup CreateTransformGroup(Vector3 translation, float scale, SharpDX.Quaternion rotation)
        {
            // Convert SharpDX types to WPF types
            System.Windows.Media.Media3D.Vector3D wpfTranslation = new System.Windows.Media.Media3D.Vector3D(translation.X, translation.Y, translation.Z);
            System.Windows.Media.Media3D.Quaternion wpfQuaternion = new System.Windows.Media.Media3D.Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);

            // Create the translation transform
            TranslateTransform3D translationTransform = new TranslateTransform3D(wpfTranslation);

            // Create the scaling transform
            ScaleTransform3D scalingTransform = new ScaleTransform3D(scale, scale, scale);


            // Create the rotation transform
            RotateTransform3D rotationTransform = new RotateTransform3D(new QuaternionRotation3D(wpfQuaternion));

            // Combine the transformations
            Transform3DGroup transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(translationTransform);
            transformGroup.Children.Add(rotationTransform);
            transformGroup.Children.Add(scalingTransform);

            return transformGroup;
        }



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


            Nodes.ForEach(n => n.Position += dif);
            Bbcorners.ForEach(n => n.Position += dif);

        }
        public void TranslateToCenter(Vector3 newlocation)
        {
            var bb = this.GetBoundingBox();
            var oldcenter = bb.Minimum + (bb.Maximum - bb.Minimum) / 2;
            var newcenter = newlocation + (bb.Maximum - bb.Minimum) / 2;
            var dif = newcenter - oldcenter;
            var trans = new TranslateTransform3D(dif.ToVector3D());

            Transform.Dispatcher.Invoke(() =>
            {
                Transform.Children.Add(trans);
            });

            Location = newlocation;

            foreach (var node in Bbcorners)
            {
                node.Position += dif;
            }
        }
        public virtual async Task TranslateToAsync(Vector3 newlocation)
        {
            var dif = -Location + newlocation;
            var trans = new TranslateTransform3D(dif.ToVector3D());
            await Transform.Dispatcher.InvokeAsync(() =>
            {
                Transform.Children.Add(trans);
            });

            Location = newlocation;


            Bbcorners.ForEach(x => x.Position += dif);
        }
        public virtual async Task TranslateAsync(Vector3 translation)
        {
            Location += translation;
            await Transform.Dispatcher.InvokeAsync(() =>
            {
                Transform.Children.Add(new TranslateTransform3D(translation.ToVector3D()));
            });
            Bbcorners.ForEach(x => x.Position += translation);
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


            for (int i = 0; i < Nodes.Count; i++)
            {
                var node = Nodes[i];
                var transformedPoint = trans.Transform(new Point3D(node.Position.X, node.Position.Y, node.Position.Z));
                Nodes[i] = new Node3D(new Vector3((float)transformedPoint.X, (float)transformedPoint.Y, (float)transformedPoint.Z)) { NodePos = node.NodePos, Color = node.Color };
            }

            for (int i = 0; i < Bbcorners.Count; i++)
            {
                var node = Bbcorners[i];
                var transformedPoint = trans.Transform(new Point3D(node.Position.X, node.Position.Y, node.Position.Z));
                Bbcorners[i] = new Node3D(new Vector3((float)transformedPoint.X, (float)transformedPoint.Y, (float)transformedPoint.Z)) { NodePos = node.NodePos, Color = node.Color };
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

            Nodes.ForEach(n => n.Position *= (float)scaleFactor);


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



        //TRANSFORMDATAS
        public void ConvertTransform3DGroupToTransformData()
        {
            // Initialize default values
            Vector3D translation = new Vector3D(0, 0, 0);
            Vector3D scale = new Vector3D(1, 1, 1);
            Quaternion rotation = new Quaternion(0, 0, 0, 1);


            //// Iterate through the children of the Transform3DGroup and extract the relevant data
            //if (transform is not null)
            //    foreach (var transform in transform.Children)
            //    {
            //        if (transform is TranslateTransform3D translateTransform)
            //        {
            //            translation = new Vector3D(translateTransform.OffsetX, translateTransform.OffsetY, translateTransform.OffsetZ);
            //        }
            //        else if (transform is ScaleTransform3D scaleTransform)
            //        {
            //            scale = new Vector3D(scaleTransform.ScaleX, scaleTransform.ScaleY, scaleTransform.ScaleZ);
            //        }
            //        else if (transform is RotateTransform3D rotateTransform)
            //        {
            //            if (rotateTransform.Rotation is AxisAngleRotation3D axisAngleRotation)
            //            {
            //                Vector3D axis = new Vector3D(
            //                    axisAngleRotation.Axis.X,
            //                    axisAngleRotation.Axis.Y,
            //                    axisAngleRotation.Axis.Z);
            //                double angle = axisAngleRotation.Angle;
            //                rotation = new Quaternion(
            //                    axis.X * Math.Sin(angle / 2),
            //                    axis.Y * Math.Sin(angle / 2),
            //                    axis.Z * Math.Sin(angle / 2),
            //                    Math.Cos(angle / 2));
            //            }

            //        }
            //        TransformDatas.Add(new TransformData(translation, scale, rotation));
            //    }

        }
        public void ConvertTransformDataToTransform3DGroup()
        {
            //Transform3DGroup transformGroup = new Transform3DGroup();

            //foreach (var data in TransformDatas)
            //{
            //    // Add the translation transformation
            //    TranslateTransform3D translation = new TranslateTransform3D(
            //        data.Translation.X,
            //        data.Translation.Y,
            //        data.Translation.Z);
            //    transformGroup.Children.Add(translation);

            //    // Add the scale transformation
            //    ScaleTransform3D scale = new ScaleTransform3D(
            //        data.Scale.X,
            //        data.Scale.Y,
            //        data.Scale.Z);
            //    transformGroup.Children.Add(scale);

            //    // Add the rotation transformation
            //  //  Quaternion rotation = data.Rotation;
            //    Vector3D axis = new Vector3D(rotation.X, rotation.Y, rotation.Z);
            //    double angle = 2 * Math.Acos(rotation.W);
            //    AxisAngleRotation3D axisAngleRotation = new AxisAngleRotation3D(axis, angle);
            //    RotateTransform3D rotateTransform = new RotateTransform3D(axisAngleRotation);
            //    transformGroup.Children.Add(rotateTransform);
            //}
            //transform = transformGroup;

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
            foreach (var item in anchors)
            {
                Nodes.Add(new Node3D(item.Location));
            }


            this.ScaleModel(0.01);




            //creates the opposite anchorPoints baes on the existing nodes on the positive Y axis
            var oppositeAnchors = Nodes.GroupBy(p => p.Position.Y)
                                     .Where(g => g.Count() >= 2)
                                     .SelectMany(g => g.ToList())
                                     .ToList();
            oppositeAnchors.ForEach(node => Nodes.Add(new Node3D(new Vector3(node.Position.X, node.Position.Y + 15f, node.Position.Z)) { Color = node.Color }));

            //Take the 3 positions on the positive Y axis
            var positiveGroupLookUp = Nodes.Where(p => p.Position.Y > 0)
                           .GroupBy(p => p.Position.Y)
                           .Where(g => g.Count() == 3)
                           .OrderByDescending(g => g.First().Position.Z)
                           .ToList();

            var posGroup = positiveGroupLookUp[0].ToList();




            //Take the 3 positions on the negative Y axis
            var negativeGroupLookUp = Nodes.Where(p => p.Position.Y <= 0)
                                          .GroupBy(p => p.Position.Y)
                                          .Where(g => g.Count() == 3)
                                          .OrderByDescending(g => g.First().Position.Z)
                                          .ToList();
            var negGroup = negativeGroupLookUp[0].ToList();


            // set the associated nodes for the positive positions on Y axis
            for (int i = 0; i < posGroup.Count(); i++)
            {
                NodePosition cornerName = (NodePosition)i + 3;

                (posGroup[i] as Node3D).NodePos = cornerName;
            }


            // set the associated nodes for the negative positions on Y axis
            for (int i = 0; i < negGroup.Count(); i++)
            {
                NodePosition cornerName = (NodePosition)i;

                (negGroup[i] as Node3D).NodePos = cornerName;


            }

            Nodes.Add(new Node3D(GetModelCenter()) { NodePos = NodePosition.MiddleCenter });


            //calculate the node position for the middle top node
            var rt = Nodes.FirstOrDefault(x => x.NodePos == NodePosition.RightTop);
            var lt = Nodes.FirstOrDefault(x => x.NodePos == NodePosition.LeftTop);
            var middle = (rt.Position + lt.Position) / 2;

            Nodes.Add(new Node3D(new Vector3(middle.X + 4, middle.Y, middle.Z)) { NodePos = NodePosition.MiddleTop });

            //calculate the node position for the middle bottom node
            var bl = Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft);
            var br = Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomRight);
            var middleBottom = (bl.Position + br.Position) / 2;
            Nodes.Add(new Node3D(new Vector3(middleBottom.X + 4, middleBottom.Y, middleBottom.Z)) { NodePos = NodePosition.MiddleBottom });


        }
        private void SetNodePositions()
        {
            //Take the 3 positions on the positive Y axis
            var positiveGroup = Nodes.Where(p => p.Position.Y > 0)
                           .GroupBy(p => p.Position.Y)
                           .Where(g => g.Count() == 3)
                           .OrderByDescending(g => g.First().Position.Z)
                           .ToList();




            //Take the 3 positions on the negative Y axis
            var negativeGroup = Nodes.Where(p => p.Position.Y <= 0)
                                          .GroupBy(p => p.Position.Y)
                                          .Where(g => g.Count() == 3)
                                          .OrderByDescending(g => g.First().Position.Z)
                                          .ToList();

            for (int i = 0; i < positiveGroup.Count(); i++)
            {
                NodePosition cornerName = (NodePosition)i;

                (positiveGroup[i] as Node3D).NodePos = cornerName;


            }

            for (int i = 0; i < negativeGroup.Count(); i++)
            {
                NodePosition cornerName = (NodePosition)i + 2;

                (negativeGroup[i] as Node3D).NodePos = cornerName;


            }

            //var rt = Nodes.FirstOrDefault(x => x.NodePos == NodePosition.RightTop);
            //var br = Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomRight);
            //var bl = Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft);

            //var hm = (rt.Position - bl.Position) / 2;
            //var wm = (br.Position - bl.Position) / 2;


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

            var bb = this.GetBoundingBox();
            //var bb = this.GetBoundingBox();
            var eightCorners = bb.GetCorners();

            Bbcorners = ConvertBoundingBoxToSquare(eightCorners.ToList());
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

            if (Bbcorners is not null)
            {
                Bbcorners.ForEach(cornerNode => cornerNode.transform = this.transform);
            }
        }

        public Vector3 GetOrientation1()
        {
            //returns the orientation of the model
            var topTrianle = Nodes.Where(n => n.NodePos == NodePosition.LeftTop || n.NodePos == NodePosition.MiddleTop || n.NodePos == NodePosition.RightTop).Select(n => n.Position).ToArray();
            var bottomTriangle = Nodes.Where(n => n.NodePos == NodePosition.BottomLeft || n.NodePos == NodePosition.MiddleBottom || n.NodePos == NodePosition.BottomRight).Select(n => n.Position).ToArray();

            var topCentroid = Utils3D.GetCentroidPosition(topTrianle);
            var bottomCentroid = Utils3D.GetCentroidPosition(bottomTriangle);

            var orientation = bottomCentroid - topCentroid;

            return orientation;
        }
        public Vector3 GetOrientation()
        {
            var topCorners = Bbcorners.Where(n => n.CornerName == CornerName.UpperLeftFrontCorner
            || n.CornerName == CornerName.UpperRightFrontCorner
            || n.CornerName == CornerName.UpperLeftBackCorner
            || n.CornerName == CornerName.UpperRightBackCorner).ToArray();

            var bottomCorners = Bbcorners.Where(n => n.CornerName == CornerName.LowerLeftFrontCorner
           || n.CornerName == CornerName.LowerRightFrontCorner
           || n.CornerName == CornerName.LowerLeftBackCorner
           || n.CornerName == CornerName.LowerRightBackCorner).ToArray();

            var topCornerPositions = topCorners.Select(n => n.Position).ToArray();
            var bottomCornerPositions = bottomCorners.Select(n => n.Position).ToArray();


            var topCentroid = Utils3D.GetCentroidPosition(topCornerPositions);
            var bottomCentroid = Utils3D.GetCentroidPosition(bottomCornerPositions);

            var orientation = topCentroid - bottomCentroid;

            return orientation;
        }

        public Vector3 GetHorizontalOrientation()
        {
            var frontFaceCorners = Bbcorners.Where(n => n.CornerName == CornerName.UpperLeftFrontCorner
          || n.CornerName == CornerName.UpperRightFrontCorner
          || n.CornerName == CornerName.LowerLeftFrontCorner
          || n.CornerName == CornerName.LowerRightFrontCorner).ToArray();

            var backFaceCorners = Bbcorners.Where(n => n.CornerName == CornerName.UpperLeftBackCorner
       || n.CornerName == CornerName.UpperRightBackCorner
       || n.CornerName == CornerName.LowerLeftBackCorner
       || n.CornerName == CornerName.LowerRightBackCorner).ToArray();

            var frontFaceCornersPositions = frontFaceCorners.Select(n => n.Position).ToArray();
            var backFaceCornersPositions = backFaceCorners.Select(n => n.Position).ToArray();

            Vector3 frontCentroid = Utils3D.GetCentroidPosition(frontFaceCornersPositions);
            Vector3 backCentroid = Utils3D.GetCentroidPosition(backFaceCornersPositions);

            var orientation = frontCentroid - backCentroid;

            return orientation;


        }

        public Quaternion rotationQuaternion()
        {
            // Create an array of the bounding box vertices
            Vector3[] boxVertices = Bbcorners.Select(item => item.Position).ToArray();

            // Compute the orientation vector of the bounding box as the vector between the centroids of the top and bottom faces
            Vector3 topCentroid = (boxVertices[0] + boxVertices[1] + boxVertices[3] + boxVertices[4]) / 4f;
            Vector3 bottomCentroid = (boxVertices[5] + boxVertices[6] + boxVertices[7] + boxVertices[8]) / 4f;
            Vector3 orientationVector = bottomCentroid - topCentroid;

            // Normalize the orientation vector to ensure that it has unit length
            orientationVector.Normalize();

            // Compute the angle between the orientation vector and the up direction (assumed to be Vector3.UnitY)
            float angle = (float)Math.Acos(Vector3.Dot(orientationVector, Vector3.UnitY));

            // Compute the axis of rotation by taking the cross product of the orientation vector and Vector3.UnitY
            Vector3 axis = Vector3.Cross(orientationVector, Vector3.UnitY);

            // Create the quaternion that represents the rotation between the two vectors
            Quaternion rotationQuaternion = Quaternion.RotationAxis(axis, angle);

            return rotationQuaternion;
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
            square.Add(new Node3D(new Vector3(minX, minY, minZ)) { NodePos = NodePosition.BottomRight, Color = Color.Red, Pinned = true });
            square.Add(new Node3D(new Vector3(maxX, minY, minZ + halfDepth)) { NodePos = NodePosition.RightTop, Color = Color.Green, Pinned = true });
            square.Add(new Node3D(new Vector3(maxX, maxY, minZ + halfDepth)) { NodePos = NodePosition.LeftTop, Color = Color.Yellow, Pinned = true });
            square.Add(new Node3D(new Vector3(minX, maxY, minZ)) { NodePos = NodePosition.BottomLeft, Color = Color.Purple, Pinned = true });

            // Calculate the center point of the square
            // Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);

            // Add a center node to the list
            //square.Add(new Node3D(center) { NodePos = NodePosition.Center, Color = Color.White });

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
                    SharpDX.Vector4 position4 = Vector3.Transform(geometry.Positions[i], combinedTransform.Value.ToSharpDX());
                    Vector3 position = new Vector3(position4.X, position4.Y, position4.Z);

                    min = Vector3.Min(min, position);
                    max = Vector3.Max(max, position);
                }
            }

            return new BoundingBox(min, max);
        }
        public Node3D GetNode(NodePosition nodeposition)
        {
            return Nodes.FirstOrDefault(x => x.NodePos == nodeposition);
        }


        public List<verletElement3D> GetElements()
        {
            var elements = new List<verletElement3D>();
            var bbNodes = new List<Node3D>();

            var lt = Nodes.FirstOrDefault(n => n.NodePos == NodePosition.LeftTop);
            var lm = Nodes.FirstOrDefault(n => n.NodePos == NodePosition.LeftMiddle);
            var bl = Nodes.FirstOrDefault(n => n.NodePos == NodePosition.BottomLeft);
            var rt = Nodes.FirstOrDefault(n => n.NodePos == NodePosition.RightTop);
            var rm = Nodes.FirstOrDefault(n => n.NodePos == NodePosition.RightMiddle);
            var br = Nodes.FirstOrDefault(n => n.NodePos == NodePosition.BottomRight);
            var mt = Nodes.FirstOrDefault(n => n.NodePos == NodePosition.MiddleTop);
            var mb = Nodes.FirstOrDefault(n => n.NodePos == NodePosition.MiddleBottom);
            var cen = Nodes.FirstOrDefault(n => n.NodePos == NodePosition.MiddleCenter);


            //new nodes for the verlet elements
            bbNodes.Add(new Node3D(lt.Position) { NodePos = lt.NodePos, Pinned = true });//0
            bbNodes.Add(new Node3D(lm.Position) { NodePos = lm.NodePos, Pinned = true });//1
            bbNodes.Add(new Node3D(bl.Position) { NodePos = bl.NodePos, Pinned = true });//2
            bbNodes.Add(new Node3D(rt.Position) { NodePos = rt.NodePos, Pinned = true });//3
            bbNodes.Add(new Node3D(rm.Position) { NodePos = rm.NodePos, Pinned = true });//4
            bbNodes.Add(new Node3D(br.Position) { NodePos = br.NodePos, Pinned = true });//5          
            bbNodes.Add(new Node3D((mt.Position)) { NodePos = mt.NodePos, Pinned = true });//6
            bbNodes.Add(new Node3D((cen.Position)) { NodePos = NodePosition.MiddleCenter, Pinned = true });//7
            bbNodes.Add(new Node3D((mb.Position)) { NodePos = mb.NodePos, Pinned = true });//8




            // TOP TRIANGLE
            elements.Add(new verletElement3D() { Start = bbNodes[0], End = bbNodes[6] });
            elements.Add(new verletElement3D() { Start = bbNodes[6], End = bbNodes[3] });
            elements.Add(new verletElement3D() { Start = bbNodes[3], End = bbNodes[0] });

            // MIDDLE TRIANGLE
            elements.Add(new verletElement3D() { Start = bbNodes[1], End = bbNodes[7] });
            elements.Add(new verletElement3D() { Start = bbNodes[7], End = bbNodes[4] });
            elements.Add(new verletElement3D() { Start = bbNodes[4], End = bbNodes[1] });

            // BOTTOM TRIANGLE
            elements.Add(new verletElement3D() { Start = bbNodes[2], End = bbNodes[8] });
            elements.Add(new verletElement3D() { Start = bbNodes[8], End = bbNodes[5] });
            elements.Add(new verletElement3D() { Start = bbNodes[5], End = bbNodes[2] });

            //LINEAR SIDES
            elements.Add(new verletElement3D() { Start = bbNodes[6], End = bbNodes[7] });
            elements.Add(new verletElement3D() { Start = bbNodes[7], End = bbNodes[8] });
            elements.Add(new verletElement3D() { Start = bbNodes[3], End = bbNodes[4] });
            elements.Add(new verletElement3D() { Start = bbNodes[4], End = bbNodes[5] });
            elements.Add(new verletElement3D() { Start = bbNodes[0], End = bbNodes[1] });
            elements.Add(new verletElement3D() { Start = bbNodes[1], End = bbNodes[2] });

            //DIAGONALS
            elements.Add(new verletElement3D() { Start = bbNodes[0], End = bbNodes[4] });
            elements.Add(new verletElement3D() { Start = bbNodes[4], End = bbNodes[2] });
            elements.Add(new verletElement3D() { Start = bbNodes[6], End = bbNodes[4] });
            elements.Add(new verletElement3D() { Start = bbNodes[6], End = bbNodes[1] });
            elements.Add(new verletElement3D() { Start = bbNodes[1], End = bbNodes[8] });
            elements.Add(new verletElement3D() { Start = bbNodes[8], End = bbNodes[4] });

            elements.Add(new verletElement3D() { Start = bbNodes[1], End = bbNodes[5] });
            elements.Add(new verletElement3D() { Start = bbNodes[3], End = bbNodes[1] });
            elements.Add(new verletElement3D() { Start = bbNodes[3], End = bbNodes[7] });
            elements.Add(new verletElement3D() { Start = bbNodes[0], End = bbNodes[7] });
            elements.Add(new verletElement3D() { Start = bbNodes[7], End = bbNodes[2] });
            elements.Add(new verletElement3D() { Start = bbNodes[7], End = bbNodes[5] });




            return elements;
        }

        public Vector3[] GetTriangle(string side)
        {
            var top = Nodes.Where(n => n.NodePos == NodePosition.LeftTop || n.NodePos == NodePosition.MiddleTop || n.NodePos == NodePosition.RightTop).Select(n => n.Position).ToArray();
            var bottom = Nodes.Where(n => n.NodePos == NodePosition.BottomLeft || n.NodePos == NodePosition.MiddleBottom || n.NodePos == NodePosition.BottomRight).Select(n => n.Position).ToArray();


            switch (side)
            {
                case "top":
                    return top;
                    break;
                case "bottom":
                    return bottom;
                    break;
                default:
                    return top;
                    break;
            }
        }
        public void SetBoundingBox()
        {
            var bb = this.GetBoundingBox();
            BoundingBox = bb;
            var corners = bb.GetCorners().ToList();


            var nodeCorners = new List<Node3D>();


            // giving the corespondending names for the nodes 
            for (int i = 0; i < corners.Count(); i++)
            {
                CornerName cornerName = (CornerName)i;
                nodeCorners.Add(new Node3D(corners[i]) { CornerName = cornerName });
                Debug.WriteLine(corners[i]);
            }

            Bbcorners = nodeCorners;
        }
        public List<verletElement3D> GetBbElements()
        {
            var bb = this.GetBoundingBox();
            var corners = bb.GetCorners().ToList();
            var nodeCorners = new List<Node3D>();




            // giving the corespondending names for the nodes 
            for (int i = 0; i < corners.Count(); i++)
            {
                CornerName cornerName = (CornerName)i;
                nodeCorners.Add(new Node3D(corners[i]) { CornerName = cornerName });
                Debug.WriteLine(corners[i]);
            }



            var elements = new List<verletElement3D>();

            //Sides
            elements.Add(new verletElement3D() { Start = nodeCorners[0], End = nodeCorners[1] });
            elements.Add(new verletElement3D() { Start = nodeCorners[0], End = nodeCorners[3] });
            elements.Add(new verletElement3D() { Start = nodeCorners[0], End = nodeCorners[4] });
            elements.Add(new verletElement3D() { Start = nodeCorners[1], End = nodeCorners[5] });
            elements.Add(new verletElement3D() { Start = nodeCorners[1], End = nodeCorners[2] });
            elements.Add(new verletElement3D() { Start = nodeCorners[2], End = nodeCorners[3] });
            elements.Add(new verletElement3D() { Start = nodeCorners[2], End = nodeCorners[6] });
            elements.Add(new verletElement3D() { Start = nodeCorners[3], End = nodeCorners[7] });
            elements.Add(new verletElement3D() { Start = nodeCorners[4], End = nodeCorners[7] });
            elements.Add(new verletElement3D() { Start = nodeCorners[4], End = nodeCorners[5] });
            elements.Add(new verletElement3D() { Start = nodeCorners[5], End = nodeCorners[6] });
            elements.Add(new verletElement3D() { Start = nodeCorners[6], End = nodeCorners[7] });

            //Diagonal
            elements.Add(new verletElement3D() { Start = nodeCorners[1], End = nodeCorners[6] });
            elements.Add(new verletElement3D() { Start = nodeCorners[2], End = nodeCorners[5] });
            elements.Add(new verletElement3D() { Start = nodeCorners[0], End = nodeCorners[7] });
            elements.Add(new verletElement3D() { Start = nodeCorners[3], End = nodeCorners[4] });
            elements.Add(new verletElement3D() { Start = nodeCorners[3], End = nodeCorners[6] });
            elements.Add(new verletElement3D() { Start = nodeCorners[2], End = nodeCorners[7] });
            elements.Add(new verletElement3D() { Start = nodeCorners[0], End = nodeCorners[5] });
            elements.Add(new verletElement3D() { Start = nodeCorners[1], End = nodeCorners[4] });
            elements.Add(new verletElement3D() { Start = nodeCorners[0], End = nodeCorners[2] });
            elements.Add(new verletElement3D() { Start = nodeCorners[1], End = nodeCorners[3] });
            elements.Add(new verletElement3D() { Start = nodeCorners[4], End = nodeCorners[6] });
            elements.Add(new verletElement3D() { Start = nodeCorners[5], End = nodeCorners[7] });

            return elements;
        }
    }
}




