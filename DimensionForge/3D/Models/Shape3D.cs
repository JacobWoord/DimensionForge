using DimensionForge._2D.interfaces;
using DimensionForge._3D.interfaces;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;

using DimensionForge._3D.Data;
using Newtonsoft.Json;
using Transform3DGroup = System.Windows.Media.Media3D.Transform3DGroup;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using ScaleTransform3D = System.Windows.Media.Media3D.ScaleTransform3D;
using RotateTransform3D = System.Windows.Media.Media3D.RotateTransform3D;
using Quaternion = System.Windows.Media.Media3D.Quaternion;

using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Material = HelixToolkit.Wpf.SharpDX.Material;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using DimensionForge.Common;

namespace DimensionForge._3D.Models
{
    public partial class Shape3D : ObservableObject, IShape3D
    {
        public string Name { get; set; }    
        public string Id { get; set; }
        public Color4 Color { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 OldPosition { get; set; }
        public bool IsSelected { get; set; }

        public Shape3D()
        {
            transform = new System.Windows.Media.Media3D.Transform3DGroup();

            Id = Guid.NewGuid().ToString();

            material = SetMaterial();

            TransformDatas = new List<TransformData>();

        }

        public IList<TransformData> TransformDatas { get; set; }
        public UseCase UseCase { get; set; }

        [ObservableProperty]
        [property: JsonIgnore]
        Material material;

        [ObservableProperty]
        [property: JsonIgnore]
        HelixToolkit.SharpDX.Core.Geometry3D geometry;

        [ObservableProperty]
        [property: JsonIgnore]
        Transform3DGroup transform;

        public virtual void Draw()
        {


        }
        public void ScaleModel(double scaleFactor)
        {
            ScaleTransform3D scaleTransform = new ScaleTransform3D(scaleFactor, scaleFactor, scaleFactor);

            if (transform == null)
            {

                Transform = new Transform3DGroup();
            }


            Transform.Children.Add(scaleTransform);


        }
        public void Rotate(Vector3D Axis, double Angle)
        {
            var trans = new RotateTransform3D(
                 new AxisAngleRotation3D(
                     axis: Axis,
                     angle: Angle));

            Transform.Children.Add(trans);
        }
        public void ConvertTransform3DGroupToTransformData()
        {
            // Initialize default values
            Vector3D translation = new Vector3D(0, 0, 0);
            Vector3D scale = new Vector3D(1, 1, 1);
            Quaternion rotation = new Quaternion(0, 0, 0, 1);


            // Iterate through the children of the Transform3DGroup and extract the relevant data
            foreach (var transform in transform.Children)
            {
                if (transform is TranslateTransform3D translateTransform)
                {
                    translation = new Vector3D(translateTransform.OffsetX, translateTransform.OffsetY, translateTransform.OffsetZ);
                }
                else if (transform is ScaleTransform3D scaleTransform)
                {
                    scale = new Vector3D(scaleTransform.ScaleX, scaleTransform.ScaleY, scaleTransform.ScaleZ);
                }
                else if (transform is RotateTransform3D rotateTransform)
                {
                    if (rotateTransform.Rotation is AxisAngleRotation3D axisAngleRotation)
                    {
                        Vector3D axis = new Vector3D(
                            axisAngleRotation.Axis.X,
                            axisAngleRotation.Axis.Y,
                            axisAngleRotation.Axis.Z);
                        double angle = axisAngleRotation.Angle;
                        rotation = new Quaternion(
                            axis.X * Math.Sin(angle / 2),
                            axis.Y * Math.Sin(angle / 2),
                            axis.Z * Math.Sin(angle / 2),
                            Math.Cos(angle / 2));
                    }

                }
                TransformDatas.Add(new TransformData(translation, scale, rotation));
            }

        }
        public void ConvertTransformDataToTransform3DGroup()
        {
            Transform3DGroup transformGroup = new Transform3DGroup();
           
            foreach (TransformData transformData in TransformDatas)
            {
                // Add the translation transformation
                TranslateTransform3D translation = new TranslateTransform3D(
                    transformData.Translation.X,
                    transformData.Translation.Y,
                    transformData.Translation.Z);
                transformGroup.Children.Add(translation);

                // Add the scale transformation
                ScaleTransform3D scale = new ScaleTransform3D(
                    transformData.Scale.X,
                    transformData.Scale.Y,
                    transformData.Scale.Z);
                transformGroup.Children.Add(scale);

                // Add the rotation transformation
                Quaternion rotation = transformData.Rotation;
                Vector3D axis = new Vector3D(rotation.X, rotation.Y, rotation.Z);
                double angle = 2 * Math.Acos(rotation.W);
                AxisAngleRotation3D axisAngleRotation = new AxisAngleRotation3D(axis, angle);
                RotateTransform3D rotateTransform = new RotateTransform3D(axisAngleRotation);
                transformGroup.Children.Add(rotateTransform);
            }
            transform = transformGroup;
        }
        public Material SetMaterial()
        {

            if (Color == SharpDX.Color.Transparent)
            {
                return null;
            }

            var material = PhongMaterials.Red;
            material.DiffuseColor = Color;
          
            return material;
        }

        public void Translate(Vector3 translation)
        {
            transform.Children.Add(new TranslateTransform3D(translation.X, translation.Y, translation.Z));
        }


        public void Select()
        {
          IsSelected = !IsSelected;
        }

        public void Deselect()
        {
           
        }

        public virtual List<VerletElement3D> GetElements()
        {

            var elements = new List<VerletElement3D>();
            return elements;
        }
    }
}
