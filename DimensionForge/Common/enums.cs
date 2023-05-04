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
        FrontMiddleRight,
        MiddleTop,
        MiddleBottom,
        FrontMiddleLeft,
        Center,
        None

    }

    public enum BbCornerName
    {
        BinnenLinksBoven = 0,
        BuitenLinksBoven = 1,
        BuitenRechtsBoven = 2,
        BinnenRechtsBoven = 3,
        BinnenLinksOnder = 4,
        BuitenRechtsOnder = 5,
        BuitenLinksOnder = 6,
        BinnenRechtsOnder = 7
    }



    public enum UseCase
    {
        verlet,
        direction,
        boundings,
        anchorPoints
    }

}
