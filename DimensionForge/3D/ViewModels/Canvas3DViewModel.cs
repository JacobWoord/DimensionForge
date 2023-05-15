
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

            ImportDoorModel();

            buildResult = new VerletBuildResult();
            var doorModel = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;
            var boundingelements = doorModel.GetBoundingElements();
            boundingelements.ForEach(x => shapes.Add(x));


            //Draw the center points of the bounding elements
            var modelCenterPoints = doorModel.BoundingPositions.Where(x => x.CornerName == CornerName.TopPlaneCenter
            || x.CornerName == CornerName.FrontPlaneCenter
            || x.CornerName == CornerName.BottomPlaneCenter
            || x.CornerName == CornerName.LeftPlaneCenter
            || x.CornerName == CornerName.RightPlaneCenter
            || x.CornerName == CornerName.BackPlaneCenter).ToList();

            modelCenterPoints.ForEach(x => Shapes.Add(new CornerPoint3D() { LinkedNode = x, Color = Color.Yellow }));
           



            //var centerPositions = new List<Node3D>();
            //var centerNames = new[]
            //{
            //    CornerName.TopPlaneCenter,
            //    CornerName.BottomPlaneCenter,
            //    CornerName.LeftPlaneCenter,
            //    CornerName.RightPlaneCenter,
            //};
            //foreach (var centerName in centerNames)
            //{
            //    var center = buildResult.GetCenter(centerName);
            //    centerPositions.Add(new Node3D(center) { CornerName = centerName, UseCase = UseCase.verlet });
            //}

            //centerPositions.ForEach(x => Shapes.Add(new CornerPoint3D() { LinkedNode = x, Color = Color.Red }));


            //Draws the the elements from the buildresult class
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
        void UpdateCentersFromBox()
        {
            // Recalculates the positions of the centers each frame

            foreach (var shape in shapes)
            {
                if (shape is CornerPoint3D s && s.LinkedNode.UseCase == UseCase.verlet)
                {
                    var node = s.LinkedNode;
                    var newPos = buildResult.GetCenter(node.CornerName);
                    node.Position = newPos;
                }
                if (shape is AxisArrows3D a)
                {
                   var model =  Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;

                    a.CenterPosition.Position = ComputeBoxRotationHelper.GetCenter(model.BoundingPositions);
                }
            }
        }
        private async void ImportDoorModel()
        {
            var model = new ObjModel3D(await ObjHelperClass.ImportAsMeshGeometry3D("C:\\Users\\jacob\\AppData\\Roaming\\Net Designer\\3DModels\\FISHINGBOARD_SB.obj"));
            shapes.Add(model);
            ObjHelperClass.UpdatePosition(model, new Vector3(0, 0, 10));
        }

        [RelayCommand]
        public void AxisTest()
        {
            var doorModel = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;

            var FrontPlaneCenter = doorModel.BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.FrontPlaneCenter);
            var topCenter = doorModel.BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.TopPlaneCenter);
            var leftRight = doorModel.BoundingPositions.FirstOrDefault(x => x.CornerName == CornerName.RightPlaneCenter);
            var center = new Node3D(ComputeBoxRotationHelper.GetCenter(doorModel.BoundingPositions)){ UseCase = UseCase.verlet};

            var arrowFront = new AxisArrows3D(center, FrontPlaneCenter,Color.Green);
            var arrowTop = new AxisArrows3D(center, topCenter,Color.Red);
            var leftRightcenter = new AxisArrows3D(center, leftRight,Color.Blue);
           
            shapes.Add(arrowFront);
            shapes.Add(arrowTop);
            shapes.Add(leftRightcenter);
            

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

            await Task.Run(async () =>
            {
                while (continueVerlet)
                {
                    var stopWatch = Stopwatch.StartNew();

                    UpdatePhysics();
                    UpdateCentersFromBox();
                    UpdateModelPosition();

                    // the function below is to use the EMA filter to smooth the door position
                    // UpdateDoorPositionEMA();
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


        private void UpdateModelPosition()
        {
            //this function brings the model to the center of the verlet box

            var model = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;
            var center = ComputeBoxRotationHelper.GetCenter(buildResult.Nodes);
            ObjHelperClass.UpdatePosition(model, center);
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

        private void InitializeMeshSphere()
        {
            var s = new CornerPoint3D()
            {
                Color = Color.Transparent,
                LinkedNode = new Node3D(new Vector3(20, 20, 20)),
                Radius = 10,
            };

            //Shapes.Add(s);


            s.Draw();


            // Geometry logic
            s.Geometry.IsDynamic = true;
            var g = s.Geometry as MeshGeometry3D;
            var indices = g.Indices;
            var vertices = g.Positions;

            // Initialize new buildresult  and give the node elements to the buildresult
            buildResult = new VerletBuildResult();

            // Draw a new sphere for each node in the buildresult
            // buildResult.Nodes.ForEach(x => Shapes.Add(new CornerPoint3D() { LinkedNode = x, Color = Color.Purple }));
            //vertices.ForEach(x => buildResult.Nodes.Add(new Node3D(x)));
            //vertices.ForEach(x => Debug.WriteLine($"vertex pos: {x}"));

            var sphereElements = s.GetVerletElements(s.Geometry as MeshGeometry3D);
            buildResult.Elements = sphereElements;

            HashSet<string> addedNodeIds = new HashSet<string>();

            foreach (var e in sphereElements)
            {
                if (addedNodeIds.Add(e.Start.Id))
                {
                    buildResult.Nodes.Add(e.Start);
                }

                if (addedNodeIds.Add(e.End.Id))
                {
                    buildResult.Nodes.Add(e.End);
                }
            }

            foreach (var element in buildResult.Elements)
            {
                Shapes.Add(new Cylinder3D() { Start = element.Start, End = element.End, Color = element.Color });
            }
            for (int i = 0; i < buildResult.Nodes.Count(); i++)
            {
                Shapes.Add(new CornerPoint3D() { LinkedNode = buildResult.Nodes[i], Color = Color.Purple });

            }


            Draw();



        }






        [RelayCommand]
        public void UpdateDoorPositionEMA()
        {
            var door = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;

            // Calculate the translation vector based on the center positions of the models
            Vector3 verletCenter = ComputeBoxRotationHelper.GetCenter(buildResult.Nodes);
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
        void Rotate()
        {
            var door = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;
            ObjHelperClass.RotateGeometry(door, Vector3.UnitX, 60);
        }

        [RelayCommand]
        void DrawBoundingBox()
        {

            var door = Shapes.FirstOrDefault(x => x is ObjModel3D) as ObjModel3D;

            door.BoundingPositions.ForEach(x => shapes.Add(new CornerPoint3D() { Color = Color.Yellow, LinkedNode = x }));
            Draw();
        }

        [RelayCommand]
        void MoveToNextIteration()
        {
            continueVerlet = !continueVerlet;
        }
        void UpdateSticks(VerletElement3D stick)
        {
            try
            {
                var delta = stick.End.Position - stick.Start.Position;
                float deltaLength = delta.Length();
                float diff = (deltaLength - stick.Length);

                float damping = 0.1f;
                var offset = delta * (diff / deltaLength) / 2 * (1 - damping);

                stick.Start.Position += offset;
                stick.End.Position -= offset;

                // Clamp positions to representable range
                stick.Start.Position = Vector3.Clamp(stick.Start.Position, new Vector3(float.MinValue, float.MinValue, float.MinValue), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
                stick.End.Position = Vector3.Clamp(stick.End.Position, new Vector3(float.MinValue, float.MinValue, float.MinValue), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateSticks: {ex.Message}");
            }

        }
        public void ConstrainGround(Node3D node)
        {
            try
            {
                var xv = node.Position.X - node.OldPosition.X;  // x velocity
                var yv = node.Position.Y - node.OldPosition.Y;  // y velocity
                var zv = node.Position.Z - node.OldPosition.Z;  // z velocity

                var bounce = 0.99f;

                // Apply ground constraint
                if (node.Position.Z < 0)
                {
                    node.Position = new Vector3(node.Position.X, node.Position.Y, Math.Max(node.Position.Z, 0));
                    node.OldPosition = new Vector3(node.Position.X, node.Position.Y, Math.Max(node.Position.Z + zv * bounce, 0));
                }

                // Clamp positions to representable range
                node.Position = Vector3.Clamp(node.Position, new Vector3(float.MinValue, float.MinValue, 0), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
                node.OldPosition = Vector3.Clamp(node.OldPosition, new Vector3(float.MinValue, float.MinValue, 0), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ConstrainGround: {ex.Message}");
            }
        }

        void UpdatePositions(Node3D node)
        {

            try
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdatePositions: {ex.Message}");
            }

        }

        [RelayCommand]
        void Navigate(string destination)
        {
            switch (destination)
            {
                case "transformations":
                    SelectedToolPanel = Ioc.Default.GetService<CoordinationViewModel>();
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
            //InitBuildResult();
            InitBuildResult();

        }



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
