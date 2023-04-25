using SharpDX;
using System;
using DimensionForge.Common;

namespace DimensionForge._3D.Models
{
    public partial class Node3D : ObservableObject
    {
        
        public float Bounce { get; set; } = 1f;   
        public string Id { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 OldPosition { get; set; }
        public float RadiusInMeters { get; set; } = 0.1f;
        public NodePosition NodePos { get; set; }
        public Color Color { get; set; } 

        public Node3D(Vector3 pos) 
        { 
            OldPosition = pos -5;
            Position = pos;
            Id = Guid.NewGuid().ToString();
        }

        public void ConstrainGround()
        {
            var xv = Position.X - OldPosition.X;  // x velocity
            var yv = Position.Y - OldPosition.Y;  // y velocity
            var zv = Position.Z - OldPosition.Z;  // z velocity

            if (Position.Z < RadiusInMeters)
            {
                Position = new Vector3(Position.X, Position.Y, RadiusInMeters);
                OldPosition = new Vector3(OldPosition.X, OldPosition.Y, Position.Z + zv * Bounce);
            }
        }


    }
}
