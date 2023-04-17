using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DimensionForge._2D.Models
{
    public partial class Rectangle2D :  Shape2D
    {
        private float width = 100;
        public float Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }

        private float height = 100;
        public float Height
        {
            get => height;
            set => SetProperty(ref height, value);
        }

        public Rectangle2D()
        {
            
        }


        public Rect GetBounds()
        {
            // Calculate the bounds of the ellipse based on its position, width, and height
            double left = Position.X - Width / 2;
            double top = Position.Y - Height / 2;
            double right = Position.X + Width / 2;
            double bottom = Position.Y + Height / 2;

            return new Rect(left, top, Width, Height);
        }

    }
}
