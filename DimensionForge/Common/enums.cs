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
        LowerLeftFrontCorner,
        LowerRightFrontCorner,
        UpperRightFrontCorner,
        UpperLeftFrontCorner,
        LowerLeftBackCorner,
        LowerRightBackCorner,
        UpperRightBackCorner,
        UpperLeftBackCorner,
        FrontPlaneCenter,
        BackPlaneCenter
    }




    public enum UseCase
    {
        verlet,
        direction,
        boundings,
        anchorPoints
    }

}
