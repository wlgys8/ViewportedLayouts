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
            if(_PrefabOfItem == null){
                return new GameObject("LayoutItem").AddComponent<RectTransform>();
            }else{
                return Object.Instantiate<RectTransform>(_PrefabOfItem);
            }
        }

        public RectTransform PrefabOfItem{
            get{
                return _PrefabOfItem;
            }set{
                _PrefabOfItem = value;
            }
        }
   
    }
}
