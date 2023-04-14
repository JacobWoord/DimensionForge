
using System.Windows;
using DimensionForge._3D.ViewModels;
using DimensionForge.Main.ViewModels;
using HelixToolkit.Wpf.SharpDX;
using Microsoft.Xaml.Behaviors;

namespace DimensionForge._3D.viewport3DXExtensions
{

    public class Viewport3DXBehavior : Behavior<Viewport3DX>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnViewportLoaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnViewportLoaded;
            base.OnDetaching();
        }

        private void OnViewportLoaded(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.DataContext is Canvas3DViewModel viewModel)
            {
                viewModel.OnViewportInitialized(AssociatedObject);
            }
        }
    }

}
