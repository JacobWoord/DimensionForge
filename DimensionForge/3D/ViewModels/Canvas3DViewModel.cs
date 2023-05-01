
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

            Draw();
        }

        private bool continueVerlet = true;

        
        
        [RelayCommand]
        [property: JsonIgnore]
        async Task UpdateVerlet()
        {

            //capture the ui Dispatcher 
            var uiDispatcher =  Application.Current.Dispatcher; 
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
                    });
                    var elapsed = (int)stopWatch.ElapsedMilliseconds;
                    Debug.WriteLine(elapsed);
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

         



            // //  UpdateModelTransformations();








        }


        void UpdateDoorPosition()
        {
            //var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;

            //var pos1 = door.VerletData.CornerList.First(x => x.NodePos == NodePosition.BottomRight).Position;
            //var pos2 = door.VerletData.CornerList.First(x => x.NodePos == NodePosition.BottomLeft).Position;
            //door.TranslateTo(pos1);
            // var dir =Vector3.Normalize(pos1 - pos2);

            // var thirdNode = door.cornerNodes.First(x=> x.NodePos==NodePosition.LeftTop).Position;
            // var angle = Utils3D.AngleBetweenAxes(pos1, pos2, pos1, thirdNode);

            // var axis = pos2 + dir * 100;
            //// door.Rotate(axis.ToVector3D(), angle);
        }









        void UpdateModelTransformations()
        {
            //This functions is updating the model postitions
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            //Verlet object corner (the door need to follow this position
            var cornerPosition = door.cornerNodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomRight).Position;
            //translates the door each frame to the verlet objects
            door.TranslateTo(cornerPosition);
            //third point from plane
            //var thirdPoint = door.cornerNodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft).Position;
            ////set the rotations for the door
            //door.VerletData.SetRotation(thirdPoint);

            ////translates the door to the origin to exercute the rotation
            //var angle = door.VerletData.Angle * 180 / MathF.PI;
            //var axis = door.VerletData.Normal.ToVector3D();
            //door.TranslateTo(Vector3.Zero);
            //door.Rotate(axis, angle);
            ////door.Rotate(axis, 180);
            ////translates the door back to the position
            //door.TranslateTo(cornerPosition.Position);


            //var cyl = shapes.FirstOrDefault(x => x is Cylinder3D) as Cylinder3D;
            //cyl.P1.Position = door.VerletData.P1;
            //cyl.P2.Position = door.VerletData.P2;


        }






        // Call this function to continue to the next iteration
        [RelayCommand]
        void MoveToNextIteration()
        {
            continueVerlet = !continueVerlet;
        }





        void UpdateSticks(verletElement3D stick)
        {
            //reference to the Node3D of the Cylinder
            var node1 = stick.Start;
            var node2 = stick.End;
            var pos1 = node1.Position;
            var pos2 = node2.Position;

            var dx = pos2.X - pos1.X;
            var dy = pos2.Y - pos1.Y;
            var distance = Vector3.Distance(node1.Position, node2.Position);
            var stickLength = stick.Length;
            var differnce = stickLength - distance;
            var percent = differnce / distance / 2;
            var offsetX = dx * percent;
            var offsetY = dy * percent;

            pos1.X -= offsetX;
            pos1.Y -= offsetY;
            pos2.X += offsetX;
            pos2.Y += offsetY;

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







        //test method to add verlet on the boundries of the door.
        [RelayCommand]
        void DrawVerletNodes()
        {
            //var model = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            //var cornerList = model.VerletData.CornerList;
            //var elementList = model.VerletData.ElementList;



            ////stick 1
            //var s1 = new verletElement3D();
            //s1.Start = cornerList[0];
            //s1.End = cornerList[1];

            //s1.Length = Vector3.Distance(s1.Start.Position, s1.End.Position);
            //elementList.Add(s1);

            ////stick 2
            //var s2 = new verletElement3D();
            //s2.Start = cornerList[1];
            //s2.End = cornerList[2];

            //s2.Length = Vector3.Distance(s2.Start.Position, s2.End.Position);
            //elementList.Add(s2);

            ////stick 3
            //var s3 = new verletElement3D();
            //s3.Start = cornerList[2];
            //s3.End = cornerList[3];

            //s3.Length = Vector3.Distance(s3.Start.Position, s3.End.Position);
            //elementList.Add(s3);

            //var s4 = new verletElement3D();
            //s4.Start = cornerList[3];
            //s4.End = cornerList[0];
            //s4.Length = s4.GetElementLength();

            //elementList.Add(s4);


            ////stick 5 Diagonal
            //var rightTop = cornerList.Where(v => v.Position.X >= cornerList[0].Position.X && v.Position.Y >= cornerList[0].Position.Y)
            //     .OrderBy(v => v.Position.X)
            //     .ThenByDescending(v => v.Position.Y)
            //     .FirstOrDefault();
            //var leftBottom = cornerList.Where(v => v.Position.X <= cornerList[2].Position.X && v.Position.Y <= cornerList[2].Position.Y)
            //    .OrderByDescending(v => v.Position.X)
            //    .ThenBy(v => v.Position.Y)
            //    .FirstOrDefault();
            //var s5 = new verletElement3D();
            //s5.Start = rightTop;
            //s5.End = leftBottom;

            //s5.Length = s5.GetElementLength();
            //elementList.Add(s5);

            ////Stick 6 Diagonal
            //var leftTop = cornerList.Where(v => v.Position.X >= cornerList[1].Position.X && v.Position.Y >= cornerList[1].Position.Y)
            //     .OrderBy(v => v.Position.X)
            //     .ThenByDescending(v => v.Position.Y)
            //     .FirstOrDefault();
            //var rightBottom = cornerList.Where(v => v.Position.X <= cornerList[3].Position.X && v.Position.Y <= cornerList[3].Position.Y)
            //    .OrderByDescending(v => v.Position.X)
            //    .ThenBy(v => v.Position.Y)
            //    .FirstOrDefault();
            //var s6 = new verletElement3D();
            //s6.Start = leftTop;
            //s6.End = rightBottom;

            //s6.Length = s6.GetElementLength();
            //elementList.Add(s6);

            //elementList.ForEach(e => shapes.Add(new Cylinder3D() { P1 = e.Start, P2 = e.End, Color = Color.Green }));
            //cornerList.ForEach(n => shapes.Add(new CornerPoint3D { LinkedNode = n }));

            //Draw();
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
            //var b = shapes.First(x => x is ImportedModel) as ImportedModel;
            //var node = b.First();
            //node.Position += 1;
            //await Draw();
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

            var dirCyl = new Cylinder3D();
            dirCyl.Color = Color.White;
            shapes.Add(dirCyl);
        }

        [ObservableProperty]
        ObservableCollection<IShape3D> shapes = new();

    }
}
