
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
using DimensionForge.Common;
using Quaternion = SharpDX.Quaternion;
using MeshGeometry3D = HelixToolkit.SharpDX.Core.MeshGeometry3D;
using DimensionForge.HelperTools;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Assimp;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using Assimp;
using System.Xml.Linq;
using System.Diagnostics;
using DimensionForge._2D.ViewModels;
using System.Threading;
using HelixToolkit.SharpDX.Core.Animations;
using System.Reflection.Metadata.Ecma335;

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

            this.EffectsManager = new DefaultEffectsManager();
            // Create and set up the camera
            Camera = new HelixToolkit.Wpf.SharpDX.OrthographicCamera
            {
                Position = new Point3D(0, 0, 10),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 150,
                NearPlaneDistance = -150,
                Width = 1000,
            };
            Camera.CreateViewMatrix();
        }


        private void InitBuildResult()
        {


            MeshGeometry();
            buildResult = new VerletBuildResult();
            var door = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;
            var boundingelements = door.GetBoundingElements();
            boundingelements.ForEach(x => shapes.Add(x));
           



            foreach (var element in buildResult.Elements)
            {
                Shapes.Add(new Cylinder3D() { Start = element.Start, End = element.End, Color = element.Color });
            }
            for (int i = 0; i < buildResult.Nodes.Count(); i++)
            {
                Shapes.Add(new CornerPoint3D() { LinkedNode = buildResult.Nodes[i], Color = Color.Purple });

            }
            Shapes.Add(new CornerPoint3D() { LinkedNode = new Node3D(Vector3.Zero), Color = Color.Red });
            Draw();
        }

        private async void MeshGeometry()
        {
            var model = new ObjModel3D(await ObjHelperClass.ImportAsMeshGeometry3D("C:\\Users\\jacob\\AppData\\Roaming\\Net Designer\\3DModels\\FISHINGBOARD_SB.obj"));
            shapes.Add(model);
            ObjHelperClass.UpdatePosition(model, new Vector3(0, 0, 10));
        }



        private bool continueVerlet = true;

        [RelayCommand]
        [property: JsonIgnore]
        async Task UpdateVerlet()
        {

            continueVerlet = true;
            var uiDispatcher = Application.Current.Dispatcher;
            int duration = 33; //==30fps

            await Task.Run(async () =>
            {
                while (continueVerlet)
                {
                    var stopWatch = Stopwatch.StartNew();

                    UpdatePhysics();
                    // UpdateExperiment();
                    UpdateDoorPositionEMA();
                    await uiDispatcher.InvokeAsync(() =>
                    {
                        Draw();
                        return Task.CompletedTask;

                        
                    });
                    var elapsed = (int)stopWatch.ElapsedMilliseconds;
                    Debug.WriteLine(elapsed);
                    int delay = Math.Max(0, duration - elapsed);
                    await Task.Delay(delay);
                }
            });
        }

        [RelayCommand]
        public void MoveToCenter()
        {
            var door = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;
            ObjHelperClass.UpdatePosition(door, Vector3.Zero);

        }

        [RelayCommand]
        void DrawCenters()
        {
            var door = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;


         
            Draw();

        }




        [property: JsonIgnore]
        void UpdatePhysics()
        {
            // generates a loop that stays on the same thread as the UI thread
            foreach (var n in buildResult.Nodes)
            {
                UpdatePositions(n);
            }
            for (int i = 0; i < 100; i++)
            {
                foreach (var s in buildResult.Elements)
                    UpdateSticks(s);
                foreach (var n in buildResult.Nodes)
                    ConstrainGround(n);
            }
        }


        private void UpdateExperiment()
        {
            var door = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;
            var boxcenter = PositioningHelper.GetCenterOfMass(buildResult.Nodes);


            ObjHelperClass.UpdatePosition(door,boxcenter);


        }


        Queue<Quaternion> rotationHistory = new Queue<Quaternion>();

        private Quaternion _previousRotation = Quaternion.Identity;

        [RelayCommand]
        public void UpdateDoorPositionEMA()
        {
            var door = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;

            // Calculate the translation vector based on the center positions of the models
            Vector3 verletCenter = PositioningHelper.GetCenterOfMass(buildResult.Nodes);
            Vector3 modelCenter = ObjHelperClass.CalculateModelCenter(door);
            Vector3 translation = verletCenter - modelCenter;

            // Define the alpha value for the EMA smoothing technique
            float alpha = 0.1f; // You can experiment with this value

            // Calculate the combined rotation quaternion using the EMA smoothing technique
            Quaternion combinedRotationQuaternion = ComputeBoxRotationHelper.ComputeCombinedRotationQuaternionEMA(door.BoundingPositions, buildResult.Nodes, ref _previousRotation, alpha);

            var dSize = DebugHelper.GetBoxDimensions(door.BoundingPositions);
            var vSize = DebugHelper.GetBoxDimensions(buildResult.Nodes);

            DebugHelper.DebugDimensionDifference(dSize, vSize);

           // Debug.WriteLine($"MODEL DIMENSIONS: Height: {dSize.Z} Width:{dSize.Y} Depth:{dSize.X}" );
           // Debug.WriteLine($"BOX DIMENSIONS: Height: {vSize.Z} Width:{vSize.Y} Depth:{vSize.X}");

            if (!IsValidQuaternion(combinedRotationQuaternion))
            {
                Debug.WriteLine(":Invalid rotation quaternion detected  ");
                return;
            }

          
            // Apply the rotation and update the position
            ObjHelperClass.ApplyRotation(door, combinedRotationQuaternion, modelCenter);
            ObjHelperClass.UpdatePosition(door, verletCenter);
        }


        private bool IsValidQuaternion(Quaternion q)
        {
            float epsilon = 1e-6f;
            float length = q.Length();
            return (Math.Abs(length - 1) < epsilon);
        }




        [RelayCommand]
        void Centreer()
        {
            var door = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;

            Vector3 verletCenter = PositioningHelper.GetCenterOfMass(buildResult.Nodes);
            Vector3 modelCenter = ObjHelperClass.CalculateModelCenter(door);
            Vector3 translation = verletCenter - modelCenter;


            ObjHelperClass.UpdatePosition(door ,verletCenter);
              
               Draw();


            
        }



        [RelayCommand]
        void Rotate()
        {
            var door = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;
            ObjHelperClass.RotateGeometry(door, Vector3.UnitX, 60);
        }

        [RelayCommand]
        void DrawBoundingBox()
        {
            
            var door = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;

            door.BoundingPositions.ForEach(x => shapes.Add(new CornerPoint3D() { Color  = Color.Yellow , LinkedNode = x}));
            Draw();
        }

        [RelayCommand]
        void MoveToNextIteration()
        {
            continueVerlet = !continueVerlet;
        }
        void UpdateSticks(verletElement3D stick)
        {


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
        public void Draw()
        {
            foreach (var s in Shapes.ToList())
            {
                if (s is Shape3D shape3D)
                {

                    shape3D.Draw();
                }

            }
        }

        [RelayCommand]
        [property: JsonIgnore]
        public void ObjectedClicked(IShape3D shape)
        {


        }

        [RelayCommand]
        public void Reload(string floorNumber = "1")
        {
            continueVerlet = false;
            shapes.Clear();
            CreateFloor(floorNumber);
            //await Import("C:\\Users\\jacob\\AppData\\Roaming\\Net Designer\\3DModels\\FISHINGBOARD_SB.obj");
            InitBuildResult();
        }


        [property: JsonIgnore]
        void CreateFloor(string num = "1")
        {
            var floor = new Floor3D(num);
            floor.Draw();

            Shapes.Add(floor);
            Draw();
        }
        public void OnViewportInitialized(Viewport3DX viewport)
        {
            Shapes.Clear();
            MyViewPort = viewport;
            CreateFloor();
            InitBuildResult();

            Shapes.Add(new CornerPoint3D()
            {
                LinkedNode = new Node3D(new Vector3(0, 0, 0)),
                Color = Color.Purple,
                UseCase = UseCase.direction
            });

            // ZoomTo();


            Draw();

        }

        public void ZoomTo()
        {
            var offset = 1;
            var model = shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;

            var rect3d = new Rect3D(ObjHelperClass.CalculateModelCenter(model).ToPoint3D(), new Size3D(model.Width + offset, model.Height + offset, model.Depth + offset));

            MyViewPort.ZoomExtents(rect3d, 500);
            MyViewPort.Reset();
        }







        [ObservableProperty]
        ObservableCollection<IShape3D> shapes = new();

    }
}
