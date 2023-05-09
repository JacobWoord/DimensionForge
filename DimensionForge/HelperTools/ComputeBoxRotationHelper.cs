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



        //public static Quaternion ComputeCombinedRotationQuaternion(List<Node3D> modelNodes, List<Node3D> verletNodes)
        //{

        //    Quaternion horizontalRotationQuaternion = ComputeBoxRotationHelper.ComputeLeftRightRotationQuaternion(modelNodes, verletNodes);
        //    Quaternion verticalRotationQuaternion = ComputeBoxRotationHelper.ComputeTopBottomRotationQuaternion(modelNodes, verletNodes);
        //    Quaternion frontBackRotationQuaternion = ComputeBoxRotationHelper.ComputeFrontBackRotationQuaternion(modelNodes, verletNodes);

        //    Quaternion combinedRotationQuaternion = Quaternion.Multiply(horizontalRotationQuaternion, verticalRotationQuaternion);
        //    combinedRotationQuaternion = Quaternion.Multiply(combinedRotationQuaternion, frontBackRotationQuaternion);

        //    return combinedRotationQuaternion;
        //}

        //public static Quaternion ComputeCombinedRotationQuaternion(List<Node3D> modelNodes, List<Node3D> verletNodes)
        //{
        //    for (int i = 0; i < modelNodes.Count; i++)
        //    {
        //        Debug.WriteLine($"Model NodePosition{i}: {modelNodes[i].Position}");
        //    }
        //    for (int i = 0; i < verletNodes.Count; i++)
        //    {
        //        Debug.WriteLine($"verlet NodePosition{i}: {verletNodes[i].Position}");
        //    }

        //    // Calculate the horizontal rotation quaternion
        //    Vector3 horizontalVerletOrientation = ComputeBoxRotationHelper.GetLeftRightOrientation(verletNodes);
        //    Vector3 horizontalModelOrientation = ComputeBoxRotationHelper.GetLeftRightOrientation(modelNodes);
        //    horizontalVerletOrientation.Normalize();
        //    horizontalModelOrientation.Normalize();
        //    Vector3 horizontalRotationAxis = Vector3.Cross(horizontalModelOrientation, horizontalVerletOrientation);
        //    float horizontalRotationAngle = (float)Math.Acos(Vector3.Dot(horizontalModelOrientation, horizontalVerletOrientation));
        //    Quaternion horizontalRotationQuaternion = Quaternion.RotationAxis(horizontalRotationAxis, horizontalRotationAngle);
        //    Debug.WriteLine($"Horizontal Rotation Quaternion: {horizontalRotationQuaternion}");

        //    // Calculate the vertical rotation quaternion
        //    Vector3 verticalVerletOrientation = ComputeBoxRotationHelper.GetTopBottomOrientation(verletNodes);
        //    Vector3 verticalModelOrientation = ComputeBoxRotationHelper.GetTopBottomOrientation(modelNodes);
        //    verticalVerletOrientation.Normalize();
        //    verticalModelOrientation.Normalize();
        //    Vector3 verticalRotationAxis = Vector3.Cross(verticalModelOrientation, verticalVerletOrientation);
        //    float verticalRotationAngle = (float)Math.Acos(Vector3.Dot(verticalModelOrientation, verticalVerletOrientation));
        //    Quaternion verticalRotationQuaternion = Quaternion.RotationAxis(verticalRotationAxis, verticalRotationAngle);
        //    Debug.WriteLine($"Vertical Rotation Quaternion: {verticalRotationQuaternion}");

        //    // Calculate the front-back rotation quaternion
        //    Vector3 frontBackVerletOrientation = ComputeBoxRotationHelper.GetFrontBacklOrientation(verletNodes);
        //    Vector3 frontBackModelOrientation = ComputeBoxRotationHelper.GetFrontBacklOrientation(modelNodes);
        //    frontBackVerletOrientation.Normalize();
        //    frontBackModelOrientation.Normalize();
        //    Vector3 frontBackRotationAxis = Vector3.Cross(frontBackModelOrientation, frontBackVerletOrientation);
        //    float frontBackRotationAngle = (float)Math.Acos(Vector3.Dot(frontBackModelOrientation, frontBackVerletOrientation));
        //    Quaternion frontBackRotationQuaternion = Quaternion.RotationAxis(frontBackRotationAxis, frontBackRotationAngle);
        //    Debug.WriteLine($"Front-Back Rotation Quaternion: {frontBackRotationQuaternion}");

        //    // Combine the rotation quaternions
        //    Quaternion combinedRotationQuaternion = Quaternion.Multiply(horizontalRotationQuaternion, verticalRotationQuaternion);
        //    combinedRotationQuaternion = Quaternion.Multiply(combinedRotationQuaternion, frontBackRotationQuaternion);
        //    Debug.WriteLine($"Combined Rotation Quaternion: {combinedRotationQuaternion}");

        //    return combinedRotationQuaternion;
        //}
        public static Quaternion ComputeRotationQuaternion(List<Node3D> modelNodes, List<Node3D> verletNodes)
        {
            if (modelNodes.Count != verletNodes.Count)
            {
                throw new ArgumentException("Model and verlet nodes lists should have the same length.");
            }

            Vector3 modelCentroid = GetCentroid(modelNodes);
            Vector3 verletCentroid = GetCentroid(verletNodes);

            Debug.WriteLine($"Model Centroid: {modelCentroid}");
            Debug.WriteLine($"Verlet Centroid: {verletCentroid}");

            Vector3 modelOrientation = modelCentroid - modelNodes[0].Position;
            Vector3 verletOrientation = verletCentroid - verletNodes[0].Position;

            modelOrientation.Normalize();
            verletOrientation.Normalize();

            Debug.WriteLine($"Model Orientation: {modelOrientation}");
            Debug.WriteLine($"Verlet Orientation: {verletOrientation}");

            Vector3 crossProduct = Vector3.Cross(modelOrientation, verletOrientation);
            float dotProduct = Vector3.Dot(modelOrientation, verletOrientation);

            Debug.WriteLine($"Cross Product: {crossProduct}");
            Debug.WriteLine($"Dot Product: {dotProduct}");

            Vector4 rotationVector = new Vector4(crossProduct.X, crossProduct.Y, crossProduct.Z, dotProduct);
            Quaternion rotationQuaternion = Quaternion.Normalize(new Quaternion(rotationVector));

            Debug.WriteLine($"Rotation Quaternion: {rotationQuaternion}");

            return rotationQuaternion;
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
            var frontFaceCorners = nodes.Where(n => n.CornerName == CornerName.UpperLeftFrontCorner
              || n.CornerName == CornerName.UpperRightFrontCorner
              || n.CornerName == CornerName.UpperLeftBackCorner
              || n.CornerName == CornerName.UpperRightBackCorner).ToArray();

            var backFaceCorners = nodes.Where(n => n.CornerName == CornerName.LowerRightBackCorner
           || n.CornerName == CornerName.LowerLeftBackCorner
           || n.CornerName == CornerName.LowerRightFrontCorner
           || n.CornerName == CornerName.LowerLeftFrontCorner).ToArray();

            var frontFaceCornersPositions = frontFaceCorners.Select(n => n.Position).ToArray();
            var backFaceCornersPositions = backFaceCorners.Select(n => n.Position).ToArray();

            Vector3 frontCentroid = Utils3D.GetCentroidPosition(frontFaceCornersPositions);
            Vector3 backCentroid = Utils3D.GetCentroidPosition(backFaceCornersPositions);

            var orientation = frontCentroid - backCentroid;

            return orientation;


        }


        public static Vector3 GetFrontBacklOrientation(List<Node3D> nodes)
        {

            // computes the the orientation from te bottomMiddlePoint - topMiddelPoint
            var frontFaceCorners = nodes.Where(n => n.CornerName == CornerName.UpperLeftFrontCorner
              || n.CornerName == CornerName.UpperRightFrontCorner
              || n.CornerName == CornerName.LowerLeftFrontCorner
              || n.CornerName == CornerName.LowerRightFrontCorner).ToArray();

            var backFaceCorners = nodes.Where(n => n.CornerName == CornerName.UpperRightBackCorner
           || n.CornerName == CornerName.UpperLeftBackCorner
           || n.CornerName == CornerName.LowerLeftBackCorner
           || n.CornerName == CornerName.LowerRightBackCorner).ToArray();

            var frontFaceCornersPositions = frontFaceCorners.Select(n => n.Position).ToArray();
            var backFaceCornersPositions = backFaceCorners.Select(n => n.Position).ToArray();

            Vector3 frontCentroid = Utils3D.GetCentroidPosition(frontFaceCornersPositions);
            Vector3 backCentroid = Utils3D.GetCentroidPosition(backFaceCornersPositions);

            var orientation = frontCentroid - backCentroid;

            return orientation;


        }





        public static Vector3 GetLeftRightOrientation(List<Node3D> nodes)
        {

            // computes the the orientation from te bottomMiddlePoint - topMiddelPoint
            var topCorners = nodes.Where(n => n.CornerName == CornerName.UpperLeftFrontCorner
            || n.CornerName == CornerName.UpperLeftBackCorner
            || n.CornerName == CornerName.LowerLeftFrontCorner
            || n.CornerName == CornerName.LowerLeftBackCorner).ToArray();

            var bottomCorners = nodes.Where(n => n.CornerName == CornerName.UpperRightFrontCorner
           || n.CornerName == CornerName.UpperRightBackCorner
           || n.CornerName == CornerName.LowerRightFrontCorner
           || n.CornerName == CornerName.LowerRightBackCorner).ToArray();

            var topCornerPositions = topCorners.Select(n => n.Position).ToArray();
            var bottomCornerPositions = bottomCorners.Select(n => n.Position).ToArray();


            var topCentroid = Utils3D.GetCentroidPosition(topCornerPositions);
            var bottomCentroid = Utils3D.GetCentroidPosition(bottomCornerPositions);

            var orientation = bottomCentroid - topCentroid;

            return orientation;
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
            Vector3 verletOrientation = GetFrontBacklOrientation(verletNodes);
            Vector3 modelOrientation = GetFrontBacklOrientation(modelNodes);

            verletOrientation.Normalize();
            modelOrientation.Normalize();

            Vector3 rotationAxis = Vector3.Cross(modelOrientation, verletOrientation);
            float rotationAngle = (float)Math.Acos(Vector3.Dot(modelOrientation, verletOrientation));

            Quaternion rotationQuaternion = Quaternion.RotationAxis(rotationAxis, rotationAngle);

            return rotationQuaternion;
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



        public static Transform3DGroup CreateTransformGroup( SharpDX.Quaternion rotation, List<Node3D> modelNodes, Transform3DGroup existingTransform)
        {
            
            // Convert SharpDX types to WPF types
            //System.Windows.Media.Media3D.Vector3D wpfTranslation = new System.Windows.Media.Media3D.Vector3D(translation.X, translation.Y, translation.Z);
            System.Windows.Media.Media3D.Quaternion wpfQuaternion = new System.Windows.Media.Media3D.Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);

            
        

            // Create the rotation transform
            RotateTransform3D rotationTransform = new RotateTransform3D(new QuaternionRotation3D(wpfQuaternion));

            // Combine the transformations with the existing Transform3DGroup
     
            existingTransform.Children.Add(rotationTransform);
          

            // Create a Matrix3D representing the combined translation and rotation
            Matrix3D transformationMatrix = new Matrix3D();
            transformationMatrix.Rotate(wpfQuaternion);
           // transformationMatrix.Translate(wpfTranslation);

            // Apply the translation and rotation to each Node3D.Position in modelNodes
            foreach (Node3D node in modelNodes)
            {
                // Convert Node3D.Position to a WPF Point3D
                System.Windows.Media.Media3D.Point3D point = new System.Windows.Media.Media3D.Point3D(node.Position.X, node.Position.Y, node.Position.Z);

                // Apply the transformation matrix to the point
                point = transformationMatrix.Transform(point);

                // Convert the transformed Point3D back to a Vector3 and update the node.Position
               node.Position = new Vector3((float)point.X, (float)point.Y, (float)point.Z);
            }

            return existingTransform;
        }

        public static Transform3DGroup CreateTransformGroup1(SharpDX.Quaternion rotation, List<Node3D> modelNodes, Transform3DGroup existingTransform)
        {
            // Convert SharpDX types to WPF types
            System.Windows.Media.Media3D.Quaternion wpfQuaternion = new System.Windows.Media.Media3D.Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);

            // Create the rotation transform
            RotateTransform3D rotationTransform = new RotateTransform3D(new QuaternionRotation3D(wpfQuaternion));

            // Combine the rotations with the existing Transform3DGroup
            int rotateIndex = -1;

            // Find the index of the existing RotateTransform3D within the existing Transform3DGroup
            for (int i = 0; i < existingTransform.Children.Count; i++)
            {
                if (existingTransform.Children[i] is RotateTransform3D)
                {
                    rotateIndex = i;
                    break;
                }
            }

            // Update the existing rotation by multiplying the new rotation quaternion with the previous one
            if (rotateIndex != -1)
            {
                QuaternionRotation3D previousRotation = ((RotateTransform3D)existingTransform.Children[rotateIndex]).Rotation as QuaternionRotation3D;
                System.Windows.Media.Media3D.Quaternion combinedRotation = previousRotation.Quaternion * wpfQuaternion;
                ((RotateTransform3D)existingTransform.Children[rotateIndex]).Rotation = new QuaternionRotation3D(combinedRotation);
            }
            else
            {
                // If there was no previous rotation, add the new rotation
                existingTransform.Children.Add(rotationTransform);
            }

            // Create a Matrix3D representing the combined translation and rotation
            Matrix3D transformationMatrix = existingTransform.Value;

            // Apply the transformation matrix to each Node3D.Position in modelNodes
            foreach (Node3D node in modelNodes)
            {
                // Convert Node3D.Position to a WPF Point3D
                System.Windows.Media.Media3D.Point3D point = new System.Windows.Media.Media3D.Point3D(node.Position.X, node.Position.Y, node.Position.Z);

                // Apply the transformation matrix to the point
                point = transformationMatrix.Transform(point);

                // Convert the transformed Point3D back to a Vector3 and update the node.Position
                node.Position = new Vector3((float)point.X, (float)point.Y, (float)point.Z);
            }

            return existingTransform;
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


        //public static Transform3DGroup CreateTransformGroup(Vector3 translation, float scale, SharpDX.Quaternion rotation)
        //{
        //    // Convert SharpDX types to WPF types
        //    System.Windows.Media.Media3D.Vector3D wpfTranslation = new System.Windows.Media.Media3D.Vector3D(translation.X, translation.Y, translation.Z);
        //    System.Windows.Media.Media3D.Quaternion wpfQuaternion = new System.Windows.Media.Media3D.Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);

        //    // Create the translation transform
        //    TranslateTransform3D translationTransform = new TranslateTransform3D(wpfTranslation);

        //    // Create the scaling transform
        //    ScaleTransform3D scalingTransform = new ScaleTransform3D(scale, scale, scale);


        //    // Create the rotation transform
        //    RotateTransform3D rotationTransform = new RotateTransform3D(new QuaternionRotation3D(wpfQuaternion));

        //    // Combine the transformations
        //    Transform3DGroup transformGroup = new Transform3DGroup();
        //    transformGroup.Children.Add(translationTransform);
        //    transformGroup.Children.Add(rotationTransform);
        //    transformGroup.Children.Add(scalingTransform);

        //    return transformGroup;
        //}

    }
}
