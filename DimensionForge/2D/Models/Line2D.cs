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

        public Line2D()
        {
            
        }


    }
}
