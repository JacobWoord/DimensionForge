using DimensionForge.Common;
using DimensionForge.HelperTools;
using HelixToolkit.Wpf.SharpDX;
using System.Collections.Generic;
using System.Dynamic;
using MeshGeometry3D = HelixToolkit.SharpDX.Core.MeshGeometry3D;
using Color = SharpDX.Color;
using SharpDX;
using System.Linq;
using System.Diagnostics;
using System;

namespace DimensionForge._3D.Models
{
    public partial class ObjModel3D : Shape3D
    {
        public string FileName { get; set; }

        public float Height { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }
      

        public List<Node3D> BoundingPositions { get; set; }


        public ObjModel3D(MeshGeometry3D meshGeomtry3D)
        {
            Geometry = meshGeomtry3D;
            Color = SharpDX.Color.Blue;
            InitModel();
        }


        [RelayCommand]
        public void RotateModelOnCenters(VerletBuildResult buildResult)
        {
            var model = this;

            var verletCenter = buildResult.GetCenter(CornerName.ModelCenter, this.Id);

            // Save the positions of the verletbox
            var translation = new Vector3(verletCenter.Position.X, verletCenter.Position.Y, verletCenter.Position.Z);
            // Translate the red verletbox to the origin
            buildResult.Nodes.ForEach(node => node.Position -= translation);


            //Creates Temporary nodes and bring them to the origin
            var temporaryModelBoundings = new List<Node3D>();
            model.BoundingPositions.ForEach(x => temporaryModelBoundings.Add(new Node3D(new Vector3(x.Position.X, x.Position.Y, x.Position.Z)) { CornerName = x.CornerName }));
            ObjHelperClass.UpdateTemporaryNodes(model, buildResult.GetCenter(CornerName.ModelCenter, this.Id).Position, temporaryModelBoundings);

            var rotationList = new List<CornerName>();
            rotationList.Add(CornerName.LeftPlaneCenter);
            rotationList.Add(CornerName.FrontPlaneCenter);
            rotationList.Add(CornerName.TopPlaneCenter);



            foreach (var cornerName in rotationList)
            {

                var axis1 = temporaryModelBoundings.FirstOrDefault(x => x.CornerName == cornerName).Position;
                axis1.Normalize();

                var axis2 = buildResult.GetCenter(cornerName, this.Id).Position;
                axis2.Normalize();

                var rotationAxis = Vector3.Cross(axis2, axis1);
                rotationAxis.Normalize();

                var angle = MathF.Acos(Vector3.Dot(axis1, axis2)) * -1;

                if (MathF.Abs(angle) > 0.01)
                {

                    var rotationQuaternion = Quaternion.RotationAxis(rotationAxis, angle);
                    // Apply the rotation to each vertex position in the geometry
                    for (int i = 0; i < model.Geometry.Positions.Count; i++)
                    {
                        model.Geometry.Positions[i] = Vector3.Transform(model.Geometry.Positions[i], rotationQuaternion);
                    }

                    // Apply the rotation to each bounding position in the model
                    for (int i = 0; i < model.BoundingPositions.Count; i++)
                    {
                        model.BoundingPositions[i].Position = Vector3.Transform(model.BoundingPositions[i].Position, rotationQuaternion);
                    }
                    for (int i = 0; i < temporaryModelBoundings.Count; i++)
                    {
                        temporaryModelBoundings[i].Position = Vector3.Transform(temporaryModelBoundings[i].Position, rotationQuaternion);
                    }

                }
            }

            // Update the positions of the model
            buildResult.Nodes.ForEach(node => node.Position += translation);
            ObjHelperClass.UpdatePosition(model, buildResult.GetCenter(CornerName.ModelCenter, this.Id).Position);


        }




        private void InitModel()
        {

            Material = SetMaterial();
            var size = ObjHelperClass.GetSize(this);

            Height = size.X;
            Width = size.Y;
            Depth = size.Z;

            //This method creates dynamic bounding positions. including the centers
            BoundingPositions = ObjHelperClass.GetBoundingPositions(this);

        }


        public Node3D GetCenter(CornerName centerName)
        {
            // Define the center you want to retrieve from the functions as a parameter

            Node3D center;

            // Computes the centers before returning it
         

            switch (centerName)
            {
                case CornerName.FrontPlaneCenter:
                    center = BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.FrontPlaneCenter);
                    break;
                case CornerName.BackPlaneCenter:
                    center = BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.BackPlaneCenter);

                    break;
                case CornerName.LeftPlaneCenter:
                    center = BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.LeftPlaneCenter);

