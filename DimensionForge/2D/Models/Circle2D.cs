using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DimensionForge._2D.Models
{  
    public partial class Sphere3D : Shape2D
    {
        [ObservableProperty]
        float diameter = 300f;

        [ObservableProperty]
        Point centerPosition;
   
        public Sphere3D()
        {

        }

       

    }
}
