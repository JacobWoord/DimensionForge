using DimensionForge._2D.interfaces;
using DimensionForge._2D.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DimensionForge._2D.ViewModels
{
    public partial class Canvas2DViewModel : ObservableObject
    {
        public Canvas2DViewModel()
        {
          //  DrawRectangle();
        }
        [ObservableProperty]
        ObservableCollection<IShape2D> shapes = new();



        //[RelayCommand]
        //public void DrawCircle1()
        //{
        //    var circle = new Circle2D();

        //    // Generate random X and Y positions for the circle
        //    var rand = new Random();
        //    var canvasWidth = canvas.ActualWidth;
        //    var canvasHeight = canvas.ActualHeight;
        //    circle.Position = new Point(rand.NextDouble() * canvasWidth, rand.NextDouble() * canvasHeight);

        //    // Generate random color for the circle
        //    circle.FillColor = System.Windows.Media.Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256));

        //    circle.Diameter = 300f;
        //    Shapes.Add(circle);
        //}



        [RelayCommand]
        void DrawCircle()
        {
            var circle = new Circle2D();
            circle.Position = new Point(0, 0);
            circle.FillColor = System.Windows.Media.Color.FromRgb(255, 255, 0);
            circle.Diameter = 300f;
            Shapes.Add(circle);

        }

        void DrawRectangle()
        {
            var rectangle = new Rectangle2D();
            rectangle.Width = 100;
            rectangle.Height = 100;
            rectangle.FillColor = Color.FromRgb(255, 255, 0);
            rectangle.Position = new Point(50, 50);
            Shapes.Add(rectangle);
        }

    }
}
