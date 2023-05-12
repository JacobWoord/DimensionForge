
using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using DimensionForge._3D.ViewModels;
using DimensionForge.Common;
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
        public List<verletElement3D> Elements { get; set; } = new();

      
        

        public VerletBuildResult()
        {
            viewModel = Ioc.Default.GetService<Canvas3DViewModel>();

            GetAllElements();
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
            foreach (verletElement3D element in Elements)
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
        public void GetAllElements()
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

