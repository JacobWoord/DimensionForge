using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace DimensionForge._2D.Behavior
{


        public static class CanvasLoadedBehavior
        {
            public static readonly DependencyProperty LoadedCommandProperty =
                DependencyProperty.RegisterAttached(
                    "LoadedCommand",
                    typeof(ICommand),
                    typeof(CanvasLoadedBehavior),
                    new PropertyMetadata(null, OnLoadedCommandChanged));

            public static ICommand GetLoadedCommand(Canvas canvas)
            {
                return (ICommand)canvas.GetValue(LoadedCommandProperty);
            }

            public static void SetLoadedCommand(Canvas canvas, ICommand value)
            {
                canvas.SetValue(LoadedCommandProperty, value);
            }

            private static void OnLoadedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                if (d is Canvas canvas)
                {
                    canvas.Loaded += (sender, args) =>
                    {
                        var command = GetLoadedCommand(canvas);
                        if (command != null && command.CanExecute(canvas))
                        {
                            command.Execute(canvas);
                        }
                    };
                }
            }
        }
    }
