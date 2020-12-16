using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.UGUI.ViewportedLayouts{
    public abstract class LinearLayoutT<TView> : LayoutBehaviourT<TView> where TView : Component
    {
        [SerializeField]
        private LinearLayoutSetting _setting;

        private float _lastContentSizeInCrossAxis = 0;

        public float contentSizeInCrossAxis{
            get{
                var contentRect = this.content.rect;
                if(_setting.isMainAxisHorizontal){
                    return contentRect.height;
                }else{
                    return contentRect.width;
                }
            }
        }
       

        protected override void OnLayoutCalculatorConfigurate(ILayoutCalculator calculator)
        {
            var paddedLayout = calculator as PaddedLayoutCalculator;
            var padding = _setting.padding;
            paddedLayout.padding = padding;

            var linearLayout = paddedLayout.innerLayout as LinearLayoutCalculator;
            linearLayout.flexDirection = _setting.flexDirection;
            var itemSize = _setting.itemSize;
            if(_setting.itemCrossAxisSizeMatchParent){
                var contentRect = this.content.rect;
                if(_setting.isMainAxisHorizontal){
                    itemSize.y = contentRect.height - padding.vertical;
                    _lastContentSizeInCrossAxis = itemSize.y;
                }else{
                    itemSize.x = contentRect.width - padding.horizontal;
                    _lastContentSizeInCrossAxis = itemSize.x;
                }
            }
            linearLayout.itemSize = itemSize;
            linearLayout.interval = _setting.interval;
            linearLayout.itemCount = this.itemCount;
        }

        protected override ILayoutCalculator OnLayoutCalculatorCreate()
        {
            return new PaddedLayoutCalculator(new LinearLayoutCalculator());
        }


        protected override void Update()
        {
            if(_setting.itemCrossAxisSizeMatchParent){
                //keeps checking if content size in cross axis changed.
                var contentSizeInCrossAxis = this.contentSizeInCrossAxis;
                if(_lastContentSizeInCrossAxis != contentSizeInCrossAxis){
                    _lastContentSizeInCrossAxis = contentSizeInCrossAxis;
                    SetLayoutConfigDirty();
                }
            }
            base.Update();
        }

    }
}
