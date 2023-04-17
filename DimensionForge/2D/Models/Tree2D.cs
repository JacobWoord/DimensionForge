
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;
//using System.Windows.Shapes;

//namespace DimensionForge._2D.Models
//{
//    public partial class Tree : Shape2D
//    {
//        private int depth;
//        private double angle;
//        private double branchLength;
//        private double branchWidth;
//        private SolidColorBrush branchColor;

//        public Tree(int depth, double angle, double branchLength, double branchWidth, SolidColorBrush branchColor)
//        {
//            this.depth = depth;
//            this.angle = angle;
//            this.branchLength = branchLength;
//            this.branchWidth = branchWidth;
//            this.branchColor = branchColor;
//        }

//        public void Draw(Point startPoint, double currentAngle, double currentLength, double currentWidth)
//        {
//            if (depth == 0)
//            {
//                // Draw a leaf at the end of the branch
//                Ellipse leaf = new Ellipse();
//                leaf.Width = currentWidth * 2;
//                leaf.Height = currentWidth * 2;
//                leaf.Fill = Brushes.Green;
//                Canvas.SetLeft(leaf, startPoint.X - currentWidth);
//                Canvas.SetTop(leaf, startPoint.Y - currentWidth);

//                canvas.Children.Add(leaf);
//            }
//            else
//            {
//                // Draw a branch
//                Point endPoint = new Point(startPoint.X + currentLength * Math.Cos(currentAngle), startPoint.Y + currentLength * Math.Sin(currentAngle));
//                Line branch = new Line();
//                branch.X1 = startPoint.X;
//                branch.Y1 = startPoint.Y;
//                branch.X2 = endPoint.X;
//                branch.Y2 = endPoint.Y;
//                branch.StrokeThickness = currentWidth;
//                branch.Stroke = branchColor;

//                canvas.Children.Add(branch);

//                // Draw two sub-branches
//                double newLength = currentLength * 0.8;
//                double newWidth = currentWidth * 0.7;
//                Draw(endPoint, currentAngle - angle, newLength, newWidth);
//                Draw(endPoint, currentAngle + angle, newLength, newWidth);
//            }
//        }
//    }
//}

