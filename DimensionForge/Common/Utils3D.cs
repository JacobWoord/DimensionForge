
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.Wpf.SharpDX;

using SharpDX;
using System;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Windows.Documents;
using System.Collections.Generic;

namespace Net_Designer_MVVM
{
    public static class Utils3D
    {

        private static readonly float EPS = 0.1f;

        public static List<SharpDX.Color> GetColorList()
        {
            List<SharpDX.Color> colors = new();
            colors.Add(SharpDX.Color.Red);//0
            colors.Add(SharpDX.Color.Blue);//1
            colors.Add(SharpDX.Color.Green);//2
            colors.Add(SharpDX.Color.Black);//3
            colors.Add(SharpDX.Color.Purple);//4
            colors.Add(SharpDX.Color.White);//5
            colors.Add(SharpDX.Color.DarkSeaGreen);//6
            colors.Add(SharpDX.Color.Brown);//7

            return colors;
        }


        public static float AngleBetweenAxes(Vector3 axis1Vec1, Vector3 axis1Vec2, Vector3 axis2Vec1, Vector3 axis2Vec2)
        {
            // Calculate the vectors for each axis
            Vector3 axis1 = Vector3.Normalize(axis1Vec2 - axis1Vec1);
            Vector3 axis2 = Vector3.Normalize(axis2Vec2 - axis2Vec1);

            // Calculate the dot product between the two axes
            float dotProduct = Vector3.Dot(axis1, axis2);

            // Calculate the angle between the two axes
            float angle = MathF.Acos(dotProduct);


            return angle;
        }

        public static Vector3 TranslatedReferencePoint(Plane plane, int transInCm, Vector3 point)
        {
            //init
            var point3D = point;
            var cmToTranslate = transInCm / 100;

            //get the normal of the plane
            var normal = plane.Normal;

            //translate
            var trans = normal * cmToTranslate;

            //return the new point
            return point3D + trans;
        }
        /// <summary>
        /// Rotate a Vector3 over an axis defined by two points.
        /// </summary>
        public static Vector3 RotateVectorOverAxis(Vector3 axis1, Vector3 axis2, Vector3 vectorToRotate, float angle)
        {


            // Define the point to be rotated
            Vector3 point = new Vector3(vectorToRotate.X, vectorToRotate.Y, vectorToRotate.Z);

            // Define two points that define the axis of rotation
            Vector3 point1 = axis1;
            Vector3 point2 = axis2;

            // Calculate the translation vector that will move the line to pass through the origin
            Vector3 translation = -point1;

            // Translate the point and the line
            point += translation;
            point1 += translation;
            point2 += translation;

            // Calculate the axis of rotation and angle of rotation as before
            Vector3 axis = Vector3.Normalize(point2 - point1);


            // Create the rotation quaternion
            SharpDX.Quaternion rotation = SharpDX.Quaternion.RotationAxis(axis, angle);

            // Apply the rotation to the point
            Vector3 rotatedPoint = Vector3.Transform(point, rotation);

            // Translate the rotated point back to its original position
            rotatedPoint -= translation;

            return rotatedPoint;

        }
        /// <summary>
        /// Calculates the distance between a point and a plane.
        /// </summary>     
        public static float DistancePointToPlane(Vector3 point, SharpDX.Plane plane)
        {
            /*ORGINEEL */
            float distance = Vector3.Dot(plane.Normal, point);
            distance -= plane.D;
            // Return the absolute value of the distance, since the sign indicates which side of the plane the point is on.
            return Math.Abs(distance) / plane.Normal.Length();
        }
        public static SceneNodeGroupModel3D OpenFile(string path)
        {
            if (File.Exists(path) == false)
            {
                MessageBox.Show($"bestand ({path}) bestaat niet.");
                return null;
            }
            SceneNodeGroupModel3D groupModel = new SceneNodeGroupModel3D();

            HelixToolkitScene scene;

            var loader = new Importer();
            scene = loader.Load(path);





            if (scene != null)
            {
                if (scene.Root != null)
                {
                    Debug.WriteLine("scene.Root != null");

                    foreach (var node in scene.Root.Traverse())
                    {
                        if (node is MaterialGeometryNode m)
                        {
                            //m.Geometry.SetAsTransient();
                            if (m.Material is PBRMaterialCore pbr)
                            {
                                pbr.RenderEnvironmentMap = true;
                            }
                            else if (m.Material is PhongMaterialCore phong)
                            {
                                phong.RenderEnvironmentMap = true;
                            }
                        }
                    }
                }
                groupModel.AddNode(scene.Root);


            }




            return groupModel;
        }
        /// <summary>
        /// Checks if v1-v2 is smaller then eps.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        /// 
        public static bool VectorEquals(SharpDX.Vector3 v1, SharpDX.Vector3 v2)
        {
            if (Math.Abs(v1.X - v2.X) < EPS && Math.Abs(v1.Y - v2.Y) < EPS && Math.Abs(v1.Z - v2.Z) < EPS)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Gets colour from color ramp (Blue Green Red).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="vmin"></param>
        /// <param name="vmax"></param>
        /// <returns></returns>
        public static SharpDX.Color4 GetColour(float v, float vmin, float vmax)
        {
            //https://stackoverflow.com/questions/7706339/grayscale-to-red-green-blue-matlab-jet-color-scale
            var c = new SharpDX.Color4(1.0f, 1.0f, 1.0f, 1.0f);// white
            float dv;

            if (v < vmin)
                v = vmin;
            if (v > vmax)
                v = vmax;
            dv = vmax - vmin;

            if (v < (vmin + 0.25f * dv))
            {
                c.Red = 0;
                c.Green = 4f * (v - vmin) / dv;
            }
            else if (v < (vmin + 0.5f * dv))
            {
                c.Red = 0;
                c.Green = 1f + 4f * (vmin + 0.25f * dv - v) / dv;
            }
            else if (v < (vmin + 0.75 * dv))
            {
                c.Red = 4f * (v - vmin - 0.5f * dv) / dv;
                c.Blue = 0;
            }
            else
            {
                c.Green = 1f + 4f * (vmin + 0.75f * dv - v) / dv;
                c.Blue = 0;
            }
            if (float.IsNaN(c.Green))
                c.Green = 1.0f;
            if (float.IsNaN(c.Red))
                c.Red = 1.0f;
            if (float.IsNaN(c.Blue))
                c.Blue = 1.0f;
            c.Alpha = 1.0f;
            return c;
        }

    }
}

