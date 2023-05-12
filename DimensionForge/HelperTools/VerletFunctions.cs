using DimensionForge._3D.interfaces;
using DimensionForge._3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge.HelperTools
{
    public static class VerletFunctions
    {


        public static void Find_collision(IShape3D shape, List<Shape3D> shapes)
        {

            //  DO NOT REMOVE

            Shape3D shapeToCheck = shape as Shape3D;
            var mainShapeIndex = shapes.IndexOf(shapeToCheck);
            var mainPosition = shapeToCheck.Position;
            var mainOldPosition = shapeToCheck.OldPosition;

            bool collide = false;

            for (int i = 0; i < shapes.Count(); i++)
            {
                //if the itterations is on the shape to check it skips
                if (shapes[i] == shapes[mainShapeIndex] || shapes[i] is not Shape3D)
                    continue;
                //if the itterations is on a line it skips
                var otherShape = shapes[i];



                ////convert positions to Vector2 for easier calculations
                //var otherPosition = otherShape.Position;
                //var otherOldPosition = otherShape;

                //float radiusSum = shapeToCheck.Radius + otherShape.Radius;
                //var colission_axis = mainPosition.Position - otherPosition; //n = collision_axis / 2
                //float dist = Vector3.Distance(mainPosition.Position, otherPosition);

                ////if the distance between 2 balls is less then the sum of both radius
                //if (dist < radiusSum)
                //{

                //    Vector3 n = colission_axis / dist;
                //    //delta is the value that the balls need to move to keep free of eachother 
                //    float delta = radiusSum - dist;

                //    mainPosition.Position += 0.5f * delta * n;
                //    otherPosition -= 0.5f * delta * n;

                //    shapeToCheck.Position = mainPosition;
                //    otherShape.Position.Position = otherPosition;
                //    // shapeToCheck.OldPosition = mainPosition -5;
                //    // otherShape.OldPosition = mainPosition -5;
            }
        }
    }
}
