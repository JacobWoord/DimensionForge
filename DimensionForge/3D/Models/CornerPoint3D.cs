using DimensionForge._3D.Data;
using DimensionForge._3D.interfaces;
using DimensionForge.Common;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace DimensionForge._3D.Models
{
    public partial class CornerPoint3D : Shape3D
    {
        [ObservableProperty]
        Node3D linkedNode;

        public float Radius { get; set; } = 0.05f;

        public CornerPoint3D()
        {

        }

        public override void Draw()
        {
            MeshBuilder meshbuilder = new MeshBuilder();
           
            
            meshbuilder.AddSphere(linkedNode.Position, Radius, 10, 10);

            //var mesh = meshbuilder.ToMesh();
            Material = SetMaterial();

            Geometry = meshbuilder.ToMesh() ;
    
           // var edgeLengths =   GetEdgeLengths(mesh);
        }



        private Dictionary<Vector3, Node3D> uniqueNodes = new Dictionary<Vector3, Node3D>();

        private Node3D GetOrCreateUniqueNode(Vector3 position)
        {
            if (!uniqueNodes.ContainsKey(position))
            {
                uniqueNodes[position] = new Node3D(position);
            }

            return uniqueNodes[position];
        }




        public List<double> GetEdgeLengths(HelixToolkit.SharpDX.Core.MeshGeometry3D mesh)
        {
            var edgeLengths = new List<double>();
            var visitedEdges = new HashSet<Tuple<int, int>>();

            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    int vertexIndex1 = mesh.TriangleIndices[i + j];
                    int vertexIndex2 = mesh.TriangleIndices[i + (j + 1) % 3];

                    // Ensure each edge is processed only once
                    var edge = Tuple.Create(Math.Min(vertexIndex1, vertexIndex2), Math.Max(vertexIndex1, vertexIndex2));
                    if (!visitedEdges.Contains(edge))
                    {
                        visitedEdges.Add(edge);

                        Vector3 vertex1 = mesh.Positions[vertexIndex1];
                        Vector3 vertex2 = mesh.Positions[vertexIndex2];

                       var edgeLength =  Vector3.Distance(vertex1, vertex2);
                        edgeLengths.Add(edgeLength);
                    }
                }
            }

            return edgeLengths;
        }

        public List<VerletElement3D> GetVerletElements(HelixToolkit.SharpDX.Core.MeshGeometry3D mesh)
        {
            var verletElements = new List<VerletElement3D>();
            var visitedEdges = new HashSet<Tuple<int, int>>();

            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    int vertexIndex1 = mesh.TriangleIndices[i + j];
                    int vertexIndex2 = mesh.TriangleIndices[i + (j + 1) % 3];

                    // Ensure each edge is processed only once
                    var edge = Tuple.Create(Math.Min(vertexIndex1, vertexIndex2), Math.Max(vertexIndex1, vertexIndex2));
                    if (!visitedEdges.Contains(edge))
                    {
                        visitedEdges.Add(edge);

                        Vector3 vertex1 = mesh.Positions[vertexIndex1];
                        Vector3 vertex2 = mesh.Positions[vertexIndex2];

                        // Create a new VerletElement3D for this edge
                        VerletElement3D element = new VerletElement3D
                        {
                            Start = GetOrCreateUniqueNode(vertex1),
                            End = GetOrCreateUniqueNode(vertex2)
                        };
                        verletElements.Add(element);
                    }
                }
            }

            return verletElements;
        }

    

        public List<VerletElement3D> GetVerletElementsThreshold(HelixToolkit.SharpDX.Core.MeshGeometry3D mesh)
        {
            var verletElements = new List<VerletElement3D>();
            var visitedEdges = new HashSet<Tuple<int, int>>();

            float distanceThreshold = 4f; // Adjust this value to control the rigidity

            // Iterate through all the unique pairs of vertices in the mesh
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                for (int j = i + 1; j < mesh.Positions.Count; j++)
                {
                    Vector3 vertex1 = mesh.Positions[i];
                    Vector3 vertex2 = mesh.Positions[j];

                    // Calculate the distance between the two vertices
                    float distance = Vector3.Distance(vertex1, vertex2);

                    // Only create a VerletElement3D if the distance is within the threshold
                    if (distance <= distanceThreshold)
                    {
                        VerletElement3D element = new VerletElement3D
                        {
                            Start = GetOrCreateUniqueNode(vertex1),
                            End = GetOrCreateUniqueNode(vertex2)
                        };
                        verletElements.Add(element);
                    }
                }
            }

            return verletElements;
        }




        private Color SetColor(Node3D node)
        {
            SharpDX.Color color = SharpDX.Color.Purple;

            if (node.NodePos == NodePosition.LeftTop)
            {
                color = SharpDX.Color.Red;
            }
            else if (node.NodePos == NodePosition.RightTop)
            {
                color = SharpDX.Color.Green;
            }
            else if (node.NodePos == NodePosition.BottomLeft)
            {
                color = SharpDX.Color.Blue;
            }
            else if (node.NodePos == NodePosition.BottomRight)
            {
                color= SharpDX.Color.Yellow;
            }


            return color;
        }

        public override List<VerletElement3D> GetElements()
        {
            return base.GetElements();
        }



    }
}
