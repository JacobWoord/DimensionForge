using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DimensionForge._2D.Behavior
{
    public partial class CanvasElementBehavior : Behavior<Canvas>
    {
        public static readonly DependencyProperty CallFunctionProperty =
    DependencyProperty.Register("CallFunction", typeof(Action<Canvas>), typeof(CanvasElementBehavior));

        public Action<Canvas> CallFunction
        {
            get { return (Action<Canvas>)GetValue(CallFunctionProperty); }
            set { SetValue(CallFunctionProperty, value); }
        }


        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnCanvasLoaded;
        }

        private void OnCanvasLoaded(object sender, RoutedEventArgs e)
        {
            CallFunction?.Invoke(AssociatedObject);
        }


    }
}
