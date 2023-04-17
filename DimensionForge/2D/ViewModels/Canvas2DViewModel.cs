using DimensionForge._2D.interfaces;
using DimensionForge._2D.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;
using HelixToolkit.Wpf;
using DimensionForge._2D.verlet;
using Newtonsoft.Json;
using System.Linq;
using System.Windows.Xps;

namespace DimensionForge._2D.ViewModels
{
    public partial class Canvas2DViewModel : ObservableObject
    {

        public Canvas canvas { get; set; }

        [ObservableProperty]
        ObservableCollection<IShape2D> shapes = new();

        Random random = new Random();
        public Canvas2DViewModel()
        {

        }



        [RelayCommand]
        [property: JsonIgnore]
       async void DrawParticle()
        {

            DrawCircle();

            var particle = Shapes[0];

            particle.Position = new Point(100, 100);
            particle.OldPosition = new Point(95, 95);


            for (int i = 0; i < 100; i++)
            {
               
                var vx = particle.Position - particle.OldPosition;

                particle.OldPosition = particle.Position;

                particle.Position += vx;

                if (particle.Position.X > this.canvas.Width )
                {
                    var width = this.canvas.Width;
                    particle.Position = new Point(width,particle.Position.Y);
                    particle.OldPosition = new Point(particle.Position.X + vx.X,particle.OldPosition.Y);
                }

                await Task.Delay(100);

            }



        }


        void Update()
        {

        }

        void UpdatePoints(Circle2D cirle)
        {
            for (int i = 0; i < shapes.Count(); i++)
            {
                var shape = shapes[i];

                var vx = shape.Position - shape.OldPosition;

                shape.OldPosition = shape.Position;

                shape.Position += vx;
                
            }
        }
       





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
        public void CanvasLoaded(Canvas canvasElement)
        {
            this.canvas = canvasElement;    
            
        }



        [RelayCommand]
        [property: JsonIgnore]
        void DrawCircle()
        {
            var circle = new Circle2D();
            circle.Position = GetRandomPosition();
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
           // rectangle.Position =  GetRandomPosition();
            Shapes.Add(rectangle);
        }

        System.Windows.Media.Color GetRandomColor()
        {
            Random random = new Random();
            byte red = (byte)random.Next(256);
            byte green = (byte)random.Next(256);
            byte blue = (byte)random.Next(256);
            return System.Windows.Media.Color.FromRgb(red, green, blue);
        }


        Point GetRandomPosition()
        {
            return new Point(random.NextDouble() * 800f, random.NextDouble() * 600f);
        }


        float GetRandomFloat()
        {
            return (float)random.NextDouble() * 300;
        }


    }
}
