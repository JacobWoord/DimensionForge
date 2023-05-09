using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using DimensionForge.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge.HelperTools
{
    public static class PositioningHelper
    {


        public static List<CornerPoint3D> NewBoundingBox(BatchedModel3D model)
        {

          
            model.BatchedMeshes[0].Geometry.UpdateBounds();


            var bounding = model.GetBoundingBox();
            var corners = bounding.GetCorners();
            var center = bounding.Center;

            List<CornerPoint3D> cornersList = new();
           
            corners.ToList().ForEach(x => cornersList.Add(new CornerPoint3D() { LinkedNode = new Node3D(x), Color = Color.Yellow }));


            return cornersList;
        }


        public static Vector3 GetCenterOfMass(List<Node3D> nodes)
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


        public  static List<CornerPoint3D> DrawCenters(BatchedModel3D model, VerletBuildResult builResult)
        {

            Vector3 verletCenter = PositioningHelper.GetCenterOfMass(builResult.Nodes);
            Vector3 modelCenter = PositioningHelper.GetCenterOfMass(model.Bbcorners);

            List<CornerPoint3D> cornerPoints = new List<CornerPoint3D>();

            cornerPoints.Add(new CornerPoint3D() { LinkedNode = new Node3D(verletCenter), Color = Color.Yellow });
            cornerPoints.Add(new CornerPoint3D() { LinkedNode = new Node3D(modelCenter), Color = Color.Yellow });


            return cornerPoints;

           
        }

        public static async Task MoveToCenterOfVerletShape(BatchedModel3D model , VerletBuildResult buidresult)
        {

       
            //First im trying to calculate the translation vector bases on the center positions of the models  (MODEL WORD NIET VERPLAATST MET HET CENTERPUNT)
            Vector3 verletCenter = GetCenterOfMass(buidresult.Nodes);
            Vector3 modelCenter = GetCenterOfMass(model.Bbcorners);
            Vector3 translation = verletCenter - modelCenter;

            await model.MoveCenterToPosition(translation);

        }


        public static async Task MoveModelToOrigin(BatchedModel3D model)
        {
            await model.MoveCenterToPosition(Vector3.Zero);
        }


        public static void Find_collision(IShape3D shape)
        {

            //DO NOT REMOVE

            //Sphere3D shapeToCheck = shape as Sphere3D;
            //var mainShapeIndex = shapes.IndexOf(shapeToCheck);
            //var mainPosition = shapeToCheck.Position;
            //var mainOldPosition = shapeToCheck.OldPosition;

            //bool collide = false;

            //for (int i = 0; i < shapes.Count(); i++)
            //{
            //    //if the itterations is on the shape to check it skips
            //    if (shapes[i] == shapes[mainShapeIndex] || shapes[i] is not Sphere3D)
            //        continue;
            //    //if the itterations is on a line it skips
            //    var otherShape = shapes[i] as Sphere3D;



            //    //convert positions to Vector2 for easier calculations
            //    var otherPosition = otherShape.Position.Position;
            //    var otherOldPosition = otherShape.OldPosition;

            //    float radiusSum = shapeToCheck.Radius + otherShape.Radius;
            //    var colission_axis = mainPosition.Position - otherPosition; //n = collision_axis / 2
            //    float dist = Vector3.Distance(mainPosition.Position, otherPosition);

            //    //if the distance between 2 balls is less then the sum of both radius
            //    if (dist < radiusSum)
            //    {

            //        Vector3 n = colission_axis / dist;
            //        //delta is the value that the balls need to move to keep free of eachother 
            //        float delta = radiusSum - dist;

            //        mainPosition.Position += 0.5f * delta * n;
            //        otherPosition -= 0.5f * delta * n;

            //        shapeToCheck.Position = mainPosition;
            //        otherShape.Position.Position = otherPosition;
            //        // shapeToCheck.OldPosition = mainPosition -5;
            //        // otherShape.OldPosition = mainPosition -5;
            //    }}
            
        }



    }
}
