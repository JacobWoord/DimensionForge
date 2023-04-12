using DimensionForge._2D.interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._2D.Models
{
    public partial class Shape2D : ObservableObject, IShape2D
    {

        private Guid id;  
        public Guid Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        private Point position;
        public Point Position
        {
            get => position;
            set => SetProperty(ref position, value);
        }


        private Color color;
        public Color FillColor
        {
            get => color;
            set => SetProperty(ref color, value);
        }
        public Color StrokeColor { get ; set; }
        public float StrokeThickness { get ; set ; }

        public Shape2D()
        {
            
            id = Guid.NewGuid();
             

        }


    }
}
