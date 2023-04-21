using DimensionForge._2D.ViewModels;
using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using Microsoft.Win32;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
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

        [ObservableProperty]
        List<Node3D> verletNodes = new();
        public Canvas3DViewModel()
        {
            //when creating a viewPort3DX remember that an effects manager is essential 

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
        }

        [RelayCommand]
        [property: JsonIgnore]
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

        [property: JsonIgnore]
        void UpdatePhysics()
        {
            //generates a loop that stays on the same thread as the UI thread

            for (int i = 0; i < shapes.Count(); i++)
            {
                if (!(shapes[i] is Sphere3D))
                {
                    continue;
                    break;
                }
                Application.Current.Dispatcher.Invoke(() =>
                {

                    for (int x = 0; x < 1; x++)
                    {
                        UpdatePositions(shapes[i]);
                        for (int j = 0; j < 3; j++)
                        {
                            if (shapes.Count() > 2)
                                Find_collision(shapes[i]);
                        }
                        ConstrainGround(shapes[i]);
                    }
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
                if (shapes[i] == shapes[mainShapeIndex] || shapes[i] is not Sphere3D)
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

            //adding constrains to the ground
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
            //update the position of the models by setting the current and the old position
            var sphere = shape as Models.Sphere3D;
            // to implement the bounce we multiply the OldVelocity by Bounce
            var bounce = 0.8f;
            // to implement gravity we add it to the position.Z after we add velocity
            var gravity = new Vector3(0, 0, -0.99f);
            //multiply the velocity by friction to slow it down
            var friction = 0.900f;
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

        [RelayCommand]
        void DrawVerletNodes()
        {
            verletNodes.ForEach(n => shapes.Add(new CornerPoint3D { NodePosition = n }));
            Draw();
        }
        
        
        [property: JsonIgnore]
        [RelayCommand]
        void VerletShape()
        {
            List<Node3D> nodeList = new();
            //bottom
            nodeList.Add(new Node3D(new Vector3(0, 0, 0)));//0
            nodeList.Add(new Node3D(new Vector3(0, 10, 0)));//1
            nodeList.Add(new Node3D(new Vector3(10, 10, 0)));//2
            nodeList.Add(new Node3D(new Vector3(10, 0, 0)));//3
            //top
            nodeList.Add(new Node3D(new Vector3(0, 0, 10)));//4
            nodeList.Add(new Node3D(new Vector3(0, 10, 10)));//5
            nodeList.Add(new Node3D(new Vector3(10, 10, 10)));//6
            nodeList.Add(new Node3D(new Vector3(10, 0, 10)));//7

            foreach (var node in nodeList)
            {
                var point = new Sphere3D { Radius = 1, Color = Color.Red, Position = node.Position };
                point.Draw();
                shapes.Add(point);

            }

            List<Tube3D> tubeList = new();
            tubeList.Add(new Tube3D(nodeList[0].Position, nodeList[1].Position));
            tubeList.Add(new Tube3D(nodeList[1].Position, nodeList[2].Position));
            tubeList.Add(new Tube3D(nodeList[2].Position, nodeList[3].Position));
            tubeList.Add(new Tube3D(nodeList[3].Position, nodeList[0].Position));
            tubeList.Add(new Tube3D(nodeList[4].Position, nodeList[5].Position));
            tubeList.Add(new Tube3D(nodeList[5].Position, nodeList[6].Position));
            tubeList.Add(new Tube3D(nodeList[6].Position, nodeList[7].Position));
            tubeList.Add(new Tube3D(nodeList[7].Position, nodeList[4].Position));
            tubeList.Add(new Tube3D(nodeList[0].Position, nodeList[4].Position));
            tubeList.Add(new Tube3D(nodeList[1].Position, nodeList[5].Position));
            tubeList.Add(new Tube3D(nodeList[2].Position, nodeList[6].Position));
            tubeList.Add(new Tube3D(nodeList[3].Position, nodeList[7].Position));

            foreach (var tube in tubeList)
            {
                tube.Draw();
                shapes.Add(tube);
            }

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
        public async Task ChangeNode()
        {
            var b = shapes.First(x => x is ImportedModel) as ImportedModel;
            var node = b.cornerNodes.First();
            node.Position += 1;
            await Draw();
        }



        [RelayCommand]
        [property: JsonIgnore]
        public void ObjectedClicked(IShape3D shape)
        {

            if (shape is ImportedModel || shape is Floor3D)
            {

                return;
            }

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
