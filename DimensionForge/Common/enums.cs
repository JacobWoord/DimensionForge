using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge.Common
{
    public enum NodePosition
    {
        RightTop = 0,
        RightMiddle = 1,
        BottomRight = 2,
        LeftTop = 3,
        LeftMiddle = 4,
        BottomLeft = 5,
        MiddleTop,
        MiddleBottom,
        MiddleCenter,
        None
    }

    public enum CornerName
    {
        BottomFrontLeft,
        BottomFrontRight,
        BottomBackLeft,
        BottomBackRight,
        TopFrontLeft,
        TopFrontRight,
        TopBackLeft,
        TopBackRight,
        FrontPlaneCenter,
        BackPlaneCenter,
        LeftPlaneCenter,
        RightPlaneCenter,
        TopPlaneCenter,
        BottomPlaneCenter
    }




    public enum AxisDirection
    {
        LeftRight,
        TopBottom,
        FrontBack
    }




    public enum UseCase
    {
        verlet,
        direction,
        boundings,
        anchorPoints,
        None
    }

}