                    break;
                case CornerName.RightPlaneCenter:
                    center = BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.RightPlaneCenter);

                    break;
                case CornerName.TopPlaneCenter:
                    center = BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.TopPlaneCenter);

                    break;
                case CornerName.BottomPlaneCenter:
                    center = BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.BottomPlaneCenter);
                    break;
                case CornerName.ModelCenter:
                    center = BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.ModelCenter);

                    break;
                default: return null;

            }

            return center;
        }







        public Vector3 GetCentroid(List<Node3D> nodes)
        {
            Vector3 centerOfMass = Vector3.Zero;
            int nodeCount = nodes.Count;

            if (nodeCount == 0)
                return centerOfMass;

            foreach (Node3D node in nodes)
            {
                centerOfMass += node.Position;
            }

            centerOfMass /= nodeCount;
            return centerOfMass;
        }


        public List<Cylinder3D> GetBoundingElements()
        {

            //these are just to rendere the boundingbox in the screeen

            var elements = new List<Cylinder3D>();

            var bottomFrontLeft = BoundingPositions[(int)CornerName.BottomFrontLeft];
            var bottomFrontRight = BoundingPositions[(int)CornerName.BottomFrontRight];
            var bottomBackLeft = BoundingPositions[(int)CornerName.BottomBackLeft];
            var topFrontLeft = BoundingPositions[(int)CornerName.TopFrontLeft];
            var topFrontRight = BoundingPositions[(int)CornerName.TopFrontRight];
            var topBackLeft = BoundingPositions[(int)CornerName.TopBackLeft];
            var bottomBackRight = BoundingPositions[(int)CornerName.BottomBackRight];
            var topBackRight = BoundingPositions[(int)CornerName.TopBackRight];


            // Sides
            elements.Add(new Cylinder3D() { Start = bottomFrontLeft, End = bottomFrontRight, Color = SharpDX.Color.Blue });
            elements.Add(new Cylinder3D() { Start = topFrontLeft, End = topFrontRight, Color = SharpDX.Color.Blue });

            elements.Add(new Cylinder3D() { Start = bottomBackLeft, End = bottomBackRight, Color = SharpDX.Color.Blue });
            elements.Add(new Cylinder3D() { Start = topBackLeft, End = topBackRight, Color = SharpDX.Color.Blue });
            elements.Add(new Cylinder3D() { Start = bottomFrontLeft, End = topFrontLeft, Color = SharpDX.Color.Blue });

            elements.Add(new Cylinder3D() { Start = bottomFrontRight, End = topFrontRight, Color = SharpDX.Color.Blue });
            elements.Add(new Cylinder3D() { Start = bottomBackLeft, End = topBackLeft, Color = SharpDX.Color.Blue });
            elements.Add(new Cylinder3D() { Start = bottomBackRight, End = topBackRight, Color = SharpDX.Color.Blue });
            elements.Add(new Cylinder3D() { Start = topFrontRight, End = topBackLeft, Color = SharpDX.Color.Blue });
            elements.Add(new Cylinder3D() { Start = topFrontLeft, End = topBackRight, Color = SharpDX.Color.Blue });
            elements.Add(new Cylinder3D() { Start = bottomFrontLeft, End = bottomBackRight, Color = SharpDX.Color.Blue });
            elements.Add(new Cylinder3D() { Start = bottomFrontRight, End = bottomBackLeft, Color = SharpDX.Color.Blue });

            // Diagonals on each plane side
            // Front Plane
            elements.Add(new Cylinder3D() { Start = bottomFrontLeft, End = topFrontRight, Color = SharpDX.Color.Transparent });
            elements.Add(new Cylinder3D() { Start = topFrontLeft, End = bottomFrontRight, Color = SharpDX.Color.Transparent });

            // Back Plane
            elements.Add(new Cylinder3D() { Start = bottomBackLeft, End = topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new Cylinder3D() { Start = topBackLeft, End = bottomBackRight, Color = SharpDX.Color.Transparent });

            // Left Plane
            elements.Add(new Cylinder3D() { Start = bottomFrontLeft, End = topBackLeft, Color = SharpDX.Color.Transparent });
            elements.Add(new Cylinder3D() { Start = topFrontLeft, End = bottomBackLeft, Color = SharpDX.Color.Transparent });

            // Right Plane
            elements.Add(new Cylinder3D() { Start = bottomFrontRight, End = topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new Cylinder3D() { Start = topFrontRight, End = bottomBackRight, Color = SharpDX.Color.Transparent });

            // Top Plane
            elements.Add(new Cylinder3D() { Start = topFrontRight, End = topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new Cylinder3D() { Start = topFrontLeft, End = topBackLeft, Color = SharpDX.Color.Transparent });
            elements.Add(new Cylinder3D() { Start = bottomFrontRight, End = bottomBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new Cylinder3D() { Start = bottomFrontLeft, End = bottomBackLeft, Color = SharpDX.Color.Transparent });

            // Bottom Plane


            //Cross Diagonals inside the rectangular box
            elements.Add(new Cylinder3D() { Start = bottomFrontLeft, End = topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new Cylinder3D() { Start = topFrontLeft, End = bottomBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new Cylinder3D() { Start = bottomFrontRight, End = topBackLeft, Color = SharpDX.Color.Transparent });
            elements.Add(new Cylinder3D() { Start = topFrontRight, End = bottomBackLeft, Color = SharpDX.Color.Transparent });

            return elements;
        }





        public List<VerletElement3D> GetVerletElements(string modelId)
        {
            var elements = new List<VerletElement3D>();

            var bottomFrontLeft = new Node3D(BoundingPositions[(int)CornerName.BottomFrontLeft].Position) { CornerName = CornerName.BottomFrontLeft, ModelId = modelId };
            var bottomFrontRight = new Node3D(BoundingPositions[(int)CornerName.BottomFrontRight].Position) { CornerName = CornerName.BottomFrontRight, ModelId = modelId };
            var bottomBackLeft = new Node3D(BoundingPositions[(int)CornerName.BottomBackLeft].Position) { CornerName = CornerName.BottomBackLeft, ModelId = modelId };
            var topFrontLeft = new Node3D(BoundingPositions[(int)CornerName.TopFrontLeft].Position) { CornerName = CornerName.TopFrontLeft, ModelId = modelId };
            var topFrontRight = new Node3D(BoundingPositions[(int)CornerName.TopFrontRight].Position) { CornerName = CornerName.TopFrontRight , ModelId = modelId };
            var topBackLeft = new Node3D(BoundingPositions[(int)CornerName.TopBackLeft].Position) { CornerName = CornerName.TopBackLeft, ModelId = modelId };
            var bottomBackRight = new Node3D(BoundingPositions[(int)CornerName.BottomBackRight].Position) { CornerName = CornerName.BottomBackRight , ModelId = modelId };
            var topBackRight = new Node3D(BoundingPositions[(int)CornerName.TopBackRight].Position) { CornerName = CornerName.TopBackRight, ModelId = modelId };


            // Sides
            elements.Add(new VerletElement3D() { Start = bottomFrontLeft, End = bottomFrontRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = topFrontLeft, End = topFrontRight, Color = SharpDX.Color.Red });

            elements.Add(new VerletElement3D() { Start = bottomBackLeft, End = bottomBackRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = topBackLeft, End = topBackRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = bottomFrontLeft, End = topFrontLeft, Color = SharpDX.Color.Red });

            elements.Add(new VerletElement3D() { Start = bottomFrontRight, End = topFrontRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = bottomBackLeft, End = topBackLeft, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = bottomBackRight, End = topBackRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = topFrontRight, End = topBackLeft, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = topFrontLeft, End = topBackRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = bottomFrontLeft, End = bottomBackRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = bottomFrontRight, End = bottomBackLeft, Color = SharpDX.Color.Yellow });

            // Diagonals on each plane side
            // Front Plane
            elements.Add(new VerletElement3D() { Start = bottomFrontLeft, End = topFrontRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = topFrontLeft, End = bottomFrontRight, Color = SharpDX.Color.Transparent });

            // Back Plane
            elements.Add(new VerletElement3D() { Start = bottomBackLeft, End = topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = topBackLeft, End = bottomBackRight, Color = SharpDX.Color.Transparent });

            // Left Plane
            elements.Add(new VerletElement3D() { Start = bottomFrontLeft, End = topBackLeft, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = topFrontLeft, End = bottomBackLeft, Color = SharpDX.Color.Transparent });

            // Right Plane
            elements.Add(new VerletElement3D() { Start = bottomFrontRight, End = topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = topFrontRight, End = bottomBackRight, Color = SharpDX.Color.Transparent });

            // Top Plane
            elements.Add(new VerletElement3D() { Start = topFrontRight, End = topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = topFrontLeft, End = topBackLeft, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = bottomFrontRight, End = bottomBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = bottomFrontLeft, End = bottomBackLeft, Color = SharpDX.Color.Transparent });

            // Bottom Plane


            //Cross Diagonals inside the rectangular box
            elements.Add(new VerletElement3D() { Start = bottomFrontLeft, End = topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = topFrontLeft, End = bottomBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = bottomFrontRight, End = topBackLeft, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = topFrontRight, End = bottomBackLeft, Color = SharpDX.Color.Transparent });

            return elements;
        }













    }
}
