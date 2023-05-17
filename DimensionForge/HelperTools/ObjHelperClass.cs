
using DimensionForge._3D.Models;
using DimensionForge.Common;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge.HelperTools
{
    public static class ObjHelperClass
    {


        //Use this method to Import a OBJ File
        public static async Task<MeshGeometry3D> ImportAsMeshGeometry3D(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("No file specified");
            }

            var reader = new Importer();
            var scene = reader.Load(fileName);

            SceneNode rootNode = scene.Root;
            var meshNode = rootNode.Traverse()
            .OfType<MeshNode>()
            .FirstOrDefault();


            if (meshNode == null)
            {
                throw new InvalidOperationException("No mesh found in the file");
            }
            var meshGeometry = meshNode.Geometry as MeshGeometry3D;

            // The code below is optional it replaces the model with the center of the mass
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (var vertex in meshGeometry.Positions)
            {
                min = Vector3.Min(min, vertex);
                max = Vector3.Max(max, vertex);
            }
            var centerOfMass = (min + max) / 2;
            for (int i = 0; i < meshGeometry.Positions.Count; i++)
            {
                meshGeometry.Positions[i] -= centerOfMass;
            }


            // apply the desired scaling to the model
            for (int i = 0; i < meshGeometry.Positions.Count; i++)
            {
                meshGeometry.Positions[i] *= 0.001f;
            }

            meshGeometry.UpdateVertices();
            meshGeometry.UpdateBounds();
            meshGeometry.UpdateOctree();
            meshGeometry.UpdateTextureCoordinates();
            meshGeometry.UpdateTriangles();

            return meshGeometry;
        }

        public static Vector3 CalculateModelCenter(ObjModel3D model)
        {
            Vector3 center = new Vector3(0, 0, 0);

            for (int i = 0; i < model.BoundingPositions.Count; i++)
            {
                center += model.BoundingPositions[i].Position;
            }

            center /= model.BoundingPositions.Count;

            return center;
        }



        public static List<Node3D> GetBoundingPositions(ObjModel3D model)
        {
            var geometry = model.Geometry;

            Vector3 minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 maxBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var vertex in geometry.Positions)
            {
                minBounds = Vector3.Min(minBounds, vertex);
                maxBounds = Vector3.Max(maxBounds, vertex);
            }


            List<Node3D> corners = new List<Node3D>();
            corners.Add(new Node3D(new Vector3(maxBounds.X, minBounds.Y, minBounds.Z)) { CornerName = CornerName.BottomFrontLeft });
            corners.Add(new Node3D(new Vector3(maxBounds.X, maxBounds.Y, minBounds.Z)) { CornerName = CornerName.BottomFrontRight, Color = Color.Green });
            corners.Add(new Node3D(new Vector3(minBounds.X, maxBounds.Y, minBounds.Z)) { CornerName = CornerName.BottomBackLeft, Color = Color.Green });
            corners.Add(new Node3D(new Vector3(minBounds.X, minBounds.Y, minBounds.Z)) { CornerName = CornerName.BottomBackRight, Color = Color.Green });
            corners.Add(new Node3D(new Vector3(maxBounds.X, minBounds.Y, maxBounds.Z)) { CornerName = CornerName.TopFrontLeft, Color = Color.Blue });
            corners.Add(new Node3D(new Vector3(maxBounds.X, maxBounds.Y, maxBounds.Z)) { CornerName = CornerName.TopFrontRight, Color = Color.Blue });
            corners.Add(new Node3D(new Vector3(minBounds.X, maxBounds.Y, maxBounds.Z)) { CornerName = CornerName.TopBackLeft, Color = Color.Blue });
            corners.Add(new Node3D(new Vector3(minBounds.X, minBounds.Y, maxBounds.Z)) { CornerName = CornerName.TopBackRight, Color = Color.Blue });



            Vector3 frontCenter = GetCentroid(corners.Where(x => x.CornerName == CornerName.TopFrontRight
            || x.CornerName == CornerName.TopFrontLeft
            || x.CornerName == CornerName.BottomFrontRight
            || x.CornerName == CornerName.BottomFrontLeft).ToList());

            Vector3 backCenter = GetCentroid(corners.Where(x => x.CornerName == CornerName.TopBackRight
           || x.CornerName == CornerName.TopBackLeft
           || x.CornerName == CornerName.BottomBackRight
           || x.CornerName == CornerName.BottomBackLeft).ToList());


            Vector3 rightCenter = GetCentroid(corners.Where(x => x.CornerName == CornerName.TopBackRight
             || x.CornerName == CornerName.TopFrontLeft
             || x.CornerName == CornerName.BottomBackRight
             || x.CornerName == CornerName.BottomFrontLeft).ToList());


            Vector3 leftCenter = GetCentroid(corners.Where(x => x.CornerName == CornerName.TopBackLeft
              || x.CornerName == CornerName.TopFrontRight
              || x.CornerName == CornerName.BottomBackLeft
              || x.CornerName == CornerName.BottomFrontRight).ToList());



            Vector3 topCenter = GetCentroid(corners.Where(x => x.CornerName == CornerName.TopBackLeft
                || x.CornerName == CornerName.TopBackRight
                || x.CornerName == CornerName.TopFrontLeft
                || x.CornerName == CornerName.TopFrontRight).ToList());

            Vector3 bottomCenter = GetCentroid(corners.Where(x => x.CornerName == CornerName.BottomBackLeft
                || x.CornerName == CornerName.BottomBackRight
                || x.CornerName == CornerName.BottomFrontLeft
                || x.CornerName == CornerName.BottomFrontRight).ToList());

            Vector3 modelCenter = GetCentroid(corners.Where(x => x.CornerName == CornerName.TopBackRight
         || x.CornerName == CornerName.TopBackLeft
         || x.CornerName == CornerName.BottomBackRight
         || x.CornerName == CornerName.BottomBackLeft
         || x.CornerName == CornerName.TopFrontRight
         || x.CornerName == CornerName.TopFrontLeft
         || x.CornerName == CornerName.BottomFrontLeft
         || x.CornerName == CornerName.BottomFrontRight).ToList());




            // Add center points to the list
            corners.Add(new Node3D(frontCenter) { CornerName = CornerName.FrontPlaneCenter, UseCase = UseCase.modelCenter });
            corners.Add(new Node3D(backCenter) { CornerName = CornerName.BackPlaneCenter, UseCase = UseCase.modelCenter });
            corners.Add(new Node3D(leftCenter) { CornerName = CornerName.LeftPlaneCenter, UseCase = UseCase.modelCenter });
            corners.Add(new Node3D(rightCenter) { CornerName = CornerName.RightPlaneCenter, UseCase = UseCase.modelCenter });
            corners.Add(new Node3D(topCenter) { CornerName = CornerName.TopPlaneCenter, UseCase = UseCase.modelCenter });
            corners.Add(new Node3D(bottomCenter) { CornerName = CornerName.BottomPlaneCenter, UseCase = UseCase.modelCenter });
            corners.Add(new Node3D(modelCenter) { CornerName = CornerName.ModelCenter, UseCase = UseCase.modelCenter });

            return corners;
        }
        public static Vector3 GetCentroid(List<Node3D> positions)
        {
            Vector3 centroid = new Vector3(0, 0, 0);

            foreach (Node3D position in positions)
            {
                centroid += position.Position;
            }

            centroid /= positions.Count;
            return centroid;
        }
        public static Vector3 GetSize(ObjModel3D model)
        {
            var geometry = model.Geometry;

            Vector3 minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 maxBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var vertex in geometry.Positions)
            {
                minBounds = Vector3.Min(minBounds, vertex);
                maxBounds = Vector3.Max(maxBounds, vertex);
            }

            Vector3 size = maxBounds - minBounds;
            float width = size.X;
            float height = size.Y;
            float depth = size.Z;

            return size;

        }
        public static void UpdateScaling(ObjModel3D model, float scaleFactor)
        {

            // Apply the scale transform
            for (int i = 0; i < model.Geometry.Positions.Count; i++)
            {
                model.Geometry.Positions[i] *= scaleFactor;
            }

            //Apply the scale transform to the bounding positions of the model as well
            for (int i = 0; i < model.BoundingPositions.Count; i++)
            {
                model.BoundingPositions[i].Position *= scaleFactor;
            }

            model.Geometry.UpdateVertices();
        }



        public static void UpdatePosition(ObjModel3D model, Vector3 newPosition)
        {
            // Calculate the translation vector
            Vector3 translation = newPosition - model.GetCenter(CornerName.ModelCenter).Position;

            // Apply the translation transform
            for (int i = 0; i < model.Geometry.Positions.Count; i++)
            {
                model.Geometry.Positions[i] += translation;
            }

            // Apply the translation the bounding positions of the model as well
            for (int i = 0; i < model.BoundingPositions.Count; i++)
            {
                model.BoundingPositions[i].Position += translation;
            }

            //Update the models Postion
            model.Position = newPosition;
            // Notify the geometry that the positions have changed
            model.Geometry.UpdateVertices();
            //model.Geometry.UpdateBounds();
            //model.Geometry.UpdateOctree();
            //model.Geometry.UpdateTriangles();
        }


        public static void UpdateTemporaryNodes(ObjModel3D model, Vector3 newPosition, List<Node3D> temporaryNodes)
        {
            // Calculate the translation vector
            Vector3 translation = newPosition - model.GetCenter(CornerName.ModelCenter).Position;
                    
            // Apply the translation the bounding positions of the model as well
            for (int i = 0; i < temporaryNodes.Count; i++)
            {
                temporaryNodes[i].Position += translation;
            }

        }
           

           



        public static void ApplyRotation(ObjModel3D model, Quaternion rotationQuaternion, Vector3 pivotPoint)
        {
            Matrix rotationMatrix = Matrix.RotationQuaternion(rotationQuaternion);

            for (int i = 0; i < model.Geometry.Positions.Count; i++)
            {
                Vector3 positionRelativeToPivot = model.Geometry.Positions[i] - pivotPoint;
                model.Geometry.Positions[i] = Vector3.TransformCoordinate(positionRelativeToPivot, rotationMatrix) + pivotPoint;
            }

            for (int i = 0; i < model.BoundingPositions.Count; i++)
            {
                Vector3 positionRelativeToPivot = model.BoundingPositions[i].Position - pivotPoint;
                model.BoundingPositions[i].Position = Vector3.TransformCoordinate(positionRelativeToPivot, rotationMatrix) + pivotPoint;
            }

            model.Geometry.UpdateVertices();
            //      model.Geometry.UpdateBounds();
            model.Geometry.UpdateOctree();
            model.Geometry.UpdateTriangles();
        }


        public static void RotateGeometry(ObjModel3D model, Vector3 rotationAxis, float angleInDegrees)
        {
            // Convert the angle from degrees to radians
            float angleInRadians = angleInDegrees * ((float)Math.PI / 180f);

            // Create a rotation matrix
            Matrix rotationMatrix = Matrix.RotationAxis(rotationAxis, angleInRadians);


            var centerPoint = model.GetCenter(CornerName.ModelCenter);
            // Apply the rotation to each vertex position in the geometry
            for (int i = 0; i < model.Geometry.Positions.Count; i++)
            {
                model.Geometry.Positions[i] -= centerPoint.Position;

                // Apply the rotation
                model.Geometry.Positions[i] = Vector3.TransformCoordinate(model.Geometry.Positions[i], rotationMatrix);

                // Translate the geometry back
                model.Geometry.Positions[i] += centerPoint.Position;
            }

            // Apply the rotation to each bounding position in the model
            for (int i = 0; i < model.BoundingPositions.Count; i++)
            {
                model.BoundingPositions[i].Position -= centerPoint.Position;
                // Apply the rotation
                model.BoundingPositions[i].Position = Vector3.TransformCoordinate(model.BoundingPositions[i].Position, rotationMatrix);
                // Translate the geometry back
                model.BoundingPositions[i].Position += centerPoint.Position;
            }

            // Update the geometry
            model.Geometry.UpdateVertices();
            
            model.Geometry.UpdateBounds();
            model.Geometry.UpdateOctree();
            model.Geometry.UpdateTriangles();



        }






    }
}
