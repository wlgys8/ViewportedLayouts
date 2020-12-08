using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MS.UGUI.ViewportedLayouts{

    public enum AxisDirection{
        LeftToRight,
        RightToLeft,

        UpToDown,
        DownToUp,
    }

    public enum CrossAxisDirection{
        Default,
        Reverse,
    }

    public enum AlignItems{
        FlexStart,
        FlexEnd,
        Center,
    }


    public delegate void ItemViewUpdateEvent(int index);



}
