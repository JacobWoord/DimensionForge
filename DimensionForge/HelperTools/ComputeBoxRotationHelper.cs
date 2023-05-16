using DimensionForge._3D.Models;
using DimensionForge.Common;
using Net_Designer_MVVM;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Quaternion = SharpDX.Quaternion;

namespace DimensionForge.HelperTools
{
    public static class ComputeBoxRotationHelper
    {


        public static Quaternion ComputeCombinedRotationQuaternionEMA(List<Node3D> modelNodes, List<Node3D> verletNodes, ref Quaternion previousRotation, float alpha)
        {
            Quaternion horizontalRotationQuaternion = ComputeBoxRotationHelper.ComputeLeftRightRotationQuaternion(modelNodes, verletNodes);
            Quaternion verticalRotationQuaternion = ComputeBoxRotationHelper.ComputeTopBottomRotationQuaternion(modelNodes, verletNodes);
            Quaternion frontBackRotationQuaternion = ComputeBoxRotationHelper.ComputeFrontBackRotationQuaternion(modelNodes, verletNodes);

            var test = horizontalRotationQuaternion + verticalRotationQuaternion;

            Quaternion combinedRotationQuaternion = Quaternion.Multiply(horizontalRotationQuaternion, verticalRotationQuaternion);
            combinedRotationQuaternion = Quaternion.Multiply(combinedRotationQuaternion, frontBackRotationQuaternion);

            if (!IsValidQuaternion(combinedRotationQuaternion))
            {
                Debug.WriteLine("Invalid rotation quaternion detected");
                if (IsValidQuaternion(previousRotation))
                {
                    combinedRotationQuaternion = previousRotation;
                }
                else
                {
                    Quaternion defaultQ = new Quaternion(0, 0, 0, 1);
                    return defaultQ; // or provide a default quaternion
                }
            }

            Quaternion smoothedRotation = ExponentialMovingAverage(previousRotation, combinedRotationQuaternion, alpha);

            previousRotation = smoothedRotation;

            smoothedRotation.Normalize();

            return smoothedRotation;
        }

        //public static Quaternion ComputeCombinedRotationQuaternionEMA(List<Node3D> modelNodes, List<Node3D> verletNodes, ref Quaternion previousRotation, float alpha)
        //{
        //    Quaternion horizontalRotationQuaternion = ComputeBoxRotationHelper.ComputeLeftRightRotationQuaternion(modelNodes, verletNodes);
        //    Quaternion verticalRotationQuaternion = ComputeBoxRotationHelper.ComputeTopBottomRotationQuaternion(modelNodes, verletNodes);
        //    Quaternion frontBackRotationQuaternion = ComputeBoxRotationHelper.ComputeFrontBackRotationQuaternion(modelNodes, verletNodes);

        //    Quaternion combinedRotationQuaternion = Quaternion.Multiply(horizontalRotationQuaternion, verticalRotationQuaternion);
        //    combinedRotationQuaternion = Quaternion.Multiply(combinedRotationQuaternion, frontBackRotationQuaternion);

        //    Quaternion smoothedRotation = ExponentialMovingAverage(previousRotation, combinedRotationQuaternion, alpha);


        //    previousRotation = smoothedRotation;

        //    smoothedRotation.Normalize();


        //    return smoothedRotation;
        //}

        private static bool IsValidQuaternion(Quaternion q)
        {
            float epsilon = 1e-6f;
            float length = q.Length();
            return (Math.Abs(length - 1) < epsilon);
        }



        public static Quaternion ExponentialMovingAverage(Quaternion previousRotation, Quaternion currentRotation, float alpha)
        {
            Quaternion emaRotation = new Quaternion();

            emaRotation.X = previousRotation.X * (1 - alpha) + currentRotation.X * alpha;
            emaRotation.Y = previousRotation.Y * (1 - alpha) + currentRotation.Y * alpha;
            emaRotation.Z = previousRotation.Z * (1 - alpha) + currentRotation.Z * alpha;
            emaRotation.W = previousRotation.W * (1 - alpha) + currentRotation.W * alpha;

            emaRotation.Normalize();

            return emaRotation;
        }





