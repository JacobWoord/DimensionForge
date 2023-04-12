using DimensionForge._2D.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace DimensionForge._2D.ViewModels
{
    public partial class Canvas2DViewModel : ObservableObject
    {

        public Canvas2DViewModel()
        {

            var c = DrawCircle();
            var r = DrawRectangle();
            Shapes.Add(r);
            Shapes.Add(c);
        }


        [ObservableProperty]
        ObservableCollection<Shape2D> shapes = new();


        Circle2D DrawCircle()
        {
            var circle = new Circle2D();
            circle.Position = new Point(0, 0);
            circle.FillColor = System.Windows.Media.Color.FromRgb(255, 255, 0);
            circle.Diameter = 300f;

            return circle;
        }

        Rectangle2D DrawRectangle()
        {
            var rect = new Rectangle2D();
            rect.Width = 100;
            rect.Height = 100;
            rect.FillColor = Color.FromRgb(255, 255, 0);
            rect.Position = new Point(50, 50);

            return rect;

        }

    }
}
