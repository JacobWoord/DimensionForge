
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

        public List<VerletElement3D> Elements { get; set; } = new();

        public VerletBuildResult()
        {
            viewModel = Ioc.Default.GetService<Canvas3DViewModel>();

            GetAllObjElements();
            AddNodesToList();




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
                    var elements = model.GetVerletElements(model.Id);

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






        public Node3D GetCenter(CornerName centerName, string modelId)
        {
            List<Node3D> corners;
            switch (centerName)
            {
                case CornerName.FrontPlaneCenter:
                    corners = Nodes.Where(x =>
                        x.CornerName == CornerName.TopFrontRight && x.ModelId == modelId ||
                        x.CornerName == CornerName.TopFrontLeft && x.ModelId == modelId ||
                        x.CornerName == CornerName.BottomFrontRight && x.ModelId == modelId ||
                        x.CornerName == CornerName.BottomFrontLeft && x.ModelId == modelId).ToList();
                    break;
                case CornerName.BackPlaneCenter:
                    corners = Nodes.Where(x =>
                        x.CornerName == CornerName.TopBackRight && x.ModelId == modelId ||
                        x.CornerName == CornerName.TopBackLeft && x.ModelId == modelId ||
                        x.CornerName == CornerName.BottomBackRight && x.ModelId == modelId ||
                        x.CornerName == CornerName.BottomBackLeft && x.ModelId == modelId).ToList();
                    break;
                case CornerName.RightPlaneCenter:
                    corners = Nodes.Where(x =>
                        x.CornerName == CornerName.TopBackRight && x.ModelId == modelId ||
                        x.CornerName == CornerName.TopFrontLeft && x.ModelId == modelId ||
                        x.CornerName == CornerName.BottomBackRight && x.ModelId == modelId  ||
                        x.CornerName == CornerName.BottomFrontLeft && x.ModelId == modelId).ToList();
                    break;
                case CornerName.LeftPlaneCenter:
                    corners = Nodes.Where(x =>
                        x.CornerName == CornerName.TopBackLeft && x.ModelId == modelId ||
                        x.CornerName == CornerName.TopFrontRight && x.ModelId == modelId ||
                        x.CornerName == CornerName.BottomBackLeft && x.ModelId == modelId ||
                        x.CornerName == CornerName.BottomFrontRight && x.ModelId == modelId).ToList();
                    break;
                case CornerName.TopPlaneCenter:
                    corners = Nodes.Where(x =>
                        x.CornerName == CornerName.TopBackLeft && x.ModelId == modelId ||
                        x.CornerName == CornerName.TopBackRight && x.ModelId == modelId ||
                        x.CornerName == CornerName.TopFrontLeft && x.ModelId == modelId ||
                        x.CornerName == CornerName.TopFrontRight && x.ModelId == modelId).ToList();
                    break;
                case CornerName.BottomPlaneCenter:
                    corners = Nodes.Where(x =>
                        x.CornerName == CornerName.BottomBackLeft && x.ModelId == modelId ||
                        x.CornerName == CornerName.BottomBackRight && x.ModelId == modelId ||
                        x.CornerName == CornerName.BottomFrontLeft && x.ModelId == modelId ||
                        x.CornerName == CornerName.BottomFrontRight && x.ModelId == modelId).ToList();
                    break;
                case CornerName.ModelCenter:
                    corners = Nodes.Where(x => x.CornerName == CornerName.TopBackRight && x.ModelId == modelId
                      || x.CornerName == CornerName.TopBackLeft && x.ModelId == modelId
                      || x.CornerName == CornerName.BottomBackRight && x.ModelId == modelId
                      || x.CornerName == CornerName.BottomBackLeft && x.ModelId == modelId
                      || x.CornerName == CornerName.TopFrontRight && x.ModelId == modelId
                      || x.CornerName == CornerName.TopFrontLeft && x.ModelId == modelId
                      || x.CornerName == CornerName.BottomFrontLeft && x.ModelId == modelId
                      || x.CornerName == CornerName.BottomFrontRight && x.ModelId == modelId).ToList();
                    break;
                default:
                    throw new ArgumentException("Invalid centerName.");
            }

            var center = GetCentroid(corners);
            return new Node3D(center)
            {
                CornerName = centerName
            };
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

