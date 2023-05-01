using DimensionForge._3D.interfaces;
using HelixToolkit.SharpDX.Core;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._3D.Models
{
    public partial class verletElement3D : IElement3D
    {
        private Node3D start;
        public Node3D Start
        {
            get { return start; }
            set
            {
                start = value;
                GetElementLength();
            }
        }
        private Node3D end;
        public Node3D End
        {
            get { return end; }
            set
            {
                end = value;
                GetElementLength();
            }
        }
        public float Length { get; set; } = 0;

        public float radius = 0.8f;




        public verletElement3D()
        {


        }

        private void GetElementLength()
        {

            if (Start != null && End != null)
            {
                Length = Vector3.Distance(Start.Position, End.Position);

            }

        }




    }
}

