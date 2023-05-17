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
            UpdateCenterPositions();

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





        private void UpdateCenterPositions()
        {
            try
            {
                var frontCorners = BoundingPositions.Where(x => x.CornerName == CornerName.TopFrontRight
                      || x.CornerName == CornerName.TopFrontLeft
                      || x.CornerName == CornerName.BottomFrontRight
                      || x.CornerName == CornerName.BottomFrontLeft).ToList();
                var frontCenter = GetCentroid(frontCorners);

                if (!float.IsInfinity(frontCenter.X) && !float.IsInfinity(frontCenter.Y) && !float.IsInfinity(frontCenter.Z) &&
                    !float.IsNaN(frontCenter.X) && !float.IsNaN(frontCenter.Y) && !float.IsNaN(frontCenter.Z))
                {
                    BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.FrontPlaneCenter).Position = frontCenter;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


            try
            {
                var backCorners = BoundingPositions.Where(x => x.CornerName == CornerName.TopBackRight
                    || x.CornerName == CornerName.TopBackLeft
                    || x.CornerName == CornerName.BottomBackRight
                    || x.CornerName == CornerName.BottomBackLeft).ToList();
                var backCenter = GetCentroid(backCorners);
                if (!float.IsInfinity(backCenter.X) && !float.IsInfinity(backCenter.Y) && !float.IsInfinity(backCenter.Z) &&
                    !float.IsNaN(backCenter.X) && !float.IsNaN(backCenter.Y) && !float.IsNaN(backCenter.Z))
                {
                    BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.BackPlaneCenter).Position = backCenter;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


            try
            {
                var rightCorners = BoundingPositions.Where(x => x.CornerName == CornerName.TopBackRight
                    || x.CornerName == CornerName.TopFrontLeft
                    || x.CornerName == CornerName.BottomBackRight
                    || x.CornerName == CornerName.BottomFrontLeft).ToList();
                var rightCenter = GetCentroid(rightCorners);

                if (!float.IsInfinity(rightCenter.X) && !float.IsInfinity(rightCenter.Y) && !float.IsInfinity(rightCenter.Z) &&
                   !float.IsNaN(rightCenter.X) && !float.IsNaN(rightCenter.Y) && !float.IsNaN(rightCenter.Z))
                {
                    BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.RightPlaneCenter).Position = rightCenter;
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            try
            {
                var leftCorners = BoundingPositions.Where(x => x.CornerName == CornerName.TopBackLeft
                   || x.CornerName == CornerName.TopFrontRight
                   || x.CornerName == CornerName.BottomBackLeft
                   || x.CornerName == CornerName.BottomFrontRight).ToList();
                var leftCenter = GetCentroid(leftCorners);

                if (!float.IsInfinity(leftCenter.X) && !float.IsInfinity(leftCenter.Y) && !float.IsInfinity(leftCenter.Z) &&
                   !float.IsNaN(leftCenter.X) && !float.IsNaN(leftCenter.Y) && !float.IsNaN(leftCenter.Z))
                {
                    BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.LeftPlaneCenter).Position = leftCenter;
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            try
            {
                var topCorners = BoundingPositions.Where(x => x.CornerName == CornerName.TopBackLeft
                     || x.CornerName == CornerName.TopBackRight
                     || x.CornerName == CornerName.TopFrontLeft
                     || x.CornerName == CornerName.TopFrontRight).ToList();
                var topCenter = GetCentroid(topCorners);

                if (!float.IsInfinity(topCenter.X) && !float.IsInfinity(topCenter.Y) && !float.IsInfinity(topCenter.Z) &&
                  !float.IsNaN(topCenter.X) && !float.IsNaN(topCenter.Y) && !float.IsNaN(topCenter.Z))
                {
                    BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.TopPlaneCenter).Position = topCenter;
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            try
            {
                var bottomCorners = BoundingPositions.Where(x => x.CornerName == CornerName.BottomBackLeft
                    || x.CornerName == CornerName.BottomBackRight
                    || x.CornerName == CornerName.BottomFrontLeft
                    || x.CornerName == CornerName.BottomFrontRight).ToList();
                var bottomCenter = GetCentroid(bottomCorners);

                if (!float.IsInfinity(bottomCenter.X) && !float.IsInfinity(bottomCenter.Y) && !float.IsInfinity(bottomCenter.Z) &&
               !float.IsNaN(bottomCenter.X) && !float.IsNaN(bottomCenter.Y) && !float.IsNaN(bottomCenter.Z))
                {
                    BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.BottomPlaneCenter).Position = bottomCenter;
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            try
            {
                var allCorners = BoundingPositions.Where(x => x.CornerName == CornerName.TopBackRight
            || x.CornerName == CornerName.TopBackLeft
            || x.CornerName == CornerName.BottomBackRight
            || x.CornerName == CornerName.BottomBackLeft
            || x.CornerName == CornerName.TopFrontRight
            || x.CornerName == CornerName.TopFrontLeft
            || x.CornerName == CornerName.BottomFrontLeft
            || x.CornerName == CornerName.BottomFrontRight).ToList();
                var modelCenter = GetCentroid(allCorners);

                if (!float.IsInfinity(modelCenter.X) && !float.IsInfinity(modelCenter.Y) && !float.IsInfinity(modelCenter.Z) &&
              !float.IsNaN(modelCenter.X) && !float.IsNaN(modelCenter.Y) && !float.IsNaN(modelCenter.Z))
                {
                    BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.BottomPlaneCenter).Position = modelCenter;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


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





        public List<VerletElement3D> GetVerletElements()
        {
            var elements = new List<VerletElement3D>();

            var bottomFrontLeft = new Node3D(BoundingPositions[(int)CornerName.BottomFrontLeft].Position) { CornerName = CornerName.BottomFrontLeft };
            var bottomFrontRight = new Node3D(BoundingPositions[(int)CornerName.BottomFrontRight].Position) { CornerName = CornerName.BottomFrontRight };
            var bottomBackLeft = new Node3D(BoundingPositions[(int)CornerName.BottomBackLeft].Position) { CornerName = CornerName.BottomBackLeft };
            var topFrontLeft = new Node3D(BoundingPositions[(int)CornerName.TopFrontLeft].Position) { CornerName = CornerName.TopFrontLeft };
            var topFrontRight = new Node3D(BoundingPositions[(int)CornerName.TopFrontRight].Position) { CornerName = CornerName.TopFrontRight };
            var topBackLeft = new Node3D(BoundingPositions[(int)CornerName.TopBackLeft].Position) { CornerName = CornerName.TopBackLeft };
            var bottomBackRight = new Node3D(BoundingPositions[(int)CornerName.BottomBackRight].Position) { CornerName = CornerName.BottomBackRight };
            var topBackRight = new Node3D(BoundingPositions[(int)CornerName.TopBackRight].Position) { CornerName = CornerName.TopBackRight };


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
