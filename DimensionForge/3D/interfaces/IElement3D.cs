using DimensionForge._3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._3D.interfaces
{
    public interface IElement3D
    {

        public Node3D Start { get; set; }

        public Node3D End { get; set; }

        public float Length { get; set; }

  


    }
}
