using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.UGUI.ViewportedLayouts{
    public interface ILayoutCalculator
    {
        /// <summary>
        /// 计算指定项的位置和大小
        /// </summary>
        Rect GetItemRect(int index);

        /// <summary>
        /// 计算所有Item项所在的Content节点的大小
        /// </summary>
        /// <returns></returns>
        Rect GetContentRect();

        int itemCount{
            get;set;
        }
        void GetOverlapsInViewport(Rect viewport,ISet<int> overlapsItemIndexes);

    }
}
