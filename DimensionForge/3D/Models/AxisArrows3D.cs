using DimensionForge.Common;
using HelixToolkit.SharpDX.Core;
using SharpDX;
using System;
using System.Collections.Generic;
using Color = SharpDX.Color;
namespace DimensionForge._3D.Models
{
    public partial class AxisArrows3D : Shape3D
    {


        public Node3D Position { get; set; }
        public Node3D CenterPosition { get; set; }
        public AxisDirection Direction { get; set; }
        private Vector3 LineEndPoint { get; set; }
       





        public AxisArrows3D(Node3D center, Node3D position,Color color)
        {
            Position = position;
            CenterPosition = center;  
            CreateEndPoint();
            MeshBuilder meshBuilder = new MeshBuilder();
            Color = color;
            Material = SetMaterial();
        }


        public void CreateEndPoint()
        {

            float length = 1f;

            Vector3 direction = CenterPosition.Position - Position.Position;
            direction.Normalize();

             LineEndPoint = CenterPosition.Position + direction * length;

        }




        public override void Draw()
        {
            CreateEndPoint();
            MeshBuilder meshBuilder = new MeshBuilder();
            Color = SharpDX.Color.Red;
            meshBuilder.AddArrow(CenterPosition.Position, LineEndPoint, 0.06);
            Geometry =  meshBuilder.ToMesh();
        }
            




      
   
    }
}
