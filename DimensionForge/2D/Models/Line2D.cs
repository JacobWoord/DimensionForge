using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DimensionForge._2D.Models
{
    public partial class Line2D : Shape2D
    {
        [ObservableProperty]
        Point p0;

        [ObservableProperty]
        Point p1;

        [ObservableProperty]
        int strokeThickness;

        [ObservableProperty]
        string p0ID;

        [ObservableProperty]
        string p1ID;

        [ObservableProperty]
        bool hidden;

        [ObservableProperty]
        Sphere3D circle1;

        [ObservableProperty]
        Sphere3D circle2;

        

        public Line2D(Sphere3D c1, Sphere3D c2)
        {
            circle1 = c1;
            circle2 = c2; 
            
            p0 = circle1.Position;
            p1 = circle2.Position;
        }


    }
}
