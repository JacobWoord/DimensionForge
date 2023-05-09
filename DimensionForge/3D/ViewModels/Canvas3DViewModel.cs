
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
using DimensionForge.HelperTools;

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

        private void InitBuildResult()
        {

            buildResult = new VerletBuildResult();
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            door.SetBoundingBox();
            foreach (var element in buildResult.Elements)
            {
                shapes.Add(new Cylinder3D() { P1 = element.Start, P2 = element.End });
            }
            for (int i = 0; i < buildResult.Nodes.Count(); i++)
            {
                shapes.Add(new CornerPoint3D() { LinkedNode = buildResult.Nodes[i], Color = Color.Purple });

            }
            shapes.Add(new CornerPoint3D() { LinkedNode = new Node3D(Vector3.Zero), Color = Color.Red });
            Draw();
        }
        private bool continueVerlet = true;

        [RelayCommand]
        [property: JsonIgnore]
        async Task UpdateVerlet()
        {

            continueVerlet = true;
            var uiDispatcher = Application.Current.Dispatcher;
            int duration = 33; //==30fps
            InitBuildResult();

            await Task.Run(async () =>
            {
                while (continueVerlet)
                {
                    var stopWatch = Stopwatch.StartNew();

                    UpdatePhysics();

                    _ = uiDispatcher.InvokeAsync(async () =>
                    {

                        await UpdateDoorPosition();
                        Draw();
                    });
                    var elapsed = (int)stopWatch.ElapsedMilliseconds;
                    //Debug.WriteLine(elapsed);
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

            for (int i = 0; i < 200; i++)
            {

                foreach (var s in buildResult.Elements)
                    UpdateSticks(s);


                foreach (var n in buildResult.Nodes)
                    ConstrainGround(n);
            }


        }

        public async Task UpdateDoorPosition()
        {

            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;


            //First im trying to calculate the translation vector bases on the center positions of the models  (MODEL WORD NIET VERPLAATST MET HET CENTERPUNT)
            Vector3 verletCenter = PositioningHelper.GetCenterOfMass(buildResult.Nodes);
            Vector3 modelCenter = PositioningHelper.GetCenterOfMass(door.Bbcorners);
            Vector3 translation = verletCenter - modelCenter;

            Quaternion combinedRotationQuaternion = ComputeBoxRotationHelper.ComputeRotationQuaternion(door.Bbcorners, buildResult.Nodes);
            await door.MoveCenterToPosition(Vector3.Zero);
            door.Transform = ComputeBoxRotationHelper.CreateTransformGroup(combinedRotationQuaternion, door.Bbcorners, door.Transform);
            await door.MoveCenterToPosition(verletCenter);



            //verletCenter = GetCenterOfMass(buildResult.Nodes);
            //modelCenter = GetCenterOfMass(door.Bbcorners);
            //translation = verletCenter - modelCenter;
            // await door.MoveCenterToPosition(verletCenter);

        }

        [RelayCommand]
        void DrawBoundingBox()
        {
            //reNew
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            //clear the list from the previous bounding shapes

            door.Bbcorners.ForEach(x => shapes.Add(new CornerPoint3D() { LinkedNode = x, Color = x.Color, UseCase = UseCase.boundings }));
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

        [RelayCommand]
        public async Task Reload(string floorNumber = "1")
        {

            continueVerlet = false;
            shapes.Clear();
            CreateFloor(floorNumber);
            await Import("C:\\Users\\jacob\\AppData\\Roaming\\Net Designer\\3DModels\\FISHINGBOARD_SB.obj");
        }

        [property: JsonIgnore]
        void CreateFloor(string num = "1")
        {
            var floor = new Floor3D(num);
            floor.Draw();

            Shapes.Add(floor);
            Draw();
        }
        public async Task OnViewportInitialized(Viewport3DX viewport)
        {
            shapes.Clear();
            MyViewPort = viewport;
            CreateFloor();
            await Import("C:\\Users\\jacob\\AppData\\Roaming\\Net Designer\\3DModels\\FISHINGBOARD_SB.obj");

            shapes.Add(new CornerPoint3D()
            {
                LinkedNode = new Node3D(new Vector3(0, 0, 0)),
                Color = Color.Purple,
                UseCase = UseCase.direction
            });
            var door = shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;

              Draw();

        }





        async Task Import(string filePath)
        {
            var batchedmodel = new ImportedModel(filePath);
            await batchedmodel.OpenFile();
            Shapes.Add(batchedmodel);

            var model = Shapes.FirstOrDefault(x => x is BatchedModel3D) as BatchedModel3D;
            model.TranslateTo(new Vector3(0, 0, 8));


        }


        [ObservableProperty]
        ObservableCollection<IShape3D> shapes = new();

    }
}
