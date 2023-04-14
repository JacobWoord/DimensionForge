using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge.HelperTools
{
    using SharpDX;
    using System.Windows.Media.Media3D;

    public static class Transform3DHelper
    {
        public static Transform3D Combine(Transform3D left, Transform3D right)
        {
            if (left == null || left == Transform3D.Identity)
                return right;

            if (right == null || right == Transform3D.Identity)
                return left;

            var group = left as Transform3DGroup;

            if (group == null)
            {
                group = new Transform3DGroup();
                group.Children.Add(left);
            }

            group.Children.Add(right);

            return group;
        }

        public static Matrix ToSharpDX(this Matrix3D matrix)
        {
            return new Matrix(
                (float)matrix.M11, (float)matrix.M12, (float)matrix.M13, (float)matrix.M14,
                (float)matrix.M21, (float)matrix.M22, (float)matrix.M23, (float)matrix.M24,
                (float)matrix.M31, (float)matrix.M32, (float)matrix.M33, (float)matrix.M34,
                (float)matrix.OffsetX, (float)matrix.OffsetY, (float)matrix.OffsetZ, (float)matrix.M44);
        }
    }

}
