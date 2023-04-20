using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace DimensionForge._3D.ViewModels
{

    [Serializable]
    public partial class Canvas3DViewModel : ObservableObject
    {


        [JsonIgnore]
        public HelixToolkit.Wpf.SharpDX.OrthographicCamera Camera { get; set; }

        [JsonIgnore]
        public EffectsManager EffectsManager { get; set; }

        [JsonIgnore]
        public Viewport3DX MyViewPort { get; set; }


        //property to control the content control
        [ObservableProperty]
        ObservableObject selectedToolPanel;



        public Canvas3DViewModel()
        {
            this.EffectsManager = new DefaultEffectsManager();

            // Create and set up the camera
            Camera = new HelixToolkit.Wpf.SharpDX.OrthographicCamera
            {
                Position = new Point3D(0, 0, 10),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 1000,
                NearPlaneDistance = -1000,
                Width = 100,
            };
            Camera.CreateViewMatrix();



            //  SelectedTool = new TransformationsViewModel();

        }

        [RelayCommand]
        async Task UpdateVerlet()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    UpdatePhysics();
                    await Task.Delay(16);
                }
            });
        }

        void UpdatePhysics()
        {
            for (int i = 0; i < shapes.Count(); i++)
            {
                if (shapes[i] is not Models.Sphere3D)
                    continue;


                Application.Current.Dispatcher.Invoke(() =>
                {
                   
                        UpdatePositions(shapes[i]);
                      
                    for (int j = 0; j < 20; j++)
                    {
                        if (shapes.Count() > 2)
                            Find_collision(shapes[i]);
                    }

                        ConstrainGround(shapes[i]);


                    // UpdateConstrains(shapes[i]);
                    Draw();
                });

            }


        }


        void Find_collision(IShape3D shape)
        {

            Sphere3D shapeToCheck = shape as Sphere3D;
            var mainShapeIndex = shapes.IndexOf(shapeToCheck);
            var mainPosition = shapeToCheck.Position;
            var mainOldPosition = shapeToCheck.OldPosition;

            bool collide = false;

            for (int i = 0; i < shapes.Count(); i++)
            {
                //if the itterations is on the shape to check it skips
                if (shapes[i] == shapes[mainShapeIndex] || shapes[i] is Floor3D)
                    continue;
                //if the itterations is on a line it skips


                var otherShape = shapes[i] as Sphere3D;

                //convert positions to Vector2 for easier calculations
                var otherPosition = otherShape.Position;
                var otherOldPosition = otherShape.OldPosition;

                float radiusSum = shapeToCheck.Radius + otherShape.Radius;
                var colission_axis = mainPosition - otherPosition; //n = collision_axis / 2
                float dist = Vector3.Distance(mainPosition, otherPosition);

                //if the distance between 2 balls is less then the sum of both radius
                if (dist < radiusSum)
                {

                    Vector3 n = colission_axis / dist;
                    //delta is the value that the balls need to move to keep free of eachother 
                    float delta = radiusSum - dist;

                    mainPosition += 0.5f * delta * n;
                    otherPosition -= 0.5f * delta * n;

                    shapeToCheck.Position = mainPosition;
                    otherShape.Position = otherPosition;
                   // shapeToCheck.OldPosition = mainPosition -5;
                   // otherShape.OldPosition = mainPosition -5;
                }
            }
        }

        public void ConstrainGround(IShape3D ishape)
        {

            var shape = ishape as Models.Sphere3D;

            var xv = shape.Position.X - shape.OldPosition.X;  // x velocity
            var yv = shape.Position.Y - shape.OldPosition.Y;  // y velocity
            var zv = shape.Position.Z - shape.OldPosition.Z;  // z velocity

            var bounce = 0.8f;

            if (shape.Position.Z < shape.Radius)
            {
                shape.Position = new Vector3(shape.Position.X, shape.Position.Y, shape.Radius);
                shape.OldPosition = new Vector3(shape.OldPosition.X, shape.OldPosition.Y, shape.Position.Z + zv * bounce);
            }
        }

        void UpdatePositions(IShape3D shape)
        {
            var sphere = shape as Models.Sphere3D;
            // to implement the bounce we multiply the OldVelocity by Bounce
            var bounce = 0.2f;
            // to implement gravity we add it to the position.Y after we add velocity
            var gravity = new Vector3(0, 0, -0.81f);
            //multiply the velocity by friction to slow it down
            var friction = 0.950f;
            var radius = sphere.Radius;
            var position = sphere.Position;
            var oldPosition = sphere.OldPosition;
            var velocity = (position - oldPosition) * friction;
            oldPosition = position;
            position += velocity;
            position += gravity;



            sphere.Position = position;
            sphere.OldPosition = oldPosition;

        }

        void UpdateConstrains(IShape3D shape)
        {
            if (shape is not Sphere3D)
                return;

            var sphere = shape as Models.Sphere3D;
            // to implement the bounce we multiply the OldVelocity by Bounce
            var bounce = 0.7f;
            // to implement gravity we add it to the position.Y after we add velocity
            var gravity = new Vector2(0, 10.0f);
            //multiply the velocity by friction to slow it down
            var friction = 0.900f;
            var radius = sphere.Radius;
            var position = sphere.Position;
            var oldPosition = sphere.OldPosition;
            var velocity = (position - oldPosition) * friction;

            // oldPosition = position;
            // position += velocity;
            // position += gravity;

            var height1 = 40 - (radius);
            var width1 = 50 - (radius);

            //constrain for the right wall
            if (position.X > width1 - radius)
            {
                width1 = width1 - radius;
                position = new Vector3(width1, position.Y, position.Z);
                oldPosition += new Vector3(velocity.X * bounce, 0, 0);
            }
            //constrain for the left wall
            else if (position.X < radius)
            {
                position = new Vector3(radius, position.Y, position.Z);
                oldPosition += new Vector3(velocity.X * bounce, position.Y, position.Z);
            }
            //constrain for top
            else if (position.Y > height1 - radius)
            {
                position = new Vector3(position.X, height1 - radius, position.Z);
                oldPosition += new Vector3(0, velocity.Y * bounce, position.Z);
            }
            //constrain for bottom
            else if (position.Y < radius)
            {
                position = new Vector3(position.X, radius, position.Z);
                oldPosition += new Vector3(0, velocity.Y * bounce, position.Z);
            }

            sphere.Position = position;
            sphere.OldPosition = oldPosition;
        }





        [RelayCommand]
        void Navigate(string destination)
        {
            switch (destination)
            {
                case "transformations":
                    SelectedToolPanel = Ioc.Default.GetService<TransformationsViewModel>();
                    break;
                case "verlet":
                    SelectedToolPanel = Ioc.Default.GetService<verletIntigrationViewModel>();
                    break;
                case "3D":
                    SelectedToolPanel = Ioc.Default.GetService<Edit3DObjectsViewModel>();
                    break;


            }
        }




        public async Task Draw()
        {
            foreach (var s in shapes.ToList())
            {
                if (s is Shape3D shape3D)
                    shape3D.Draw();
                else if (s is ImportedModel b)
                {
                    var model = new ImportedModel(b.FileName);
                    shapes.Remove(b);
                    await model.Import();
                    shapes.Add(model);
                }
            }
        }




        [RelayCommand]
        [property: JsonIgnore]
        public void ConvertTranformations()
        {
            foreach (var shape in shapes)
                shape.ConvertTransform3DGroupToTransformData();
        }


        [RelayCommand]
        [property: JsonIgnore]
        public void ConvertTransformationsBack()
        {
            foreach (var shape in shapes)
            {
                shape.ConvertTransform3DGroupToTransformData();
            }
        }





        [RelayCommand]
        [property: JsonIgnore]
        public void ObjectedClicked(IShape3D shape)
        {
            var shape3d = shape as Shape3D;
            shape3d.Color = Color.Red;
            Draw();

        }




        [property: JsonIgnore]
        async void CreateFloor()
        {
            var floor = new Floor3D();
            Shapes.Add(floor);
            await Draw();
        }



        public async void OnViewportInitialized(Viewport3DX viewport)
        {
            MyViewPort = viewport;
            CreateFloor();
        }


        [ObservableProperty]
        ObservableCollection<IShape3D> shapes = new();

    }
}
