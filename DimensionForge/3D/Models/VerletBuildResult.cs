
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

                if (shapesList[i] is BatchedModel3D)
                {

                    var model = shapesList[i] as BatchedModel3D;
                    var elements = model.GetBbElements();
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


        public Node3D GetNode(NodePosition nodeposition)
        {
            return Nodes.FirstOrDefault(x => x.NodePos == nodeposition);
        }


        public Vector3 GetOrientation1()
        {
            //returns the orientation of the verlet shape

            var topTrianle = Nodes.Where(n => n.NodePos == NodePosition.LeftTop || n.NodePos == NodePosition.MiddleTop || n.NodePos == NodePosition.RightTop).Select(n => n.Position).ToArray();
            var bottomTriangle = Nodes.Where(n => n.NodePos == NodePosition.BottomLeft || n.NodePos == NodePosition.MiddleBottom || n.NodePos == NodePosition.BottomRight).Select(n => n.Position).ToArray();

           var topCentroid =  Utils3D.GetCentroidPosition(topTrianle);
           var bottomCentroid = Utils3D.GetCentroidPosition(bottomTriangle);

            var orientation = bottomCentroid - topCentroid;

            return orientation;
        }
        public Vector3 GetOrientation()
        {
            var topCorners = Nodes.Where(n => n.CornerName == CornerName.UpperLeftFrontCorner
            || n.CornerName == CornerName.UpperRightFrontCorner
            || n.CornerName == CornerName.UpperLeftBackCorner
            || n.CornerName == CornerName.UpperRightBackCorner).ToArray();

            var bottomCorners = Nodes.Where(n => n.CornerName == CornerName.LowerLeftFrontCorner
           || n.CornerName == CornerName.LowerRightFrontCorner
           || n.CornerName == CornerName.LowerLeftBackCorner
           || n.CornerName == CornerName.LowerRightBackCorner).ToArray();

            var topCornerPositions = topCorners.Select(n => n.Position).ToArray();
            var bottomCornerPositions = bottomCorners.Select(n => n.Position).ToArray();


            var topCentroid = Utils3D.GetCentroidPosition(topCornerPositions);
            var bottomCentroid = Utils3D.GetCentroidPosition(bottomCornerPositions);

            var orientation = bottomCentroid - topCentroid;

            return orientation;
        }

        public Vector3 GetHorizontalOrientation()
        {
            var frontFaceCorners = Nodes.Where(n => n.CornerName == CornerName.UpperLeftFrontCorner
          || n.CornerName == CornerName.UpperRightFrontCorner
          || n.CornerName == CornerName.LowerLeftFrontCorner
          || n.CornerName == CornerName.LowerRightFrontCorner).ToArray();

            var backFaceCorners = Nodes.Where(n => n.CornerName == CornerName.UpperLeftBackCorner
       || n.CornerName == CornerName.UpperRightBackCorner
       || n.CornerName == CornerName.LowerLeftBackCorner
       || n.CornerName == CornerName.LowerRightBackCorner).ToArray();

            var frontFaceCornersPositions = frontFaceCorners.Select(n => n.Position).ToArray();
            var backFaceCornersPositions = backFaceCorners.Select(n => n.Position).ToArray();

            Vector3 frontCentroid = Utils3D.GetCentroidPosition(frontFaceCornersPositions);
            Vector3 backCentroid = Utils3D.GetCentroidPosition(backFaceCornersPositions);

            var orientation = frontCentroid - backCentroid;

            return orientation;


        }




    }
}
