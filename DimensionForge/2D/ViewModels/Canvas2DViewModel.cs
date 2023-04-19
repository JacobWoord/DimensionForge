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

        // (temp fields need to find other approach) Store IDs of the circles
        public string Circle1ID { get; set; }
        public string Circle2ID { get; set; }


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
            foreach (IShape2D shape in shapesCopy.ToList())
            {
                if (shape is Circle2D)
                    UpdatePositions(shape as Circle2D);
            }

            for (int i = 0; i < 5; i++)
            {

                foreach (IShape2D shape in shapesCopy.ToList())
                {
                    if (shape is Circle2D l && Shapes.Count() > 1)
                        Find_collision(l);

                }


                foreach (IShape2D shape in shapesCopy.ToList())
                {
                    if (shape is Line2D l)
                        UpdateSticks(l);
                }



                foreach (IShape2D shape in shapesCopy.ToList())
                {
                    if (shape is Circle2D l)
                        UpdateConstrains(l);

                }

            }






        }

        void UpdateConstrains(IShape2D shape)
        {
            if (shape is not Circle2D)
                return;



            var circle = shape as Circle2D;
            // to implement the bounce we multiply the OldVelocity by Bounce
            var bounce = 0.7f;
            // to implement gravity we add it to the position.Y after we add velocity
            var gravity = new Vector2(0, 0.5f);
            //multiply the velocity by friction to slow it down
            var friction = 0.990f;
            var radius = circle.Diameter / 2f;
            var position = circle.Position.ToVector2();
            var oldPosition = circle.OldPosition.ToVector2();
            var velocity = (position - oldPosition) * friction;
            //oldPosition = position;
            //position += velocity;
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
            var circle1 = shapes.First(x => x.ID == line.P0ID);
            var circle2 = shapes.First(x => x.ID == line.P1ID);

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

            line.P0 = new Point(circle1Vector.ToPoint().X + (circle1 as Circle2D).Diameter / 2, circle1Vector.ToPoint().Y + (circle1 as Circle2D).Diameter / 2);
            line.P1 = new Point(circle2Vector.ToPoint().X + (circle2 as Circle2D).Diameter / 2, circle2Vector.ToPoint().Y + (circle2 as Circle2D).Diameter / 2);

            circle1.Position = circle1Vector.ToPoint();
            circle2.Position = circle2Vector.ToPoint();

        }
        void UpdatePositions(Circle2D shape)
        {


            var circle = shape;
            // to implement the bounce we multiply the OldVelocity by Bounce
            var bounce = 0.7f;
            // to implement gravity we add it to the position.Y after we add velocity
            var gravity = new Vector2(0, 0.5f);
            //multiply the velocity by friction to slow it down
            var friction = 0.990f;
            var radius = circle.Diameter / 2f;
            var position = circle.Position.ToVector2();
            var oldPosition = circle.OldPosition.ToVector2();
            var velocity = (position - oldPosition) * friction;
            oldPosition = position;
            position += velocity;
            position += gravity;

            var height1 = Height - (radius + 10);
            var width1 = Width - (radius + 10);


            shape.Position = position.ToPoint();
            shape.OldPosition = oldPosition.ToPoint();

        }

        async Task ConstrainPoints(Circle2D shape)
        {


            var circle = shape;
            // to implement the bounce we multiply the OldVelocity by Bounce
            var bounce = 0.7f;
            // to implement gravity we add it to the position.Y after we add velocity
            var gravity = new Vector2(0, 0.5f);
            //multiply the velocity by friction to slow it down
            var friction = 0.990f;
            var radius = circle.Diameter / 2f;
            var position = circle.Position.ToVector2();
            var oldPosition = circle.OldPosition.ToVector2();
            var velocity = (position - oldPosition) * friction;
            oldPosition = position;
            position += velocity;
            position += gravity;

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



        void Find_collision(IShape2D shape1 = null, IShape2D otherShape = null)
        {
            var shape2 = shapes.Where(x => x.ID != shape1.ID).First();
            var gravity = new Vector2(0, 0.5f);
            var friction = 0.999f;
            var bounce = 0.9f;



            var position1 = shape1.Position.ToVector2();
            var position2 = shape2.Position.ToVector2();

            var oldPosition1 = shape1.OldPosition.ToVector2();
            var oldPosition2 = shape2.OldPosition.ToVector2();

            var velocity1 = (position1 - oldPosition1) * friction;
            var velocity2 = (position1 - oldPosition1) * friction;

            bool collide = false;
            float dist = Vector2.Distance(position1, position2);
            Vector2 dir1 = Vector2.Normalize(position2 - position1);
            Vector2 dir2 = Vector2.Normalize(position1 - position2);
            float radiusSum = (shape1 as Circle2D).Diameter / 2 + (shape2 as Circle2D).Diameter / 2;

            if (dist < radiusSum)
            {
                var colission_axis = position1 - position2;

                var translation1 = dir1 * (radiusSum - dist);
                var translation2 = -dir2 * (radiusSum - dist);

                var pos1 = new Point(shape1.Position.X - translation1.X, shape1.Position.Y - translation1.Y).ToVector2();// + gravity;
                var pos2 = new Point(shape2.Position.X + translation1.X, shape2.Position.Y + translation1.Y).ToVector2();// + gravity;

                shape1.Position = pos1.ToPoint();
                shape2.Position = pos2.ToPoint();

                shape1.OldPosition = new Point(shape1.Position.X - 1, shape1.Position.Y - 1);
                shape2.OldPosition = new Point(shape2.Position.X + 1, shape2.Position.Y + 1);

            }



        }

        async Task VerletLoopAndStickUpdate()
        {

            await Task.Run(async () =>
            {
                while (true)
                {
                    for (int s = 0; s < shapes.Count; s++)
                    {
                        var shape = shapes[s];

                        if (shape is Circle2D)
                        {
                            ConstrainPoints(shape as Circle2D);

                            for (int o = 0; o < shapes.Count; o++)
                            {
                                if (o != s && shapes[o] is Circle2D)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {

                                       // Find_collision(shape, shapes[o] as Circle2D);
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < 3; i++)
                    {

                        for (int l = 0; l < shapes.Count; l++)
                        {


                            if (shapes[l] is Line2D line)
                            {
                                //cast the shapes to
                                var circle1 = shapes.First(x => x.ID == line.P0ID);
                                var circle2 = shapes.First(x => x.ID == line.P1ID);

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

                                line.P0 = new Point(circle1Vector.ToPoint().X + (circle1 as Circle2D).Diameter / 2, circle1Vector.ToPoint().Y + (circle1 as Circle2D).Diameter / 2);
                                line.P1 = new Point(circle2Vector.ToPoint().X + (circle2 as Circle2D).Diameter / 2, circle2Vector.ToPoint().Y + (circle2 as Circle2D).Diameter / 2);

                                circle1.Position = circle1Vector.ToPoint();
                                circle2.Position = circle2Vector.ToPoint();


                            }



                        }
                    }
                    await Task.Delay(16);
                }
            });
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
        async Task PartII()
        {
            var p1 = new Circle2D();
            var p2 = new Circle2D();
            var p3 = new Circle2D();
            var p4 = new Circle2D();
            var p5 = new Circle2D();




            p1.Position = GetRandomPosition();
            p2.Position = GetRandomPosition();
            p3.Position = GetRandomPosition();
            //p4.Position = GetRandomPosition();
            // p5.Position = GetRandomPosition();

            (p1 as Shape2D).OldPosition = new Point(p1.Position.X - 5, p1.Position.Y - 2);
            (p2 as Shape2D).OldPosition = new Point(p2.Position.X - 5, p2.Position.Y - 2);
            (p3 as Shape2D).OldPosition = new Point(p3.Position.X - 5, p3.Position.Y - 2);
            //(p4 as Shape2D).OldPosition = new Point(p4.Position.X - 5, p4.Position.Y - 2);
            // (p5 as Shape2D).OldPosition =new Point( p5.Position.X - 5, p5.Position.Y - 2);

            p1.FillColor = GetRandomColor();
            p2.FillColor = GetRandomColor();
            p3.FillColor = GetRandomColor();
            // p4.FillColor = GetRandomColor();
            // p5.FillColor = GetRandomColor();
            p1.Diameter = 20;
            p2.Diameter = 20;
            p3.Diameter = 20;
            // p4.Diameter = 20;
            //  p5.Diameter = 20;



            //line 1
            var line = new Line2D();
            line.P0 = new Point(p1.Position.X + p1.Diameter / 2, p1.Position.Y + p1.Diameter / 2);
            line.P1 = new Point(p2.Position.X + p2.Diameter / 2, p2.Position.Y + p2.Diameter / 2);
            line.P0ID = p1.ID;
            line.P1ID = p2.ID;

            //line2
            var line2 = new Line2D();
            line2.P0 = new Point(p2.Position.X + p2.Diameter / 2, p2.Position.Y + p2.Diameter / 2);
            line2.P1 = new Point(p3.Position.X + p3.Diameter / 2, p3.Position.Y + p3.Diameter / 2);
            line2.P0ID = p2.ID;
            line2.P1ID = p3.ID;

            //line3
            var line3 = new Line2D();
            line3.P0 = new Point(p3.Position.X + p3.Diameter / 2, p3.Position.Y + p3.Diameter / 2);
            line3.P1 = new Point(p1.Position.X + p4.Diameter / 2, p1.Position.Y + p1.Diameter / 2);
            line3.P0ID = p3.ID;
            line3.P1ID = p1.ID;

            ////line 4
            //var line4 = new Line2D();
            //line4.P0 = new Point(p4.Position.X + p4.Diameter / 2, p4.Position.Y + p4.Diameter / 2);
            //line4.P1 = new Point(p1.Position.X + p1.Diameter / 2, p1.Position.Y + p1.Diameter / 2);
            //line4.P0ID = p4.ID;
            //line4.P1ID = p1.ID;

            ////line 5
            //var line5 = new Line2D();
            //line5.P0 = new Point(p1.Position.X + p1.Diameter / 2, p1.Position.Y + p1.Diameter / 2);
            //line5.P1 = new Point(p3.Position.X + p3.Diameter / 2, p3.Position.Y + p3.Diameter / 2);
            //line5.P0ID = p1.ID;
            //line5.P1ID = p3.ID;



            Shapes.Add(p1);
            Shapes.Add(p2);
            Shapes.Add(p3);
            //Shapes.Add(p4);
            // Shapes.Add(p5);
            Shapes.Add(line);
            Shapes.Add(line2);
            Shapes.Add(line3);
            // Shapes.Add(line4);
            //Shapes.Add(line5);

        }


        [RelayCommand]
        [property: JsonIgnore]
        async Task SpawnBalls()
        {

            //30 balls will appear in the canvas when finish
            int i = 0;
            while (i < 400)
            {
                IShape2D circle = new Circle2D();
                circle.Position = GetRandomPosition();
                circle.OldPosition = new Point(circle.Position.X - 5, circle.Position.Y - 5);
                circle.FillColor = GetRandomColor();
                (circle as Circle2D).Diameter = 20f;
                Shapes.Add(circle);

                i++;
                await Task.Delay(100);
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
            //await VerletLoopAndStickUpdate();

            await UpdateVerlet();

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




