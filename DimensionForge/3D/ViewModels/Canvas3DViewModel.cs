
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
using System.Collections.Generic;
using System.Diagnostics;
using HelixToolkit.Wpf;
using SharpDX.Direct3D9;
using Assimp;
using System.IO;
using HelixToolkit.SharpDX.Core.Model.Scene;
using MaterialDesignColors.Recommended;

namespace DimensionForge._3D.ViewModels
{
    //CURRENT

    [Serializable]
    public partial class Canvas3DViewModel : ObservableObject
    {

        private Quaternion _previousRotation = Quaternion.Identity;

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
        Shape3D selectedModel;

        partial void OnSelectedModelChanged(Shape3D value)
        {
            if (value is CornerPoint3D S)
            {
                SelectedToolPanel = new MoveControlsViewModel();
            }
            else
            {
                SelectedToolPanel = null;
            }
        }

        public VerletBuildResult buildResult { get; set; }

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

            ImportDoorModel("C:\\Users\\jacob\\AppData\\Roaming\\Net Designer\\3DModels\\FISHINGBOARD_SB.obj");
            var model = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;
            buildResult = new VerletBuildResult();

            var s = new CornerPoint3D() { LinkedNode = new Node3D(Vector3.Zero), Color = Color.Yellow , Radius = 0.01f};
            Shapes.Add(s);



            var node = new CornerPoint3D() { LinkedNode = model.ConnectionNodes.FirstOrDefault(), Color = Color.Green, Radius = 0.01f };
            Shapes.Add(node);

        }

        private async void ImportDoorModel(string filepath)
        {
            var model = new ObjModel3D(await ObjHelperClass.ImportAsMeshGeometry3D(filepath));
            model.Name = Path.GetFileNameWithoutExtension(filepath);

            model.ConnectionNodes.ForEach(x => Shapes.Add(new CornerPoint3D() { LinkedNode = x, Color = Color.Green, Radius = 0.01f}));
            shapes.Add(model);
            //ObjHelperClass.RotateGeometry(model, Vector3.UnitY, 60);
          
        }


        private void MeshExperiment()
        {
            var sphere = new CornerPoint3D() { LinkedNode = new Node3D(new Vector3(0,0,100)), Radius = 50 };
            (sphere as Shape3D).Draw();


            var elements = sphere.GetVerletElementsThreshold(sphere.Geometry as MeshGeometry3D);

            buildResult = new VerletBuildResult();
            elements.ForEach(x => buildResult.Elements.Add(x));
            buildResult.AddNodesToList();


            foreach (var element in buildResult.Elements) 
            {
                Shapes.Add(new Cylinder3D()
                {
                    Start = element.Start,
                    End = element.End,
                    Color = Color.Green,
                    Radius = 1
                });
            }


            foreach (var element in buildResult.Nodes)
            {
                Shapes.Add(new CornerPoint3D()
                {
                    LinkedNode = element,
                    Color = Color.Black,
                    Radius = 1
                });
            }

            Draw();




            
            
            
        }




        public string GetLastWordFromPath(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            string[] words = fileName.Split(' ');
            string lastWord = words[words.Length - 1];
            return lastWord;
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

                    foreach (var item in shapes)
                    {
                        if (item is ObjModel3D s)
                        {
                            var model = s as ObjModel3D;
                            s.RotateModelOnCenters(buildResult);
                        }



                    }

                    await uiDispatcher.InvokeAsync(() =>
                   {
                       Draw();
                       return Task.CompletedTask;


                   });
                    var elapsed = (int)stopWatch.ElapsedMilliseconds;
                    Debug.WriteLine(elapsed);
                    int delay = Math.Max(0, duration - elapsed);
                    await Task.Delay(300);
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
            for (int i = 0; i < 100; i++)
            {
                foreach (var s in buildResult.Elements)
                    UpdateSticks(s);

                foreach (var n in buildResult.Nodes)
                {

                    ConstrainGround(n);
                }
            }
        }








        [RelayCommand]
        void MoveToNextIteration()
        {
            continueVerlet = !continueVerlet;
        }
        void UpdateSticks(VerletElement3D stick)
        {

            var delta = stick.End.Position - stick.Start.Position;
            float deltaLength = delta.Length();
            float diff = (deltaLength - stick.Length);

            float damping = 0.1f;
            var offset = delta * (diff / deltaLength) / 2 * (1 - damping);

            stick.Start.Position += offset;
            stick.Start.Position = Vector3.Clamp(stick.Start.Position, new Vector3(float.MinValue, float.MinValue, float.MinValue), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));

            stick.End.Position -= offset;
            stick.End.Position = Vector3.Clamp(stick.End.Position, new Vector3(float.MinValue, float.MinValue, float.MinValue), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
        }
        public void ConstrainGround(Node3D node)
        {

            var xv = node.Position.X - node.OldPosition.X;  // x velocity
            var yv = node.Position.Y - node.OldPosition.Y;  // y velocity
            var zv = node.Position.Z - node.OldPosition.Z;  // z velocity

            var bounce = 0.99f;

            // Apply ground constraint
            if (node.Position.Z < 0)
            {
                var newPos = new Vector3(node.Position.X, node.Position.Y, Math.Max(node.Position.Z, 0));
                node.Position = newPos;
                node.Position = Vector3.Clamp(node.Position, new Vector3(float.MinValue, float.MinValue, 0), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));

                var oldPos = new Vector3(node.Position.X, node.Position.Y, Math.Max(node.Position.Z + zv * bounce, 0));
                node.OldPosition = oldPos;
                node.OldPosition = Vector3.Clamp(node.OldPosition, new Vector3(float.MinValue, float.MinValue, 0), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));


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

            // Clamp positions to representable range
            position = Vector3.Clamp(position, new Vector3(float.MinValue, float.MinValue, float.MinValue), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));


            node.Position = position;
            node.OldPosition = oldPosition;
        }


        [RelayCommand]
        public void Navigate(string destination)
        {
            switch (destination)
            {
                case "transformations":
                    SelectedToolPanel = Ioc.Default.GetService<CoordinationViewModel>();
                    break;
                case "verlet":
                    SelectedToolPanel = Ioc.Default.GetService<ItemsListViewModel>();
                    break;
              
                case "FloorTextures":
                    SelectedToolPanel = Ioc.Default.GetService<FloorTexturesViewModel>();
                    break;
                case "Close":
                    SelectedToolPanel = null;
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
        public void ObjectedClicked(Shape3D shape)
        {
            shape.Color = Color.Green;
            shape.Select();
            if (shape.IsSelected)
            {
                SelectedModel = shape;
            }
            else
            {
                SelectedModel = null;
            }
           
        }

        [RelayCommand]
        public async Task Reload(string floorNumber = "4")
        {
            continueVerlet = false;
            shapes.Clear();
           CreateFloor(floorNumber);
            
            InitBuildResult();

        }


        
        void CreateFloor(string num = "4")
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



            Draw();

        }

        public void ZoomTo()
        {
            var offset = 1;
            var model = shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;

            var rect3d = new Rect3D(ObjHelperClass.GetCentroid(model.BoundingPositions).ToPoint3D(), new Size3D(model.Width + offset, model.Height + offset, model.Depth + offset));
            MyViewPort.ZoomExtents(rect3d, 500);
            MyViewPort.Reset();

        }


        [RelayCommand]
        void Rotate()
        {
            var model = shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;
            ObjHelperClass.RotateGeometry(model, Vector3.UnitX, 90);
        }




        [ObservableProperty]
        ObservableCollection<IShape3D> shapes = new();

    }
}
