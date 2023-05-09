using DimensionForge._3D.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge.HelperTools
{
    internal class TransformGroupConverters
    {


        public void ConvertTransform3DGroupToTransformData()
        {
            // Initialize default values
            //Vector3D translation = new Vector3D(0, 0, 0);
            //Vector3D scale = new Vector3D(1, 1, 1);
            //Quaternion rotation = new Quaternion(0, 0, 0, 1);


            //// Iterate through the children of the Transform3DGroup and extract the relevant data
            //if (transform is not null)
            //    foreach (var transform in transform.Children)
            //    {
            //        if (transform is TranslateTransform3D translateTransform)
            //        {
            //            translation = new Vector3D(translateTransform.OffsetX, translateTransform.OffsetY, translateTransform.OffsetZ);
            //        }
            //        else if (transform is ScaleTransform3D scaleTransform)
            //        {
            //            scale = new Vector3D(scaleTransform.ScaleX, scaleTransform.ScaleY, scaleTransform.ScaleZ);
            //        }
            //        else if (transform is RotateTransform3D rotateTransform)
            //        {
            //            if (rotateTransform.Rotation is AxisAngleRotation3D axisAngleRotation)
            //            {
            //                Vector3D axis = new Vector3D(
            //                    axisAngleRotation.Axis.X,
            //                    axisAngleRotation.Axis.Y,
            //                    axisAngleRotation.Axis.Z);
            //                double angle = axisAngleRotation.Angle;
            //                rotation = new Quaternion(
            //                    axis.X * Math.Sin(angle / 2),
            //                    axis.Y * Math.Sin(angle / 2),
            //                    axis.Z * Math.Sin(angle / 2),
            //                    Math.Cos(angle / 2));
            //            }

            //        }
            //        TransformDatas.Add(new TransformData(translation, scale, rotation));
            //    }

        }
        public void ConvertTransformDataToTransform3DGroup()
        {
            //Transform3DGroup transformGroup = new Transform3DGroup();

            //foreach (var data in TransformDatas)
            //{
            //    // Add the translation transformation
            //    TranslateTransform3D translation = new TranslateTransform3D(
            //        data.Translation.X,
            //        data.Translation.Y,
            //        data.Translation.Z);
            //    transformGroup.Children.Add(translation);

            //    // Add the scale transformation
            //    ScaleTransform3D scale = new ScaleTransform3D(
            //        data.Scale.X,
            //        data.Scale.Y,
            //        data.Scale.Z);
            //    transformGroup.Children.Add(scale);

            //    // Add the rotation transformation
            //  //  Quaternion rotation = data.Rotation;
            //    Vector3D axis = new Vector3D(rotation.X, rotation.Y, rotation.Z);
            //    double angle = 2 * Math.Acos(rotation.W);
            //    AxisAngleRotation3D axisAngleRotation = new AxisAngleRotation3D(axis, angle);
            //    RotateTransform3D rotateTransform = new RotateTransform3D(axisAngleRotation);
            //    transformGroup.Children.Add(rotateTransform);
            //}
            //transform = transformGroup;

        }


        public void ConvertTranformations()
        {
            //foreach (var shape in shapes)
            //    shape.ConvertTransform3DGroupToTransformData();
        }
        public void ConvertTransformationsBack()
        {
            //foreach (var shape in shapes)
            //{
            //    shape.ConvertTransform3DGroupToTransformData();
            //}
        }


    }
}
