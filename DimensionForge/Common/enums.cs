using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge.Common
{
    public enum NodePosition
    {
        RightTop,
        LeftTop,
        BottomLeft,
        BottomRight
        
    }

    public enum BbCornerName
    {
        BinnenLinksBoven = 0,
        BuitenLinksBoven =1,
        BuitenRechtsBoven= 2,
        BinnenRechtsBoven = 3,
        BinnenLinksOnder = 4,
        BuitenRechtsOnder =5,
        BuitenLinksOnder =6,
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
