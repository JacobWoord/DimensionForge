using DimensionForge._3D.Models;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace DimensionForge.HelperTools
{
    public static class Transformations
    {

        public static void RotateAroundCenter(Transform3DGroup transform,Vector3D axis, double angle,List<Node3D> nodes)
        {
            // Get the center point of the model
            // Vector3 center = this.GetModelCenter();

            //// Translate the model to the origin
            //Transform3D translateToOrigin = new TranslateTransform3D(-center.X, -center.Y, -center.Z);

            //// Apply the rotation transform
            //Transform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(axis, angle));

            //// Translate the model back to its original position
            //Transform3D translateBack = new TranslateTransform3D(center.X, center.Y, center.Z);

            //transform.Children.Add(translateToOrigin);
            //transform.Children.Add(rotateTransform);
            //transform.Children.Add(translateBack);

            //if (Bbcorners is not null)
            //{
            //    Bbcorners.ForEach(cornerNode => cornerNode.transform = this.transform);
            //}
        }

    }
}
