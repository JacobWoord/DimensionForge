using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DimensionForge._2D.Models
{  
    public partial class Circle2D : Shape2D
    {

        private float diameter = 300f;
        public float Diameter
        {
            get => diameter;
            set => SetProperty(ref diameter, value);
        }



        public Circle2D()
        {
            


        }
    }
}
