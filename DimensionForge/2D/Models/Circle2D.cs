using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DimensionForge._2D.Models
{  
    public partial class Circle2D : Shape2D
    {
        [ObservableProperty]
        float diameter = 300f;
   
        public Circle2D()
        {

        }

        //public Rect GetBounds()
        //{
        //    // Calculate the bounds of the ellipse based on its position, width, and height
        //    double left = Position.X - Width / 2;
        //    double top = Position.Y - Height / 2;
        //    double right = Position.X + Width / 2;
        //    double bottom = Position.Y + Height / 2;

        //    return new Rect(left, top, Width, Height);
        //}

    }
}
