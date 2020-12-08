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

    public delegate void ItemViewUpdateEvent(int index);



}
