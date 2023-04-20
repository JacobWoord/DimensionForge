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
using System.Xml;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections;

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

        async Task UpdateVerlet()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    Update_Physics(Shapes);
                    await Task.Delay(32);
                }

            });

        }
        void Update_Physics(ObservableCollection<IShape2D> shapesCopy)
        {
            foreach (IShape2D shape in shapesCopy.ToArray())
            {
                if (shape is Sphere3D)
                    UpdatePositions(shape as Sphere3D);
            }

            for (int i = 0; i < 8; i++)
            {



                foreach (IShape2D shape in shapesCopy.ToArray())
                {
                    if (shape is Line2D l)
                        UpdateSticks(l);
                }



                foreach (IShape2D shape in shapesCopy.ToArray())
                {
                    if (shape is Sphere3D l && Shapes.Count() > 1)
                        Find_collision(l);
                }


                foreach (IShape2D shape in shapesCopy.ToArray())
                {
                    if (shape is Sphere3D l)
                        UpdateConstrains(l);
                }
            }
        }
        void UpdateConstrains(IShape2D shape)
        {
            if (shape is not Sphere3D)
                return;

            var circle = shape as Sphere3D;
            // to implement the bounce we multiply the OldVelocity by Bounce
            var bounce = 0.7f;
            // to implement gravity we add it to the position.Y after we add velocity
            var gravity = new Vector2(0, 10.0f);
            //multiply the velocity by friction to slow it down
            var friction = 0.900f;
            var radius = circle.Diameter / 2f;
            var position = circle.Position.ToVector2();
            var oldPosition = circle.OldPosition.ToVector2();
            var velocity = (position - oldPosition) * friction;

            // oldPosition = position;
            // position += velocity;
            // position += gravity;

            var height1 = Height - (radius + 10);
            var width1 = Width - (radius + 10);

            //constrain for the right wall
            if (position.X > width1 - radius)
            {
                width1 = width1 - radius;
                position = new Vector2(width1, position.Y);
                oldPosition += new Vector2(velocity.X * bounce, 0);
            }
            //constrain for the left wall
            else if (position.X < radius)
            {
                position = new Vector2(radius, position.Y);
                oldPosition += new Vector2(velocity.X * bounce, 0);
            }
            //constrain for top
            else if (position.Y > height1 - radius)
            {
                position = new Vector2(position.X, height1 - radius);
                oldPosition += new Vector2(0, velocity.Y * bounce);
            }
            //constrain for bottom
            else if (position.Y < radius)
            {
                position = new Vector2(position.X, radius);
                oldPosition += new Vector2(0, velocity.Y * bounce);
            }

            shape.Position = position.ToPoint();
            shape.OldPosition = oldPosition.ToPoint();
        }

        void UpdateSticks(Line2D line)
        {
            //cast the shapes to
            var circle1 = line.Circle1;
            var circle2 = line.Circle2;

            var circle1Vector = circle1.Position.ToVector2();
            var circle2Vector = circle2.Position.ToVector2();


            var stickLength = 200;
            var dx = circle2Vector.X - circle1Vector.X;
            var dy = circle2Vector.Y - circle1Vector.Y;
            var distance = Vector2.Distance(circle1Vector, circle2Vector);
            var differnce = stickLength - distance;
            var percent = differnce / distance / 2;
            var offsetX = dx * percent;
            var offsetY = dy * percent;

            circle1Vector.X -= offsetX;
            circle1Vector.Y -= offsetY;
            circle2Vector.X += offsetX;
            circle2Vector.Y += offsetY;

            line.P0 = new Point(circle1Vector.ToPoint().X + (circle1 as Sphere3D).Diameter / 2, circle1Vector.ToPoint().Y + (circle1 as Sphere3D).Diameter / 2);
            line.P1 = new Point(circle2Vector.ToPoint().X + (circle2 as Sphere3D).Diameter / 2, circle2Vector.ToPoint().Y + (circle2 as Sphere3D).Diameter / 2);

            Vector2 p0new = circle1Vector + circle1.Diameter / 2;
            Vector2 p1new = circle2Vector + circle2.Diameter / 2;

            line.P0 = p0new.ToPoint();
            line.P1 = p1new.ToPoint();


            circle1.Position = circle1Vector.ToPoint();
            circle2.Position = circle2Vector.ToPoint();

        }
        void UpdatePositions(Sphere3D shape)
        {
            var circle = shape;
            // to implement the bounce we multiply the OldVelocity by Bounce
            var bounce = 0.9f;
            // to implement gravity we add it to the position.Y after we add velocity
            var gravity = new Vector2(0, 10.0f);
            //multiply the velocity by friction to slow it down
            var friction = 0.950f;
            var radius = circle.Diameter / 2f;
            var position = circle.Position.ToVector2();
            var oldPosition = circle.OldPosition.ToVector2();
            var velocity = (position - oldPosition) * friction;
            oldPosition = position;
            position += velocity;
            position += gravity;

            var height1 = Height - (radius);
            var width1 = Width - (radius);

            shape.Position = position.ToPoint();
            shape.OldPosition = oldPosition.ToPoint();

        }
        void Find_collision(Shape2D shape)
        {

            Sphere3D shapeToCheck = shape as Sphere3D;
            var mainShapeIndex = Shapes.IndexOf(shapeToCheck);
            var mainPosition = shapeToCheck.Position.ToVector2();
            var mainOldPosition = shapeToCheck.OldPosition.ToVector2();

            bool collide = false;

            for (int i = 0; i < Shapes.Count(); i++)
            {
                //if the itterations is on the shape to check it skips
                if (Shapes[i] == Shapes[mainShapeIndex])
                    continue;
                //if the itterations is on a line it skips
                if (Shapes[i] is Line2D)
                    continue;

                Sphere3D otherShape = Shapes[i] as Sphere3D;

                //convert positions to Vector2 for easier calculations
                var otherPosition = otherShape.Position.ToVector2();
                var otherOldPosition = otherShape.OldPosition.ToVector2();
                //direction for first ball
                Vector2 dir1 = Vector2.Normalize(otherPosition - mainPosition);
                Vector2 dir2 = Vector2.Normalize(mainPosition - otherPosition);
                float radiusSum = shapeToCheck.Diameter / 2 + otherShape.Diameter / 2;
                var colission_axis = mainPosition - otherPosition; //n = collision_axis / 2
                float dist = Vector2.Distance(mainPosition, otherPosition);

                //if the distance between 2 balls is less then the sum of both radius
                if (dist < radiusSum)
                {

                    Vector2 n = colission_axis / dist;
                    //delta is the value that the balls need to move to keep free of eachother 
                    float delta = radiusSum - dist;

                    mainPosition += 0.5f * delta * n;
                    otherPosition -= 0.5f * delta * n;

                    shapeToCheck.Position = mainPosition.ToPoint();
                    otherShape.Position = otherPosition.ToPoint();
                }
            }
        }

        Sphere3D CreateBall()
        {
            var p1 = new Sphere3D();
            p1.Position = GetRandomPosition();

            (p1 as Shape2D).OldPosition = new Point(p1.Position.X - 5, p1.Position.Y - 2);
            p1.FillColor = GetRandomColor();

            p1.Diameter = 50;

            Shapes.Add(p1);

            return p1;
        }
        async Task InfiniteLoop()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    //this way is infinite
                }

            });
        }

        [RelayCommand]
        void SpawnStick()
        {
            //balls for the ends the lines
            var p1 = CreateBall();
            var p2 = new Sphere3D();
            //bool to check if the collection contains a stick allready  
            var containsStick = Shapes.OfType<Line2D>().Any();
            //if the collection contains a stick it takes the linkes ball as second point 
            if (containsStick)
            {
                var l = Shapes.FirstOrDefault(x => x is Line2D);
                var line = l as Line2D;
                p2 = line.Circle1;
            }
            else
            {
                p2 = CreateBall();
            }

            var l1 = new Line2D(p1, p2);
            Shapes.Add(l1);
        }

        [RelayCommand]
        [property: JsonIgnore]
        async Task SpawnBalls()
        {

            //30 balls will appear in the canvas when finish
            int i = 0;
            while (i < 100)
            {
                IShape2D circle = new Sphere3D();
                circle.Position = GetRandomPosition();
                circle.OldPosition = new Point(circle.Position.X - 5, circle.Position.Y - 5);
                circle.FillColor = GetRandomColor();
                (circle as Sphere3D).Diameter = 100;
                Shapes.Add(circle);

                i++;
                await Task.Delay(64);
            }


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
                    var randomX = random.NextDouble() * Width;
                    var randomY = random.NextDouble() * _height;

                    // Clamp the x and y positions to ensure they are within the bounds of the canvas
                    var clampedX = (float)Math.Clamp(randomX, 0, Width);
                    var clampedY = (float)Math.Clamp(randomY, 0, _height);

                    // Set the new position of the shape within the canvas bounds
                    shape.Position = new System.Windows.Point(clampedX, clampedY);

                    await Task.Delay(500);
                }

            }
        }

        [RelayCommand]
        [property: JsonIgnore]
        async Task CanvasLoaded()
        {
            await UpdateVerlet();
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
            return (float)random.Next(10, 50);
        }


    }
}




