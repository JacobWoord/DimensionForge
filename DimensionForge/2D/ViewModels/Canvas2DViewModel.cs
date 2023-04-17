using DimensionForge._2D.interfaces;
using DimensionForge._2D.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Threading.Tasks;

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

        Random random = new Random();

        [RelayCommand]
        [property: JsonIgnore]
        async void ChangePosition()
        {
            var canvasWidth = 800;
            var canvasHeight = 600;

            for (int i = 0; i < 100; i++)
            {
                foreach (var shape in shapes)
                {
                    // Get a random position within the bounds of the canvas
                    var randomX = random.NextDouble() * canvasWidth;
                    var randomY = random.NextDouble() * canvasHeight;

                    // Clamp the x and y positions to ensure they are within the bounds of the canvas
                    var clampedX = (float)Math.Clamp(randomX, 0, canvasWidth);
                    var clampedY = (float)Math.Clamp(randomY, 0, canvasHeight);

                    // Set the new position of the shape within the canvas bounds
                    shape.Position = new System.Windows.Point(clampedX, clampedY);

                    await Task.Delay(500);
                }

            }
        }



        [RelayCommand]
        [property: JsonIgnore]
        void DrawCircle()
        {
            var circle = new Circle2D();
            circle.Position = GetRandomosition();
            circle.FillColor = GetRandomColor();
            circle.Diameter = GetRandomFloat();
            Shapes.Add(circle);

        }

        void DrawRectangle()
        {
            var rectangle = new Rectangle2D();
            rectangle.Width = 100;
            rectangle.Height = 100;
            rectangle.FillColor = GetRandomColor(); 
            rectangle.Position =  GetRandomosition();
            Shapes.Add(rectangle);
        }

        Color GetRandomColor()
        {
            Random random = new Random();
            byte red = (byte)random.Next(256);
            byte green = (byte)random.Next(256);
            byte blue = (byte)random.Next(256);
            return Color.FromRgb(red, green, blue);
        }


        Point GetRandomosition()
        {
            return new Point (random.NextDouble() * 800f, random.NextDouble() * 600f);
        }

        float GetRandomFloat()
        {
            return (float)random.NextDouble() * 300;
        }


    }
}
