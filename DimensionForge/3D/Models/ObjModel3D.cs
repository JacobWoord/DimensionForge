using DimensionForge.Common;
using DimensionForge.HelperTools;
using HelixToolkit.Wpf.SharpDX;
using System.Collections.Generic;
using System.Dynamic;
using MeshGeometry3D = HelixToolkit.SharpDX.Core.MeshGeometry3D;
using Color = SharpDX.Color;
using SharpDX;
using System.Linq;

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

            switch (centerName)
            {
                case CornerName.FrontPlaneCenter:
                    center =  BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.FrontPlaneCenter);
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

            var bottomFrontLeft = new Node3D(BoundingPositions[(int)CornerName.BottomFrontLeft].Position) { CornerName = CornerName.BottomFrontLeft, Color = SharpDX.Color.Green };
            var bottomFrontRight = new Node3D(BoundingPositions[(int)CornerName.BottomFrontRight].Position) {  CornerName = CornerName.BottomFrontRight, Color = SharpDX.Color.Green };
            var bottomBackLeft = new Node3D(BoundingPositions[(int)CornerName.BottomBackLeft].Position) { CornerName = CornerName.BottomBackLeft, Color = SharpDX.Color.Green };
            var topFrontLeft = new Node3D(BoundingPositions[(int)CornerName.TopFrontLeft].Position) { CornerName = CornerName.TopFrontLeft, Color = SharpDX.Color.Green };
            var topFrontRight = new Node3D(BoundingPositions[(int)CornerName.TopFrontRight].Position) { CornerName = CornerName.TopFrontRight, Color = SharpDX.Color.Green };
            var topBackLeft = new Node3D(BoundingPositions[(int)CornerName.TopBackLeft].Position) { CornerName = CornerName.TopBackLeft, Color = SharpDX.Color.Green };
            var bottomBackRight = new Node3D(BoundingPositions[(int)CornerName.BottomBackRight].Position) { CornerName = CornerName.BottomBackRight, Color = SharpDX.Color.Green };
            var topBackRight = new Node3D(BoundingPositions[(int)CornerName.TopBackRight].Position) { CornerName = CornerName.TopBackRight, Color = SharpDX.Color.Green };

            // Sides
            // Sides
            elements.Add(new VerletElement3D() { Start = bottomFrontLeft, End =bottomFrontRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = topFrontLeft, End = topFrontRight, Color = SharpDX.Color.Red });

            elements.Add(new VerletElement3D() { Start = bottomBackLeft, End =bottomBackRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = topBackLeft, End = topBackRight , Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = bottomBackLeft, End =topBackLeft , Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = bottomBackRight, End = topBackRight, Color = SharpDX.Color.Red });

            elements.Add(new VerletElement3D() { Start = bottomFrontLeft, End =topFrontLeft, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start = bottomFrontRight, End = topFrontRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start =topFrontLeft, End =topBackRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start =topFrontRight, End =topBackLeft, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start =bottomFrontLeft, End =bottomBackRight, Color = SharpDX.Color.Red });
            elements.Add(new VerletElement3D() { Start =bottomFrontRight, End =bottomBackLeft, Color = SharpDX.Color.Red });

            // Diagonals on each plane side
            // Front Plane
            elements.Add(new VerletElement3D() { Start =bottomFrontLeft, End = topFrontRight, Color =  SharpDX.Color.Transparent  });
            elements.Add(new VerletElement3D() { Start = topFrontLeft, End = bottomFrontRight, Color = SharpDX.Color.Transparent });

            // Back Plane
            elements.Add(new VerletElement3D() { Start =bottomBackLeft, End =topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start =topBackLeft, End = bottomBackRight, Color = SharpDX.Color.Transparent });

            // Left Plane
            elements.Add(new VerletElement3D() { Start =bottomFrontLeft, End =topBackLeft, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start =topFrontLeft, End =bottomBackLeft, Color = SharpDX.Color.Transparent });

            // Right Plane
            elements.Add(new VerletElement3D() { Start =bottomFrontRight, End = topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start =topFrontRight, End = bottomBackRight, Color = SharpDX.Color.Transparent });

            elements.Add(new VerletElement3D() { Start = topFrontRight, End = topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = topFrontLeft, End = topBackLeft, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = bottomFrontLeft, End = bottomBackLeft, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = bottomFrontRight, End = bottomBackRight , Color = SharpDX.Color.Transparent });

           //Cross Diagonals inside the rectangular box
            elements.Add(new VerletElement3D() { Start = bottomFrontLeft, End = topBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = topFrontLeft, End = bottomBackRight, Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = bottomFrontRight, End = topBackLeft , Color = SharpDX.Color.Transparent });
            elements.Add(new VerletElement3D() { Start = topFrontRight, End = bottomBackLeft , Color = SharpDX.Color.Transparent });

            return elements;
        }













    }
}
