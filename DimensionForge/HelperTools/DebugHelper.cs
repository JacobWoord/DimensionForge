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
    public static class DebugHelper
    {

        public static Vector3 GetBoxDimensions(List<Node3D> verletNodes)
        {
            var bottomBackLeft = verletNodes.FirstOrDefault(x => x.CornerName == CornerName.BottomBackLeft);
            var topBackLeft = verletNodes.FirstOrDefault(x => x.CornerName == CornerName.TopBackLeft);
            var bottomFrontLeft = verletNodes.FirstOrDefault(x => x.CornerName == CornerName.BottomFrontLeft);
            var bottomBackRight = verletNodes.FirstOrDefault(x => x.CornerName == CornerName.BottomBackRight);

            if (bottomBackLeft == null || topBackLeft == null || bottomFrontLeft == null || bottomBackRight == null)
            {
                throw new ArgumentException("Missing corner nodes in the provided verletNodes list.");
            }

            float length = Vector3.Distance(bottomBackLeft.Position, bottomBackRight.Position);
            float width = Vector3.Distance(bottomBackLeft.Position, bottomFrontLeft.Position);
            float height = Vector3.Distance(bottomBackLeft.Position, topBackLeft.Position);

            return new Vector3(length, width, height);
        }


        public static void DebugDimensionDifference(Vector3 modeldimensions, Vector3 boxdimensions)
        {
            Vector3 boxDimensions = boxdimensions;
            Vector3 modelDimensions = modeldimensions;

            Vector3 difference = (boxDimensions - modelDimensions) * 1000;
            if (difference.X > 1) {
                Debug.WriteLine($"Box Dimensions: {boxDimensions}");
                Debug.WriteLine($"Model Dimensions: {modelDimensions}");
                Debug.WriteLine($"Dimension Difference * 1000: {difference}");
            }
  
        }


    }
}
