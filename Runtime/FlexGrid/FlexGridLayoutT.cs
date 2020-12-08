using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.UGUI.ViewportedLayouts{

    public abstract class FlexGridLayoutT<TView> : LayoutBehaviourT<TView> where TView:Component
    {

        [SerializeField]
        private FlexGridLayoutSetting _setting;

        [SerializeField]
        private int _itemCount = 1;
        private float _lastContentSizeInMainAxis = - 1;

        protected override ILayoutCalculator OnLayoutCalculatorCreate()
        {
            var layout = new PaddedLayoutCalculator(new FlexGridLayoutCalculator());
            return layout;
        }

        protected override void OnLayoutCalculatorConfigurate(ILayoutCalculator calculator)
        {
            this.UpdateLayoutSetting(calculator);
        }

        private FlexGridLayoutCalculator gridLayoutCalculator{
            get{
                var paddedLayout = (this.layoutCalculator as PaddedLayoutCalculator);
                return paddedLayout.innerLayout as FlexGridLayoutCalculator;
            }
        }

        private float contentSizeInMainAxis{
            get{
                if(_setting.flexDirection == AxisDirection.LeftToRight || _setting.flexDirection == AxisDirection.RightToLeft){
                    return content.rect.width;
                }else{
                    return content.rect.height;
                }
            }
        }

        private void UpdateLayoutSetting(ILayoutCalculator layoutCalculator){
            var paddedLayout = layoutCalculator as PaddedLayoutCalculator;
            var gridLayout = paddedLayout.innerLayout as FlexGridLayoutCalculator;
            paddedLayout.padding = _setting.padding;
            gridLayout.flexDirection = _setting.flexDirection;
            gridLayout.itemSize = _setting.itemSize;
            gridLayout.interval = _setting.interval;
            gridLayout.alignItemsInMainAxis = _setting.alignItemsInMainAxis;
            gridLayout.crossAxisDirection = _setting.crossAxisDirection;    
            if(gridLayout.isFlexHorizontal){
                float flexMaxSize = - _setting.padding.horizontal;
                switch(_setting.flexSizeType){
                    case FlexSizeType.MatchTransform:
                    flexMaxSize += (transform as RectTransform).rect.width;
                    break;
                    case FlexSizeType.Custom:
                    flexMaxSize += _setting.flexSize;
                    break;
                }
                gridLayout.flexMaxSize = flexMaxSize;    
            }else{
                float flexMaxSize = - _setting.padding.vertical;
                switch(_setting.flexSizeType){
                    case FlexSizeType.MatchTransform:
                    flexMaxSize += (transform as RectTransform).rect.height;
                    break;
                    case FlexSizeType.Custom:
                    flexMaxSize += _setting.flexSize;
                    break;
                }
                gridLayout.flexMaxSize = flexMaxSize;
            }
            gridLayout.itemCount = _itemCount;
        }

        public int itemCount{
            get{
                return _itemCount;
            }set{
                if(_itemCount == value){
                    return;
                }
                _itemCount = value;
                SetLayoutConfigDirty();
            }
        }


        protected override void Update()
        {
            if(_setting.flexSizeType == FlexSizeType.MatchTransform){
                var contentSizeInMainAxis = this.contentSizeInMainAxis;
                if(_lastContentSizeInMainAxis != contentSizeInMainAxis){
                    _lastContentSizeInMainAxis = contentSizeInMainAxis;
                    SetLayoutConfigDirty();
                }
            }
            base.Update();
        }


    }


}
