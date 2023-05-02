
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
using DimensionForge.Common;
using DimensionForge._2D.ViewModels;
using Net_Designer_MVVM;
using SharpDX.Direct3D11;
using System.Net.NetworkInformation;
using System.Diagnostics;

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




        private VerletBuildResult buildResult { get; set; }


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
                Width = 1000,
            };

            Camera.CreateViewMatrix();
        }

        private void InitBuildResult()
        {
            //INIT BUILD RESULT
            buildResult = new VerletBuildResult();
            buildResult.GetAllElements();
            buildResult.AddNodesToList();


            foreach (var element in buildResult.Elements)
            {
                shapes.Add(new Cylinder3D() { P1 = element.Start, P2 = element.End });
            }
            foreach (var node in buildResult.Nodes)
            {
                shapes.Add(new CornerPoint3D() { LinkedNode = node });
            }


            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            var ball = new CornerPoint3D() { LinkedNode = door.cornerNodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft), };
            shapes.Add(ball);
            //buildResult.Nodes.Add(ball.LinkedNode);

            Draw();
        }

        private bool continueVerlet = true;



        [RelayCommand]
        [property: JsonIgnore]
        async Task UpdateVerlet()
        {
            //set the bool to true
            continueVerlet = true;
            //capture the ui Dispatcher 
            var uiDispatcher = Application.Current.Dispatcher;
            int duration = 33;

            InitBuildResult();

            await Task.Run(async () =>
            {
                while (continueVerlet)
                {
                    var stopWatch = Stopwatch.StartNew();
                    UpdatePhysics();

                    _ = uiDispatcher.InvokeAsync(async () =>
                    {
                       
                          await Draw();

                        
                          UpdateDoorPosition();
                        

                       
                        // UpdateModelTransformations();

                    });
                    var elapsed = (int)stopWatch.ElapsedMilliseconds;
                  //  Debug.WriteLine(elapsed);
                    int delay = duration - elapsed;
                    await Task.Delay(delay);
                }
            });


        }



        [property: JsonIgnore]
        void UpdatePhysics()
        {

            // generates a loop that stays on the same thread as the UI thread
            foreach (var n in buildResult.Nodes)
            {
                UpdatePositions(n);

            }

            for (int i = 0; i < 5; i++)
            {

                foreach (var s in buildResult.Elements)
                    UpdateSticks(s);


                foreach (var n in buildResult.Nodes)
                    ConstrainGround(n);
            }


        }




        void UpdateDoorPosition()
        {
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            var bottomRight = buildResult.Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomRight).Position;
            var bottomLeft = buildResult.Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft).Position;

            //transform the door each itteration to the verletShape
            door.TranslateTo(bottomRight);

            //the results variable are the retrieved values from the SetRotation method
            var results = buildResult.SetRotation(door.cornerNodes.First(x=>x.NodePos == NodePosition.BottomRight).Position);
            var angle = results.angle * 180 / MathF.PI;

            //this is the cylinder that represents the calculated axis
            var cyl = shapes.FirstOrDefault(x => x is Cylinder3D) as Cylinder3D;
            cyl.P1.Position = results.p1;
            cyl.P2.Position = results.p2;


            //axis to rotate the door arround
            var axis = results.normal;

            //bring the door to the origin to apply the rotation 
            door.TranslateTo(Vector3.Zero);
            door.Rotate(axis.ToVector3D(), angle);

            //translate the door back to the calculated position
            door.TranslateTo(buildResult.Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomRight).Position);


        }













        void UpdateModelTransformations()
        {
            var bottomRight = buildResult.Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomRight).Position;
            var bottomLeft = buildResult.Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft).Position;
            var side = Vector3.Zero;
          
            
            if (bottomLeft.X > bottomRight.X)
            {
                side = bottomLeft;
            }

            side = bottomRight;


            
            //This functions is updating the model position to the verlet objects
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            door.TranslateTo(side);
            //door.SetCornerNodes();

            //third point for plane
            var thirdPoint = door.cornerNodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft);

            //set the rotations for the door
            var result = buildResult.SetRotation(thirdPoint.Position);
           
            var angle = result.angle * 180 / MathF.PI;

           
            var cyl = shapes.FirstOrDefault(x => x is Cylinder3D) as Cylinder3D;
            cyl.P1.Position = result.p1;
            cyl.P2.Position = result.p2;
            //if (angle > 3)
            //{

            var axis = result.normal.ToVector3D();

            //translates the door to the origin to exercute the rotation
            door.TranslateTo(Vector3.Zero);
            
            door.Rotate(axis, angle);
            ////translates the door back to the position
           
            door.TranslateTo(side);
            
           
        }
            
           






        [RelayCommand]
        async Task DrawCornerNodes()
        {
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
           // door.SetCornerNodes();
            var nodes = door.cornerNodes;
            foreach (var node in nodes)
            {
                shapes.Add(new CornerPoint3D() { LinkedNode = node});
            }

          await Draw();

        }

        [RelayCommand]
        void DrawPlane()
        {
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
           
            var firstPoint = buildResult.Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomRight);
            var secondPoint = buildResult.Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft);
            var thirdPoint = door.cornerNodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft);


            // shapes.Add(new DrawPlane3D(firstPoint, secondPoint, thirdPoint));

            shapes.Add(new Cylinder3D() { P1 = secondPoint, P2 = thirdPoint, Color= Color.Black });
            Draw();



        }






        // Call this function to continue to the next iteration
        [RelayCommand]
        void MoveToNextIteration()
        {
            continueVerlet = !continueVerlet;
        }







        void UpdateSticks(verletElement3D stick)
        {
            //var delta = stick.End.Position - stick.Start.Position;
            //float deltaLength = delta.Length();
            //float diff = (deltaLength - stick.Length);
            //var offset = delta * (diff / deltaLength) / 2;

            //if (!stick.Start.Pinned)
            //{
            //    if (stick.End.Pinned)
            //    {
            //        stick.Start.Position += offset * 2;
            //    }
            //    else
            //    {
            //        stick.Start.Position += offset;
            //    }

            //}
            //if (!stick.End.Pinned)
            //{
            //    if (stick.Start.Pinned)
            //    {
            //        stick.End.Position -= offset * 2;
            //    }
            //    else
            //    {
            //        stick.End.Position -= offset;
            //    }
            //}


            //reference to the Node3D of the Cylinder
            var node1 = stick.Start;
            var node2 = stick.End;
            var pos1 = node1.Position;
            var pos2 = node2.Position;

            var dx = pos2.X - pos1.X;
            var dy = pos2.Y - pos1.Y;
            var dz = pos2.Z - pos1.Z;
            var distance = Vector3.Distance(node1.Position, node2.Position);
            var stickLength = stick.Length;
            var differnce = stickLength - distance;
            var percent = differnce / distance / 2;
            var offsetX = dx * percent;
            var offsetY = dy * percent;
            var offsetZ = dz * percent;

            pos1.X -= offsetX;
            pos1.Y -= offsetY;
            pos1.Z -= offsetZ;
            pos2.X += offsetX;
            pos2.Y += offsetY;
            pos2.Z += offsetZ;

            node1.Position = pos1;
            node2.Position = pos2;
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
                var otherPosition = otherShape.Position.Position;
                var otherOldPosition = otherShape.OldPosition;

                float radiusSum = shapeToCheck.Radius + otherShape.Radius;
                var colission_axis = mainPosition.Position - otherPosition; //n = collision_axis / 2
                float dist = Vector3.Distance(mainPosition.Position, otherPosition);

                //if the distance between 2 balls is less then the sum of both radius
                if (dist < radiusSum)
                {

                    Vector3 n = colission_axis / dist;
                    //delta is the value that the balls need to move to keep free of eachother 
                    float delta = radiusSum - dist;

                    mainPosition.Position += 0.5f * delta * n;
                    otherPosition -= 0.5f * delta * n;

                    shapeToCheck.Position = mainPosition;
                    otherShape.Position.Position = otherPosition;
                    // shapeToCheck.OldPosition = mainPosition -5;
                    // otherShape.OldPosition = mainPosition -5;
                }
            }
        }
        public void ConstrainGround(Node3D node)
        {

            //adding constrains to the ground

            var xv = node.Position.X - node.OldPosition.X;  // x velocity
            var yv = node.Position.Y - node.OldPosition.Y;  // y velocity
            var zv = node.Position.Z - node.OldPosition.Z;  // z velocity

            var bounce = 0.8f;

            if (node.Position.Z < node.RadiusInMeters)
            {
                node.Position = new Vector3(node.Position.X, node.Position.Y, node.RadiusInMeters);
                node.OldPosition = new Vector3(node.Position.X, node.Position.Y, node.Position.Z + zv * bounce);
            }

        }
        void UpdatePositions(Node3D node)
        {
            //update the position of the models by setting the current and the old position
            var position = node.Position;
            // to implement the bounce we multiply the OldVelocity by Bounce
            var bounce = 0.9f;
            // to implement gravity we add it to the position.Z after we add velocity
            var gravity = new Vector3(0, 0, -0.3f);
            //multiply the velocity by friction to slow it down
            var friction = 0.950f;
            var radius = node.RadiusInMeters;

            var oldPosition = node.OldPosition;
            var velocity = (position - oldPosition) * friction;
            oldPosition = position;
            position += velocity;
            position += gravity;

            node.Position = position;
            node.OldPosition = oldPosition;
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
        async void Reload()
        {
            continueVerlet = false;
            shapes.Clear();
            CreateFloor();
            await Import("C:\\Users\\jacob\\OneDrive - Rapid Engineering B.V\\Bureaublad\\FISHINGBOARD_SB.obj");
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
            shapes.Clear();
            MyViewPort = viewport;
            //CreateFloor();
            await Import("C:\\Users\\jacob\\OneDrive - Rapid Engineering B.V\\Bureaublad\\FISHINGBOARD_SB.obj");

            var dirCyl = new Cylinder3D();
            dirCyl.Color = Color.White;
            shapes.Add(dirCyl);
        }





        async Task Import(string filePath)
        {
            var batchedmodel = new ImportedModel(filePath);
            await batchedmodel.OpenFile();
            Shapes.Add(batchedmodel);

            var model = Shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            model.TranslateTo(new Vector3(0, 0, 10));
            model.SetCornerNodes();



        }





        [ObservableProperty]
        ObservableCollection<IShape3D> shapes = new();

    }
}