        public static Quaternion AverageQuaternion(Queue<Quaternion> quaternions)
        {
            float x = 0, y = 0, z = 0, w = 0;

            foreach (Quaternion q in quaternions)
            {
                x += q.X;
                y += q.Y;
                z += q.Z;
                w += q.W;
            }

            int count = quaternions.Count;
            x /= count;
            y /= count;
            z /= count;
            w /= count;

            Quaternion average = new Quaternion(x, y, z, w);
            average.Normalize(); // Normalize the resulting quaternion

            return average;
        }



        public static Vector3 RoundVector3(Vector3 vector, int decimalPlaces)
        {
            return new Vector3(
                (float)Math.Round(vector.X, decimalPlaces),
                (float)Math.Round(vector.Y, decimalPlaces),
                (float)Math.Round(vector.Z, decimalPlaces)
            );
        }

    

      
        public static Vector3 GetCentroid(List<Node3D> nodes)
        {
            Vector3 centroid = new Vector3(0, 0, 0);
            foreach (Node3D node in nodes)
            {
                centroid += node.Position;
            }
            centroid /= nodes.Count;
            return centroid;
        }
        public static Vector3 GetTopBottomOrientation(List<Node3D> nodes)
        {
            // computes the the orientation from te bottomMiddlePoint - topMiddelPoint
            var frontFaceCorners = nodes.Where(n => n.CornerName == CornerName.TopFrontLeft
              || n.CornerName == CornerName.TopFrontRight
              || n.CornerName == CornerName.TopBackLeft
              || n.CornerName == CornerName.TopBackRight).ToList();

            var backFaceCorners = nodes.Where(n => n.CornerName == CornerName.BottomBackRight
           || n.CornerName == CornerName.BottomBackLeft
           || n.CornerName == CornerName.BottomFrontRight
           || n.CornerName == CornerName.BottomFrontLeft).ToList();

      
            Vector3 frontCentroid = GetCenter(frontFaceCorners);
            Vector3 backCentroid = GetCenter(backFaceCorners);
            var orientation = frontCentroid - backCentroid;


           return orientation;
        }
        public static Vector3 GetFrontBackOrientation(List<Node3D> nodes)
        {
            // computes the the orientation from te bottomMiddlePoint - topMiddelPoint
            var frontFaceCorners = nodes.Where(n => n.CornerName == CornerName.TopFrontLeft
              || n.CornerName == CornerName.TopFrontRight
              || n.CornerName == CornerName.BottomFrontLeft
              || n.CornerName == CornerName.BottomFrontRight).ToList();

            var backFaceCorners = nodes.Where(n => n.CornerName == CornerName.BottomBackRight
           || n.CornerName == CornerName.BottomBackLeft
           || n.CornerName == CornerName.TopBackRight
           || n.CornerName == CornerName.TopBackLeft).ToList();

            Vector3 frontCentroid = GetCenter(frontFaceCorners);
            Vector3 backCentroid = GetCenter(backFaceCorners);
            var orientation = frontCentroid - backCentroid;
            return orientation;
        }
        public static Vector3 GetLeftRightOrientation(List<Node3D> nodes)
        {

            // computes the the orientation from te bottomMiddlePoint - topMiddelPoint
            var topCorners = nodes.Where(n => n.CornerName == CornerName.TopFrontLeft
            || n.CornerName == CornerName.TopBackLeft
            || n.CornerName == CornerName.BottomFrontLeft
            || n.CornerName == CornerName.BottomBackLeft).ToList();

            var bottomCorners = nodes.Where(n => n.CornerName == CornerName.TopFrontRight
           || n.CornerName == CornerName.TopBackRight
           || n.CornerName == CornerName.BottomFrontRight
           || n.CornerName == CornerName.BottomBackRight).ToList();

          
            var topCentroid = GetCenter(topCorners);
            var bottomCentroid = GetCenter(bottomCorners);

            var orientation = bottomCentroid - topCentroid;

            return orientation;      
        }





