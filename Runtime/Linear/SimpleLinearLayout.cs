using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.UGUI.ViewportedLayouts{
    public class SimpleLinearLayout : LinearLayoutT<RectTransform>
    {

        [SerializeField]
        private RectTransform _PrefabOfItem;

        public event ItemViewUpdateEvent itemViewUpdateEvent;

        protected override RectTransform OnInstantiateItemView()
        {
            return Object.Instantiate<RectTransform>(_PrefabOfItem);
        }

        protected override void OnItemUpdate(int index)
        {
            if(itemViewUpdateEvent != null){
                itemViewUpdateEvent(index);
            }
        }
    }
}
