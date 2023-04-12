using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    }
}