        public static Quaternion ComputeTopBottomRotationQuaternion(List<Node3D> modelNodes, List<Node3D> verletNodes)
        {

            Vector3 verletOrientation = GetTopBottomOrientation(verletNodes);
            Vector3 modelOrientation = GetTopBottomOrientation(modelNodes);

            // Normalize the orientation vectors
            verletOrientation.Normalize();
            modelOrientation.Normalize();

            // Compute the rotation axis and angle
            Vector3 rotationAxis = Vector3.Cross(modelOrientation, verletOrientation);
            float rotationAngle = (float)Math.Acos(Vector3.Dot(modelOrientation, verletOrientation));

            // Create a quaternion
            Quaternion rotationQuaternion = Quaternion.RotationAxis(rotationAxis, rotationAngle);



            return rotationQuaternion;
        }


        public static Quaternion ComputeLeftRightRotationQuaternion(List<Node3D> modelNodes, List<Node3D> verletNodes)
        {

            Vector3 verletOrientation = GetLeftRightOrientation(verletNodes);
            Vector3 modelOrientation = GetLeftRightOrientation(modelNodes);

            // Normalize the orientation vectors
            verletOrientation.Normalize();
            modelOrientation.Normalize();

            // Compute the rotation axis and angle
            Vector3 rotationAxis = Vector3.Cross(modelOrientation, verletOrientation);
            float rotationAngle = (float)Math.Acos(Vector3.Dot(modelOrientation, verletOrientation));

            // Create a quaternion
            Quaternion rotationQuaternion = Quaternion.RotationAxis(rotationAxis, rotationAngle);

            return rotationQuaternion;
        }


        public static Quaternion ComputeFrontBackRotationQuaternion(List<Node3D> modelNodes, List<Node3D> verletNodes)
        {
            Vector3 verletOrientation = GetFrontBackOrientation(verletNodes);
            Vector3 modelOrientation = GetFrontBackOrientation(modelNodes);

            verletOrientation.Normalize();
            modelOrientation.Normalize();

            Vector3 rotationAxis = Vector3.Cross(modelOrientation, verletOrientation);
            float rotationAngle = (float)Math.Acos(Vector3.Dot(modelOrientation, verletOrientation));

            Quaternion rotationQuaternion = Quaternion.RotationAxis(rotationAxis, rotationAngle);

            return rotationQuaternion;
        }

      



        public static Vector3 GetCenter(List<Node3D> nodes)
        {
            Vector3 centerOfMass = Vector3.Zero;
            int nodeCount = nodes.Count;

            if (nodeCount == 0)
                return centerOfMass;

            foreach (Node3D node in nodes)
            {
                centerOfMass += node.Position;
            }

            centerOfMass /= nodeCount;
            return centerOfMass;
        }


        public static Transform3DGroup CreateTransformGroup(Vector3 translation, float scale, SharpDX.Quaternion rotation)
        {
            // Convert SharpDX types to WPF types
            System.Windows.Media.Media3D.Vector3D wpfTranslation = new System.Windows.Media.Media3D.Vector3D(translation.X, translation.Y, translation.Z);
            System.Windows.Media.Media3D.Quaternion wpfQuaternion = new System.Windows.Media.Media3D.Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);

            // Create the translation transform
            TranslateTransform3D translationTransform = new TranslateTransform3D(wpfTranslation);

            // Create the scaling transform
            ScaleTransform3D scalingTransform = new ScaleTransform3D(scale, scale, scale);


            // Create the rotation transform
            RotateTransform3D rotationTransform = new RotateTransform3D(new QuaternionRotation3D(wpfQuaternion));

            // Combine the transformations
            Transform3DGroup transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(translationTransform);
            transformGroup.Children.Add(rotationTransform);
            transformGroup.Children.Add(scalingTransform);

            return transformGroup;
        }

    }
}
