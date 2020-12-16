using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.UGUI.ViewportedLayouts{
    public class SimpleFlexGridLayout : FlexGridLayoutT<RectTransform>
    {

        [SerializeField]
        private RectTransform _PrefabOfItem;

        protected override RectTransform OnInstantiateItemView()
        {
            return Object.Instantiate<RectTransform>(_PrefabOfItem);
        }
   
    }
}
