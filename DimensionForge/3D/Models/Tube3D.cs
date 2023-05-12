using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System;

namespace DimensionForge._3D.Models
{
    public partial class Tube3D : Shape3D
    {

        [ObservableProperty]
        [property: JsonProperty]
        List<Node3D> nodeList = new();
        public Vector3[] path { get; set; }

        [ObservableProperty]
        double diameter = 0.5;

        [ObservableProperty]
        int thetaDiv = 30;
        public Tube3D(List<Node3D> path)
        {
            Material = PhongMaterials.Green;
            
            NodeList = path;

           
            
            Name = "Tube";
            Draw();
        }

        public Tube3D(Vector3 start ,Vector3 end)
        {
            Material = PhongMaterials.Green;

            nodeList.Add(new Node3D(start));
            nodeList.Add(new Node3D(end));

            Name = "Tube";
            Draw();
        }
        public override List<VerletElement3D> GetElements()
        {
            return base.GetElements();
        }



        private void rotate(Vector3[] vectors)
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = new Vector3(vectors[i].Z, vectors[i].Y, vectors[i].X);

            }
        }


        public override void Draw()
        {
            IList<Vector3> centerPoints = new List<Vector3>();
            foreach (Node3D node in nodeList)
            {
                centerPoints.Add(node.Position);
            }

            MeshBuilder meshBuilder = new MeshBuilder();
            Geometry = meshBuilder.ToMeshGeometry3D();
            meshBuilder.AddTube(centerPoints, Diameter, 10, true);
            Geometry = meshBuilder.ToMeshGeometry3D();
        }




    }
}
