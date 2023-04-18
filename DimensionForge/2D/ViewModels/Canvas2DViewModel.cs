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
using SharpDX.Direct3D11;
using SharpDX;
using HelixToolkit.Wpf.SharpDX;
using Point = System.Windows.Point;
using HelixToolkit.SharpDX.Core;

namespace DimensionForge._2D.ViewModels
{
    public partial class Canvas2DViewModel : ObservableObject
    {

        [ObservableProperty]
        float width = 1400;

        [ObservableProperty]
        float _height = 750;


        [ObservableProperty]
        ObservableCollection<IShape2D> shapes = new();

        Random random = new Random();
        public Canvas2DViewModel()
        {
           

        }

        void Update_physics(float dt)
        {

        }




        void Find_collision(IShape2D shape1, IShape2D shape2)
        {

            var gravity = new Vector2(0, 0.2f);
            var friction = 0.999f;

            var position1 = shape1.Position.ToVector2();
            var position2 = shape2.Position.ToVector2();
            
            var oldPosition1 = shape1.OldPosition.ToVector2();
            var oldPosition2 = shape2.OldPosition.ToVector2();

            var velocity1 = (position1 - oldPosition1) * friction;
            var velocity2 = (position1 - oldPosition1) * friction;

            bool collide = false;
            float dist = Vector2.Distance(position1,position2);
            Vector2 dir1 = Vector2.Normalize(position2 - position1);
            Vector2 dir2 = Vector2.Normalize(position1 - position2);
            float radiusSum = (shape1 as Circle2D).Diameter / 2 + (shape2 as Circle2D).Diameter / 2 ;

            if (dist < radiusSum)
            {
                var colission_axis = position1 - position2;
                //var n = (colission_axis / dist).ToPoint();
                //float delta = 100f - dist;

                //shape1.Position = new Point(shape1.Position.X + 0.5 * delta * n.X, shape1.Position.Y + 0.5f * delta * n.Y);
               // shape2.Position = new Point(shape2.Position.X - 0.5 * delta * n.X, shape1.Position.Y - 0.5f * delta * n.Y);

                var translation1 = dir1 * (radiusSum - dist);
                var translation2 = -dir2 * (radiusSum - dist);

                var pos1 = new Point(shape1.Position.X - translation1.X, shape1.Position.Y - translation1.Y).ToVector2() + gravity;
                var pos2 = new Point(shape2.Position.X + translation1.X, shape2.Position.Y + translation1.Y).ToVector2() + gravity;
                shape1.Position = pos1.ToPoint();
                shape2.Position = pos2.ToPoint();



                //shape1.Position = new Point(shape2.Position.X - translation2.X, shape2.Position.Y - translation2.Y);
                //shape2.Position = new Point(shape2.Position.X + translation2.X, shape2.Position.Y + translation2.Y);
                shape1.OldPosition = new Point(shape1.Position.X - 1, shape1.Position.Y - 2);
                shape2.OldPosition = new Point(shape2.Position.X - 1, shape2.Position.Y - 2);

                //collide = true;

            }



        }

        async Task VerletLoop()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    foreach (var shape in shapes)
                    {
                        var currentShape = shape;
                        var bounce = 0.9f;
                        var gravity = new Vector2(0, 0.2f);
                        var friction = 0.999f;
                        var radius = (shape as Circle2D).Diameter / 2f;
                        var position = shape.Position.ToVector2();
                        var oldPosition = shape.OldPosition.ToVector2();
                        var velocity = (position - oldPosition) * friction;
                        oldPosition = position;
                        position += velocity;
                        position += gravity;

                        var height = Height - (radius+ 10);
                        var width = Width - (radius+ 10);

                        if (position.X > width - radius)
                        {
                            width = width - radius;
                            position = new Vector2(width, position.Y);
                            oldPosition += new Vector2(velocity.X * bounce, 0);
                        }
                        else if (position.X < radius)
                        {
                            position = new Vector2(radius, position.Y);
                            oldPosition += new Vector2(velocity.X * bounce, 0);
                        }
                        else if (position.Y > height - radius)
                        {
                            position = new Vector2(position.X, height - radius);
                            oldPosition += new Vector2(0, velocity.Y * bounce);
                        }
                        else if (position.Y < radius)
                        {
                            position = new Vector2(position.X, radius);
                            oldPosition += new Vector2(0, velocity.Y * bounce);
                        }

                        shape.Position = position.ToPoint();
                        shape.OldPosition = oldPosition.ToPoint();
                        // check for collisions with other shapes
                        foreach (var otherShape in shapes.Where(s => s != currentShape))
                        {                       
                                Find_collision(shape, otherShape); 
                           
                        }

                    }

                    await Task.Delay(16);
                }
            });
        }









        [RelayCommand]
        [property: JsonIgnore]
        async Task DrawCircle()
        {

          
                IShape2D circle = new Circle2D();
                circle.Position = GetRandomPosition();
                circle.OldPosition = new Point(circle.Position.X - 2, circle.Position.Y - 2);
                circle.FillColor = GetRandomColor();
                (circle as Circle2D).Diameter = 80f;//GetRandomFloat();
                Shapes.Add(circle);

          

        }




        void Update()
        {

        }




      

        [RelayCommand]
        [property: JsonIgnore]
        async void ChangePosition()
        {
            for (int i = 0; i < 100; i++)
            {
                foreach (var shape in shapes)
                {
                    // Get a random position within the bounds of the canvas
                    var randomX = random.NextDouble() * width;
                    var randomY = random.NextDouble() * _height;

                    // Clamp the x and y positions to ensure they are within the bounds of the canvas
                    var clampedX = (float)Math.Clamp(randomX, 0, width);
                    var clampedY = (float)Math.Clamp(randomY, 0, _height);

                    // Set the new position of the shape within the canvas bounds
                    shape.Position = new System.Windows.Point(clampedX, clampedY);

                    await Task.Delay(500);
                }

            }
        }


        [RelayCommand]
        [property: JsonIgnore]
        public async Task CanvasLoaded()
        {

            await VerletLoop();

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
            return (float)random.Next(50, 100);
        }


    }
}
