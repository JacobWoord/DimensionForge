using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._2D.interfaces
{
    public interface IShape2D
    {

        Guid Id { get; set; }
        System.Drawing.Point Position { get; set; }
        System.Drawing.Color FillColor { get; set; }
        System.Drawing.Color StrokeColor { get; set; }
        float StrokeThickness { get; set; } 
    }
}
