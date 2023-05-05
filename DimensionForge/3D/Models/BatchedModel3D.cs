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


            this.ScaleModel(0.1);
            this.ScaleModel(0.1);


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

            if (Bbcorners is not null)
            {
                Bbcorners.ForEach(cornerNode => cornerNode.transform = this.transform);
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

        public Vector3 GetCentroid()
        {
            var vertices = Nodes.Where(x => x.NodePos != NodePosition.None).ToList();
            var centroid = new Vector3();
            foreach (var vertex in vertices)
            {
                centroid += vertex.Position;
            }

            centroid /= vertices.Count;

            return centroid;
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


        public Transform3DGroup CalculateFullTransformationMatrix(Vector3[] newVertices, Vector3 centroid, Transform3DGroup transformGroup)
        {
            // Create a matrix to transform the centroid using the current Transform3DGroup
            var centroidTransformMatrix = Matrix.Identity;
            if (transformGroup != null)
            {
                foreach (Transform3D transform in transformGroup.Children)
                {
                    if (transform is MatrixTransform3D matrixTransform)
                    {
                        centroidTransformMatrix = matrixTransform.Matrix.ToSharpDX();
                    }
                    else if (transform is ScaleTransform3D scaleTransform)
                    {
                        var scaleMatrix = Matrix.Scaling((float)scaleTransform.ScaleX, (float)scaleTransform.ScaleY, (float)scaleTransform.ScaleZ);
                        centroidTransformMatrix *= scaleMatrix;
                    }
                    else if (transform is RotateTransform3D rotateTransform)
                    {
                        var rotation = rotateTransform.Rotation as AxisAngleRotation3D;
                        centroidTransformMatrix *= Matrix.RotationQuaternion(new Quaternion(new Vector3((float)rotation.Axis.X, (float)rotation.Axis.Y, (float)rotation.Axis.Z), (float)rotation.Angle));
                    }
                    else if (transform is TranslateTransform3D translateTransform)
                    {
                        centroidTransformMatrix *= Matrix.Translation(new Vector3((float)translateTransform.OffsetX, (float)translateTransform.OffsetY, (float)translateTransform.OffsetZ));
                    }
                }
            }

            // Transform the centroid using the current transformation matrix
            var transformedCentroid = Vector3.TransformCoordinate(centroid, centroidTransformMatrix);

            // Create a matrix to translate the transformed centroid to the origin
            var translateToOriginMatrix = SharpDX.Matrix.Translation(-transformedCentroid);

            // Create a matrix to transform the original vertices using the Transform3DGroup
            var transformMatrix = centroidTransformMatrix;

            // Create a matrix to transform the vertices to their new positions
            var newVerticesMatrix = new Matrix(
                newVertices[0].X, newVertices[0].Y, newVertices[0].Z, 0,
                newVertices[1].X, newVertices[1].Y, newVertices[1].Z, 0,
                newVertices[2].X, newVertices[2].Y, newVertices[2].Z, 0,
                0, 0, 0, 1);

            // Create a matrix to translate the vertices from the origin to the new centroid
            var translateFromOriginMatrix = Matrix.Translation(transformedCentroid);

            // Combine the matrices to create the full transformation matrix
            var fullTransformMatrix = newVerticesMatrix * translateFromOriginMatrix * transformMatrix * translateToOriginMatrix;

            // Convert the full transformation matrix to a Matrix3D
            var fullTransformMatrix3D = fullTransformMatrix.ToMatrix3D();

            // Create new MatrixTransform3D and TranslateTransform3D based on the full transformation matrix
            var newMatrixTransform = new MatrixTransform3D(fullTransformMatrix3D);
            var newTranslateTransform = new TranslateTransform3D(-transformedCentroid.X, -transformedCentroid.Y, -transformedCentroid.Z);

            // Add the new transforms and the existing transforms (except for scaling) to the new Transform3DGroup
            var newTransformGroup = new Transform3DGroup();
            if (transformGroup != null)
            {
                foreach (Transform3D transform in transformGroup.Children)
                {
                    if (transform is MatrixTransform3D || transform is TranslateTransform3D || transform is ScaleTransform3D)
                    {
                        // Do nothing, we will handle MatrixTransform3D, TranslateTransform3D, and ScaleTransform3D separately.
                    }
                    else
                    {
                        newTransformGroup.Children.Add(transform);
                    }
                }
            }
            newTransformGroup.Children.Add(newMatrixTransform);
            newTransformGroup.Children.Add(newTranslateTransform);

            return newTransformGroup;
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



            // elements.Add(new verletElement3D() { Start = bbNodes[6], End = bbNodes[2] });//0
            //  elements.Add(new verletElement3D() { Start = bbNodes[8], End = bbNodes[0] });//1  














            return elements;
        }
        public void SetBoundingBox()
        {
            var bb = this.GetBoundingBox();
            var corners = bb.GetCorners().ToList();
            var convertedBoundingBox = ConvertBoundingBoxToSquare(corners);

            for (int i = 0; i < convertedBoundingBox.Count(); i++)
            {
                NodePosition nodePosition = (NodePosition)i;
                Bbcorners.Add(new Node3D(corners[i]) { NodePos = nodePosition });
            }
        }
        public List<verletElement3D> GetBbElements()
        {
            var bb = this.GetBoundingBox();
            var corners = bb.GetCorners().ToList();
            var nodeCorners = new List<Node3D>();
            for (int i = 0; i < corners.Count(); i++)
            {
                BbCornerName cornerName = (BbCornerName)i;
                nodeCorners.Add(new Node3D(corners[i]) { CornerName = cornerName });
            }

            //var convertertedBb = ConvertBoundingBoxToSquare(corners);

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




