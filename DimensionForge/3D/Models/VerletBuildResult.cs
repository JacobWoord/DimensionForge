
using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using DimensionForge._3D.ViewModels;
using DimensionForge.Common;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using Net_Designer_MVVM;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge
{
    public partial class VerletBuildResult : ObservableObject
    {

        private Canvas3DViewModel viewModel;
        public List<Node3D> Nodes { get; set; } = new();
        public List<Node3D> CenterPositions { get; set; } = new();
        public List<VerletElement3D> Elements { get; set; } = new();

        public VerletBuildResult()
        {
            viewModel = Ioc.Default.GetService<Canvas3DViewModel>();

            GetAllObjElements();
            AddNodesToList();


            CenterPositions = SetCenter();


        }
        public void AddNodesToList()
        {
            // adding the unique node of each element to the verlet buildresult list
            foreach (VerletElement3D element in Elements)
            {
                Node3D startNode = element.Start;
                Node3D endNode = element.End;

                if (!Nodes.Any(n => n.Id == startNode.Id))
                {
                    Nodes.Add(startNode);
                }

                if (!Nodes.Any(n => n.Id == endNode.Id))
                {
                    Nodes.Add(endNode);
                }


            }
        }

        public void GetAllObjElements()
        {

            var shapesList = viewModel.Shapes;

            for (int i = 0; i < shapesList.Count(); i++)
            {

                if (shapesList[i] is ObjModel3D)
                {

                    var model = shapesList[i] as ObjModel3D;
                    var elements = model.GetVerletElements();

                    foreach (var el in elements)
                    {
                        Elements.Add(el);
                    }

                }
                else
                {
                    continue;
                }
            }
        }

        public Node3D GetCenter(CornerName centerName)
        {
            // Define the center you want to retrieve from the functions as a parameter
            UpdateCenterPositions();


            Node3D center;

            switch (centerName)
            {
                case CornerName.FrontPlaneCenter:
                    center = CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.FrontPlaneCenter);
                    break;
                case CornerName.BackPlaneCenter:
                    center = CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.BackPlaneCenter);

                    break;
                case CornerName.LeftPlaneCenter:
                    center = CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.LeftPlaneCenter);

                    break;
                case CornerName.RightPlaneCenter:
                    center = CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.RightPlaneCenter);

                    break;
                case CornerName.TopPlaneCenter:
                    center = CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.TopPlaneCenter);

                    break;
                case CornerName.BottomPlaneCenter:
                    center = CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.BottomPlaneCenter);
                    break;
                case CornerName.ModelCenter:
                    center = CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.ModelCenter);

                    break;
                default: return null;

            }

            return center;
        }

        public List<Node3D> SetCenter()
        {


            var newCenterPositions = new List<Node3D>();

            // Calculate center points for each face of the bounding box

            Vector3 frontCenter = GetCentroid(Nodes.Where(x => x.CornerName == CornerName.TopFrontRight
            || x.CornerName == CornerName.TopFrontLeft
            || x.CornerName == CornerName.BottomFrontRight
            || x.CornerName == CornerName.BottomFrontLeft).ToList());

            Vector3 backCenter = GetCentroid(Nodes.Where(x => x.CornerName == CornerName.TopBackRight
           || x.CornerName == CornerName.TopBackLeft
           || x.CornerName == CornerName.BottomBackRight
           || x.CornerName == CornerName.BottomBackLeft).ToList());


            Vector3 rightCenter = GetCentroid(Nodes.Where(x => x.CornerName == CornerName.TopBackRight
             || x.CornerName == CornerName.TopFrontLeft
             || x.CornerName == CornerName.BottomBackRight
             || x.CornerName == CornerName.BottomFrontLeft).ToList());


            Vector3 leftCenter = GetCentroid(Nodes.Where(x => x.CornerName == CornerName.TopBackLeft
              || x.CornerName == CornerName.TopFrontRight
              || x.CornerName == CornerName.BottomBackLeft
              || x.CornerName == CornerName.BottomFrontRight).ToList());



            Vector3 topCenter = GetCentroid(Nodes.Where(x => x.CornerName == CornerName.TopBackLeft
                || x.CornerName == CornerName.TopBackRight
                || x.CornerName == CornerName.TopFrontLeft
                || x.CornerName == CornerName.TopFrontRight).ToList());

            Vector3 bottomCenter = GetCentroid(Nodes.Where(x => x.CornerName == CornerName.BottomBackLeft
                || x.CornerName == CornerName.BottomBackRight
                || x.CornerName == CornerName.BottomFrontLeft
                || x.CornerName == CornerName.BottomFrontRight).ToList());


            Vector3 modelCenter = GetCentroid(Nodes.Where(x => x.CornerName == CornerName.TopBackRight
           || x.CornerName == CornerName.TopBackLeft
           || x.CornerName == CornerName.BottomBackRight
           || x.CornerName == CornerName.BottomBackLeft
           || x.CornerName == CornerName.TopFrontRight
           || x.CornerName == CornerName.TopFrontLeft
           || x.CornerName == CornerName.BottomFrontLeft
           || x.CornerName == CornerName.BottomFrontRight).ToList());



            // Add center points to the list
            newCenterPositions.Add(new Node3D(frontCenter) { CornerName = CornerName.FrontPlaneCenter });
            newCenterPositions.Add(new Node3D(backCenter) { CornerName = CornerName.BackPlaneCenter });
            newCenterPositions.Add(new Node3D(leftCenter) { CornerName = CornerName.LeftPlaneCenter });
            newCenterPositions.Add(new Node3D(rightCenter) { CornerName = CornerName.RightPlaneCenter });
            newCenterPositions.Add(new Node3D(topCenter) { CornerName = CornerName.TopPlaneCenter });
            newCenterPositions.Add(new Node3D(bottomCenter) { CornerName = CornerName.BottomPlaneCenter });
            newCenterPositions.Add(new Node3D(modelCenter) { CornerName = CornerName.ModelCenter });

            return newCenterPositions;
        }

        public void UpdateCenterPositions()
        {
            try
            {
                var frontCorners = Nodes.Where(x => x.CornerName == CornerName.TopFrontRight
               || x.CornerName == CornerName.TopFrontLeft
               || x.CornerName == CornerName.BottomFrontRight
               || x.CornerName == CornerName.BottomFrontLeft).ToList();
                var frontCenter = GetCentroid(frontCorners);

                if (!float.IsInfinity(frontCenter.X) && !float.IsInfinity(frontCenter.Y) && !float.IsInfinity(frontCenter.Z) &&
                        !float.IsNaN(frontCenter.X) && !float.IsNaN(frontCenter.Y) && !float.IsNaN(frontCenter.Z))
                {
                    CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.FrontPlaneCenter).Position = frontCenter;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("frontCenter");

            }



            try
            {
                var backCorners = Nodes.Where(x => x.CornerName == CornerName.TopBackRight
                 || x.CornerName == CornerName.TopBackLeft
                 || x.CornerName == CornerName.BottomBackRight
                 || x.CornerName == CornerName.BottomBackLeft).ToList();
                var backCenter = GetCentroid(backCorners);

                if (!float.IsInfinity(backCenter.X) && !float.IsInfinity(backCenter.Y) && !float.IsInfinity(backCenter.Z) &&
                        !float.IsNaN(backCenter.X) && !float.IsNaN(backCenter.Y) && !float.IsNaN(backCenter.Z))
                {
                    CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.BackPlaneCenter).Position = backCenter;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("backCenter");

            }


            try
            {
                var rightCorners = Nodes.Where(x => x.CornerName == CornerName.TopBackRight
                 || x.CornerName == CornerName.TopFrontLeft
                 || x.CornerName == CornerName.BottomBackRight
                 || x.CornerName == CornerName.BottomFrontLeft).ToList();
                var rightCenter = GetCentroid(rightCorners);

                if (!float.IsInfinity(rightCenter.X) && !float.IsInfinity(rightCenter.Y) && !float.IsInfinity(rightCenter.Z) &&
                        !float.IsNaN(rightCenter.X) && !float.IsNaN(rightCenter.Y) && !float.IsNaN(rightCenter.Z))
                {
                    CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.RightPlaneCenter).Position = rightCenter;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("rightCenter");

            }

            try
            {
                var leftCorners = Nodes.Where(x => x.CornerName == CornerName.TopBackLeft
                  || x.CornerName == CornerName.TopFrontRight
                  || x.CornerName == CornerName.BottomBackLeft
                  || x.CornerName == CornerName.BottomFrontRight).ToList();
                var leftCenter = GetCentroid(leftCorners);

                if (!float.IsInfinity(leftCenter.X) && !float.IsInfinity(leftCenter.Y) && !float.IsInfinity(leftCenter.Z) &&
                      !float.IsNaN(leftCenter.X) && !float.IsNaN(leftCenter.Y) && !float.IsNaN(leftCenter.Z))
                {
                    CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.LeftPlaneCenter).Position = leftCenter;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("leftCenter");

            }


            try
            {
                var topCorners = Nodes.Where(x => x.CornerName == CornerName.TopBackLeft
                     || x.CornerName == CornerName.TopBackRight
                     || x.CornerName == CornerName.TopFrontLeft
                     || x.CornerName == CornerName.TopFrontRight).ToList();
                var topCenter = GetCentroid(topCorners);

                if (!float.IsInfinity(topCenter.X) && !float.IsInfinity(topCenter.Y) && !float.IsInfinity(topCenter.Z) &&
                    !float.IsNaN(topCenter.X) && !float.IsNaN(topCenter.Y) && !float.IsNaN(topCenter.Z))
                {
                    CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.TopPlaneCenter).Position = topCenter;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("topCenter");

            }
            try
            {
                var bottomCorners = Nodes.Where(x => x.CornerName == CornerName.BottomBackLeft
                    || x.CornerName == CornerName.BottomBackRight
                    || x.CornerName == CornerName.BottomFrontLeft
                    || x.CornerName == CornerName.BottomFrontRight).ToList();
                var bottomCenter = GetCentroid(bottomCorners);

                if (!float.IsInfinity(bottomCenter.X) && !float.IsInfinity(bottomCenter.Y) && !float.IsInfinity(bottomCenter.Z) &&
                   !float.IsNaN(bottomCenter.X) && !float.IsNaN(bottomCenter.Y) && !float.IsNaN(bottomCenter.Z))
                {
                    CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.BottomPlaneCenter).Position = bottomCenter;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("bottomCenter");
            }

            try
            {
                var allCorners = Nodes.Where(x => x.CornerName == CornerName.TopBackRight
               || x.CornerName == CornerName.TopBackLeft
               || x.CornerName == CornerName.BottomBackRight
               || x.CornerName == CornerName.BottomBackLeft
               || x.CornerName == CornerName.TopFrontRight
               || x.CornerName == CornerName.TopFrontLeft
               || x.CornerName == CornerName.BottomFrontLeft
               || x.CornerName == CornerName.BottomFrontRight).ToList();
                var modelCenter = GetCentroid(allCorners);

                if (!float.IsInfinity(modelCenter.X) && !float.IsInfinity(modelCenter.Y) && !float.IsInfinity(modelCenter.Z) &&
                 !float.IsNaN(modelCenter.X) && !float.IsNaN(modelCenter.Y) && !float.IsNaN(modelCenter.Z))
                {
                    CenterPositions.FirstOrDefault(x => x.CornerName == CornerName.ModelCenter).Position = modelCenter;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("modelCenter");

            }


        }

        public void GetExperimentElements()
        {

            var shapesList = viewModel.Shapes;

            for (int i = 0; i < shapesList.Count(); i++)
            {

                if (shapesList[i] is CornerPoint3D)
                {

                    var model = shapesList[i] as CornerPoint3D;
                    var elements = model.GetVerletElements(model.Geometry as MeshGeometry3D);
                    // var elements = model.GetElements();
                    foreach (var el in elements)
                    {
                        Elements.Add(el);
                    }

                }
                else
                {
                    continue;
                }
            }
        }






        public Vector3 GetCentroid(List<Node3D> nodes)
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
        public float GetLength()
        {
            var p = Nodes.Where(x => x.CornerName == CornerName.TopBackLeft || x.CornerName == CornerName.BottomBackLeft).ToArray();
            var length = Vector3.Distance(p[1].Position, p[0].Position);
            return length;
        }
     




    }




}

