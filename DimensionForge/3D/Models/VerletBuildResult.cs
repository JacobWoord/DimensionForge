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
                if (!uniqueNodes.Any(n => n.Position == element.Start.Position))
                {
                    Nodes.Add(element.Start);
                    uniqueNodes.Add(element.Start);
                }

                if (!uniqueNodes.Any(n => n.Position == element.End.Position))
                {
                    Nodes.Add(element.End);
                    uniqueNodes.Add(element.End);
                }
            }

            Nodes.AddRange(uniqueNodes);
        }

        public void GetAllNodes()
        {

           

        }





    }
}
