
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

        public List<Node3D> CenterPosition { get; set; } = new();
        public List<VerletElement3D> Elements { get; set; } = new();

        public VerletBuildResult()
        {
            viewModel = Ioc.Default.GetService<Canvas3DViewModel>();

            GetAllObjElements();
            AddNodesToList();



        }

        public (float angle, Vector3 p1, Vector3 p2, Vector3 normal) SetRotationVertical(Vector3 tp)
        {
            var binnenLinks = Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft).Position;
            var binnenRechts = Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomRight).Position;

            var thirdPoint = tp;
            if (thirdPoint.Z < 0)
            {
                thirdPoint.Z = 1;
            }
            var plane = new Plane(binnenLinks, binnenRechts, thirdPoint);



            //take the normal to generate a rotation axis
            var normal = plane.Normal;
            if (normal.Z < 0)
            {
                normal.Z = 1;
            }
            //secondPoint -- secondPoint + Normal* 100
            var p1 = binnenLinks;
            var p2 = p1 + normal * 100;


            var angle = Utils3D.AngleBetweenAxes(binnenLinks, thirdPoint, binnenLinks, binnenRechts);
            // var angle = Utils3D.AngleBetweenAxes(firstPoint, thirdPoint, secondPoint, thirdPoint);

            return (angle, p1, p2, normal);
        }
        public (float angle, Vector3 p1, Vector3 p2, Vector3 normal) SetRotationHorizontal(Vector3 tp)
        {
            var binnenLinksOnder = Nodes.FirstOrDefault(x => x.NodePos == NodePosition.BottomLeft).Position;
            var binnenLinksBoven = Nodes.FirstOrDefault(x => x.NodePos == NodePosition.LeftTop).Position;

            var thirdPoint = tp;
            if (thirdPoint.Z < 0)
            {
                thirdPoint.Z = 1;
            }
            var plane = new Plane(binnenLinksOnder, binnenLinksBoven, thirdPoint);



            //take the normal to generate a rotation axis
            var normal = plane.Normal;

            //secondPoint -- secondPoint + Normal* 100
            var p1 = binnenLinksOnder;
            var p2 = p1 + normal * 100;


            var angle = Utils3D.AngleBetweenAxes(binnenLinksOnder, thirdPoint, binnenLinksOnder, binnenLinksBoven);
            // var angle = Utils3D.AngleBetweenAxes(firstPoint, thirdPoint, secondPoint, thirdPoint);

            return (angle, p1, p2, normal);
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
        private void SetCenter()
        {
           
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
     
        
        public Vector3 GetCenter(CornerName centerName)
        {
            // Define the center you want to retrieve from the functions as a parameter

            List<CornerName> planeCorners;

            switch (centerName)
            {
                case CornerName.FrontPlaneCenter:
                    planeCorners = new List<CornerName> { CornerName.TopFrontLeft, CornerName.TopFrontRight, CornerName.BottomFrontLeft, CornerName.BottomFrontRight };
                    break;
                case CornerName.BackPlaneCenter:
                    planeCorners = new List<CornerName> { CornerName.TopBackLeft, CornerName.TopBackRight, CornerName.BottomBackLeft, CornerName.BottomBackRight };
                    break;
                case CornerName.LeftPlaneCenter:
                    planeCorners = new List<CornerName> { CornerName.TopFrontLeft, CornerName.TopBackRight, CornerName.BottomFrontLeft, CornerName.BottomBackRight };
                    break;
                case CornerName.RightPlaneCenter:
                    planeCorners = new List<CornerName> { CornerName.TopFrontRight, CornerName.TopBackLeft, CornerName.BottomFrontRight, CornerName.BottomBackLeft};
                    break;
                case CornerName.TopPlaneCenter:
                    planeCorners = new List<CornerName> { CornerName.TopFrontLeft, CornerName.TopFrontRight, CornerName.TopBackLeft, CornerName.TopBackRight };
                    break;
                case CornerName.BottomPlaneCenter:
                    planeCorners = new List<CornerName> { CornerName.BottomFrontLeft, CornerName.BottomFrontRight, CornerName.BottomBackLeft, CornerName.BottomBackRight };
                    break;
                case CornerName.ModelCenter:
                    planeCorners = new List<CornerName> { CornerName.BottomFrontLeft, CornerName.BottomFrontRight, CornerName.BottomBackLeft, CornerName.BottomBackRight, CornerName.TopBackLeft, CornerName.TopBackRight, CornerName.TopFrontLeft, CornerName.TopFrontRight };
                    break;
                default:
                    
                    return Vector3.Zero;
            }

            var corners = Nodes.Where(x => planeCorners.Contains(x.CornerName)).ToList();
            return GetCentroid(corners);
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
        public Node3D GetNode(NodePosition nodeposition)
        {
            return Nodes.FirstOrDefault(x => x.NodePos == nodeposition);
        }




    }




}

