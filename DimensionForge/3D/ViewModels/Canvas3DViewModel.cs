
using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
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
using Net_Designer_MVVM;
using System.Diagnostics;
using Quaternion = SharpDX.Quaternion;
using Assimp.Configs;

namespace DimensionForge._3D.ViewModels
{
    //CURRENT

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
                FarPlaneDistance = 100000,
                NearPlaneDistance = -100000,
                Width = 1000,
            };

            Camera.CreateViewMatrix();
        }

        private async Task InitBuildResult()
        {
            //INIT BUILD RESULT
            buildResult = new VerletBuildResult();
            buildResult.GetAllElements();
            buildResult.AddNodesToList();
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            door.SetBoundingBox();

            var colors = Utils3D.GetColorList();

            foreach (var element in buildResult.Elements)
            {
                shapes.Add(new Cylinder3D() { P1 = element.Start, P2 = element.End });
            }
            for (int i = 0; i < buildResult.Nodes.Count(); i++)
            {
                shapes.Add(new CornerPoint3D() { LinkedNode = buildResult.Nodes[i], Color = Color.Purple });

            }

            ////cylinder to point in the direction of the normal vector for the rotation axis
            //var cyl = new Cylinder3D();
            //cyl.UseCase = UseCase.direction;
            //cyl.P1 = new Node3D(Vector3.Zero);
            //cyl.P2 = new Node3D(Vector3.Zero);
            //shapes.Add(cyl);
         

            shapes.Add(new CornerPoint3D() { LinkedNode = new Node3D(Vector3.Zero), Color = Color.Red});


            await Draw();
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
            int duration = 33; //==30fps

            await InitBuildResult();
          //  await DrawAnchorPoints();

            await Task.Run(async () =>
            {
                while (continueVerlet)
                {
                    var stopWatch = Stopwatch.StartNew();
                    //  var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
                    UpdatePhysics();

                    _ = uiDispatcher.InvokeAsync(async () =>
                    {
                       

                        await Draw();
                       await  UpdateDoorPosition();

                    });
                    var elapsed = (int)stopWatch.ElapsedMilliseconds;
                    Debug.WriteLine(elapsed);
                    int delay = duration - elapsed;
                    await Task.Delay(delay);
                }
            });


        }


        public Vector3 GetCenterOfMass(List<Node3D> nodes)
        {
            Vector3 centerOfMass = Vector3.Zero;
            int nodeCount = nodes.Count;

            if (nodeCount == 0)
                return centerOfMass;

            foreach (Node3D node in nodes)
            {
                centerOfMass += node.Position;
            }

            centerOfMass /= nodeCount;
            return centerOfMass;
        }




        public Quaternion ComputeVerticalRotationQuaternion(List<Node3D> buildresult, BatchedModel3D model)
        {

            Vector3 verletOrientation = buildResult.GetOrientation();
            Vector3 modelOrientation = model.GetOrientation();

            // Normalize the orientation vectors
            verletOrientation.Normalize();
            modelOrientation.Normalize();

            // Compute the rotation axis and angle
            Vector3 rotationAxis = Vector3.Cross(modelOrientation, verletOrientation);
            float rotationAngle = (float)Math.Acos(Vector3.Dot(modelOrientation, verletOrientation));

            // Create a quaternion
            Quaternion rotationQuaternion = Quaternion.RotationAxis(rotationAxis, rotationAngle);

            return rotationQuaternion;
        }

        public Quaternion ComputeHorizontalRotationQuaternion(List<Node3D> buildresult, BatchedModel3D model)
        {

            Vector3 verletOrientation = buildResult.GetHorizontalOrientation();
            Vector3 modelOrientation = model.GetHorizontalOrientation();

            // Normalize the orientation vectors
            verletOrientation.Normalize();
            modelOrientation.Normalize();

            // Compute the rotation axis and angle
            Vector3 rotationAxis = Vector3.Cross(modelOrientation, verletOrientation);
            float rotationAngle = (float)Math.Acos(Vector3.Dot(modelOrientation, verletOrientation));

            // Create a quaternion
            Quaternion rotationQuaternion = Quaternion.RotationAxis(rotationAxis, rotationAngle);

            return rotationQuaternion;
        }


        [RelayCommand]
        public async Task UpdateDoorPosition()
        {
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;


            //First im trying to calculate the translation vector bases on the center positions of the models  (MODEL WORD NIET VERPLAATST MET HET CENTERPUNT)
            Vector3 verletCenter = GetCenterOfMass(buildResult.Nodes);
            Vector3 modelCenter = GetCenterOfMass(door.Nodes.Where(n => n.NodePos != NodePosition.None).ToList());//model.GetModelCenter();
            Vector3 translation = verletCenter - modelCenter;

            Quaternion horizontalRotationQuaternion = ComputeHorizontalRotationQuaternion(buildResult.Nodes, door);
            Transform3DGroup transformGroup2 = door.CreateTransformGroup(translation, 0.01f, horizontalRotationQuaternion);


            Quaternion verticalRotationQuaternion = ComputeVerticalRotationQuaternion(buildResult.Nodes, door);
            // Create a Transform3DGroup and apply it to the 3D model
            Transform3DGroup transformGroup1 = door.CreateTransformGroup(translation, 0.01f, verticalRotationQuaternion);
            

            door.Transform = transformGroup1;
            door.Transform = transformGroup2;

            door.MoveCenterToPosition(verletCenter);

        }

        [RelayCommand]
        public void MoveCenter()
        {
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;


            var shape = shapes.FirstOrDefault(x => x is Sphere3D) as Shape3D;
             door.MoveCenterToPosition(shape.Position.Position);

        }




        [property: JsonIgnore]
        void UpdatePhysics()
        {

            // generates a loop that stays on the same thread as the UI thread
            foreach (var n in buildResult.Nodes)
            {
                UpdatePositions(n);

            }

            for (int i = 0; i < 20; i++)
            {

                foreach (var s in buildResult.Elements)
                    UpdateSticks(s);


                foreach (var n in buildResult.Nodes)
                    ConstrainGround(n);
            }


        }






        [RelayCommand]
        async void DrawBoundingBox()
        {
            //reNew
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            //clear the list from the previous bounding shapes
          
            door.Bbcorners.ForEach(x => shapes.Add(new CornerPoint3D() { LinkedNode = x, Color = x.Color, UseCase = UseCase.boundings }));
            await Draw();


        }


        [RelayCommand]
        async Task DrawAnchorPoints()
        {
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            var anchorPoints = shapes.Where(x => x.UseCase == UseCase.anchorPoints).ToList();
            if (anchorPoints.Count() > 0)
                anchorPoints.ForEach(x => shapes.Remove(x));

            var lt = door.Nodes.FirstOrDefault(x => x.NodePos == NodePosition.RightTop);
            lt.Color = Color.Green;

            door.Nodes.ForEach(x => shapes.Add(new CornerPoint3D() { LinkedNode = x, Color = x.Color, UseCase = UseCase.anchorPoints }));

            door.Nodes.ForEach(x => Debug.WriteLine(x.Position));
            await Draw();
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
            //float diff = (deltaLength - stick.Length) / deltaLength;
            ////  var offset = delta * (diff / deltaLength) / 2;

            //stick.Start.Position += (delta * diff) / 2;
            //stick.End.Position -= (delta * diff) / 2;



            var delta = stick.End.Position - stick.Start.Position;
            float deltaLength = delta.Length();
            float diff = (deltaLength - stick.Length);
            var offset = delta * (diff / deltaLength) / 2;

            stick.Start.Position += offset;
            stick.End.Position -= offset;

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

            var bounce = 0.99f;

            if (node.Position.Z < 0)
            {
                node.Position = new Vector3(node.Position.X, node.Position.Y, 0);
                node.OldPosition = new Vector3(node.Position.X, node.Position.Y, node.Position.Z + zv * bounce);
            }

        }
        void UpdatePositions(Node3D node)
        {
            //update the position of the models by setting the current and the old position
            var position = node.Position;
            // to implement gravity we add it to the position.Z after we add velocity
            var gravity = new Vector3(0, 0, -0.3f);
            //multiply the velocity by friction to slow it down
            var friction = 0.8f;
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
                case "FloorTextures":
                    SelectedToolPanel = Ioc.Default.GetService<FloorTexturesViewModel>();
                    break;


            }
        }

        public async Task Draw()
        {

            foreach (var s in shapes.ToList())
            {
                if (s is Shape3D shape3D)
                {
                    shape3D.Draw();
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
        public async void ObjectedClicked(IShape3D shape)
        {

            if (shape is ImportedModel || shape is Floor3D)
            {

                return;
            }

            var shape3d = shape as Shape3D;
            shape3d.Color = Color.Red;
            await Draw();

        }

        [RelayCommand]
        public async Task Reload(string floorNumber = "1")
        {

            continueVerlet = false;
            shapes.Clear();
            await CreateFloor(floorNumber);
            await Import("C:\\Users\\jacob\\AppData\\Roaming\\Net Designer\\3DModels\\FISHINGBOARD_SB.obj");
        }

        [property: JsonIgnore]
        async Task CreateFloor(string num = "1")
        {
            var floor = new Floor3D(num);
            floor.Draw();

            Shapes.Add(floor);
            await Draw();
        }
        public async void OnViewportInitialized(Viewport3DX viewport)
        {
            shapes.Clear();
            MyViewPort = viewport;
            await CreateFloor();
            await Import("C:\\Users\\jacob\\AppData\\Roaming\\Net Designer\\3DModels\\FISHINGBOARD_SB.obj");

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
            model.TranslateTo(new Vector3(0, 0, 80));


        }


        [ObservableProperty]
        ObservableCollection<IShape3D> shapes = new();

    }
}
