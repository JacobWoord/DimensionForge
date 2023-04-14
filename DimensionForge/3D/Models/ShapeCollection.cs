using DimensionForge._3D.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._3D.Models
{
    public class ShapeCollection
    {
        private List<IShape3D> shapes = new List<IShape3D>();

        public void AddShape(IShape3D shape)
        {
            shapes.Add(shape);
        }

        public void RemoveShape(IShape3D shape)
        {
            shapes.Remove(shape);
        }

        public IReadOnlyList<IShape3D> GetShapes()
        {
            return shapes.AsReadOnly();
        }
    }
}
