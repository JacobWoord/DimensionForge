using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._3D.Models
{
    public partial class VerletBox3D : ObservableObject
    {
        [ObservableProperty]
        List<Node3D> cornerList = new List<Node3D>();

        [ObservableProperty]
        List<verletElement3D> elementList = new();



        public VerletBox3D()
        {
            
        }
    }
}
