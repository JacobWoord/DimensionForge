using System.Windows;
using System.Windows.Input;
using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using Microsoft.Xaml.Behaviors;

namespace  DimensionForge._3D.Behaviors
{
    public class ClickBehavior : Behavior<Viewport3DX>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ClickBehavior), new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseDown += OnMouseDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseDown -= OnMouseDown;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(AssociatedObject);
            var hits = AssociatedObject.FindHits(point);

            foreach (var hit in hits)
            {
                if (hit.ModelHit is MeshGeometryModel3D m) 
                {
                    var clickedObject = m;
                    if (m.DataContext is IShape3D c) 
                    {
                        var cyl = c;

                        Command.Execute(cyl);

                   
                    }

                }

              
                 
                
            }
        }
    }

}
