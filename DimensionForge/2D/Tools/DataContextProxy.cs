using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace DimensionForge._2D.Tools
{
    public class DataContextProxy : FrameworkElement
    {
        public static new readonly DependencyProperty DataContextProperty = FrameworkElement.DataContextProperty.AddOwner(typeof(DataContextProxy));

        public new object DataContext
        {
            get => GetValue(DataContextProperty);
            set => SetValue(DataContextProperty, value);
        }

        public Canvas CanvasElement => (Canvas)GetValue(CanvasElementProperty);

        public static readonly DependencyProperty CanvasElementProperty =
            DependencyProperty.Register("CanvasElement", typeof(Canvas), typeof(DataContextProxy));
    }
}
