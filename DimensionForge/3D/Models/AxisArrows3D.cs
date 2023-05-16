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
        private Vector3 EndPoint { get; set; }
        private Vector3 startPoint { get; set; }
        public UseCase UseCase { get; set; }    
    
       





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

            float length = 4f;

            Vector3 direction = Position.Position - CenterPosition.Position;
            direction.Normalize();
            EndPoint = CenterPosition.Position + direction * length;
            startPoint = CenterPosition.Position - direction * length;
           
        }
         






        public override void Draw()
        {
            CreateEndPoint();
            MeshBuilder meshBuilder = new MeshBuilder();
            Color = SharpDX.Color.Red;
            meshBuilder.AddArrow(startPoint, EndPoint, 0.06);
            Geometry =  meshBuilder.ToMesh();
        }
            




      
   
    }
}
