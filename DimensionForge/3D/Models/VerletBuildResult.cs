using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using DimensionForge._3D.ViewModels;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public void GetAllElements()
        {

            var shapesList = viewModel.Shapes;

            for (int i = 0; i < shapesList.Count(); i++)
            {

                if (shapesList[i] is BatchedModel3D)
                {

                    var model = shapesList[i] as BatchedModel3D;
                    var elements = model.GetElements();

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

        public void AddNodesToList()
        {
            HashSet<Node3D> uniqueNodes = new HashSet<Node3D>(Nodes);

            foreach (verletElement3D element in Elements)
            {
                Node3D startNode = element.Start;
                Node3D endNode = element.End;

                if (!uniqueNodes.Contains(startNode) && !Nodes.Any(n => n.Position == startNode.Position))
                {
                    Nodes.Add(startNode);
                    uniqueNodes.Add(startNode);
                }

                if (!uniqueNodes.Contains(endNode) && !Nodes.Any(n => n.Position == endNode.Position))
                {
                    Nodes.Add(endNode);
                    uniqueNodes.Add(endNode);
                }
            }
        }



        public void GetAllNodes()
        {

           

        }





    }
}
