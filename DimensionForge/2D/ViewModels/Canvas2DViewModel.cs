using DimensionForge._2D.interfaces;
using DimensionForge._2D.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._2D.ViewModels
{
    public partial class Canvas2DViewModel : ObservableObject
    {

        public Canvas2DViewModel() 
        { 

            DrawCircle();

        }
        


        [ObservableProperty]
        ObservableCollection<Shape2D> shapes = new(); 



        void DrawCircle()
        {
            var circle = new Circle2D();
            circle.Position = new System.Drawing.Point(50, 50);
            circle.FillColor = System.Drawing.Color.FromArgb(255, 255, 0, 0);
            circle.Diameter = 100f;
            Shapes.Add(circle);
        }
    }
}
