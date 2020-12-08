using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.UGUI.ViewportedLayouts{
    public class LinearLayoutCalculator : ILayoutCalculator
    {

        private Vector2 _itemSize;
        private float _interval;
        private AxisDirection _direction;
        private int _itemCount;

        public Vector2 itemSize{
            get{
                return _itemSize;
            }set{
                _itemSize = Vector2.Max(Vector2.zero,value);
            }
        }

        public float interval{
            get{
                return _interval;
            }set{
                _interval = value;
            }
        }

        public AxisDirection flexDirection{
            get{
                return _direction;
            }set{
                _direction = value;
            }
        }

        public Rect GetContentRect()
        {
            var sizeInMain = CalculateContentSizeInMainAxis();
            var sizeInCross = CalculateContentSizeInCrossAxis();
            if(isMainAxisHorizontal){
                return new Rect(Vector2.zero,new Vector2(sizeInMain,sizeInCross));
            }else{
                return new Rect(Vector2.zero,new Vector2(sizeInCross,sizeInMain));
            }
        }

        public Rect GetItemRect(int index)
        {
            var startInMainAxis = CalculateStartInMainAxis();
            var itemSizeInMainAxis = this.itemSizeInMainAxis;
            var posInMainAxis = startInMainAxis + (_interval * index  + itemSizeInMainAxis * index) * mainAxisFlag;
            Vector2 pos = new Vector2();
            if(this.isMainAxisHorizontal){
                pos.x = posInMainAxis;
                pos.y = itemSizeInCrossAxis / 2;
            }else{
                pos.y = posInMainAxis;
                pos.x = itemSizeInCrossAxis / 2;
            }
            return new Rect(pos - _itemSize / 2,_itemSize);
        }

        private int mainAxisFlag{
            get{
                switch(_direction){
                    case AxisDirection.LeftToRight:
                    case AxisDirection.DownToUp:
                    return 1;
                    case AxisDirection.UpToDown:
                    case AxisDirection.RightToLeft:
                    return -1;
                }       
                return 1;            
            }
        }
 

        private float CalculateStartInMainAxis(){
            switch(_direction){
                case AxisDirection.LeftToRight:
                return _itemSize.x / 2;
                case AxisDirection.DownToUp:
                return _itemSize.y / 2;
                case AxisDirection.UpToDown:
                return this.CalculateContentSizeInMainAxis() - _itemSize.y / 2;
                case AxisDirection.RightToLeft:
                return this.CalculateContentSizeInMainAxis() - _itemSize.x / 2;
            }
            return 0;
        }

        public float CalculateContentSizeInMainAxis(){
            return itemSizeInMainAxis * _itemCount + _interval * (_itemCount - 1);
        }

        public float CalculateContentSizeInCrossAxis(){
            return itemSizeInCrossAxis;
        }

        public void GetOverlapsInViewport(Rect viewport, ISet<int> overlapsItemIndexes)
        {
            var viewportStartInMainAxis = 0f;
            var viewportEndInMainAxis = 0f;
            var itemStartPositionInMainAxis = 0f;
            switch(flexDirection){
                case AxisDirection.LeftToRight:
                viewportStartInMainAxis = viewport.xMin;
                viewportEndInMainAxis = viewport.xMax;
                itemStartPositionInMainAxis = 0;
                break;
                case AxisDirection.RightToLeft:
                viewportStartInMainAxis = viewport.xMax;
                viewportEndInMainAxis = viewport.xMin;
                itemStartPositionInMainAxis = CalculateContentSizeInMainAxis();
                break;
                case AxisDirection.UpToDown:
                viewportStartInMainAxis = viewport.yMax;
                viewportEndInMainAxis = viewport.yMin;
                itemStartPositionInMainAxis = CalculateContentSizeInMainAxis();
                break;
                case AxisDirection.DownToUp:
                viewportStartInMainAxis = viewport.yMin;
                viewportEndInMainAxis = viewport.yMax;
                itemStartPositionInMainAxis = 0;
                break;
            }


            var visibleItemStart = Mathf.FloorToInt(mainAxisFlag * (viewportStartInMainAxis + interval - itemStartPositionInMainAxis) / (itemSizeInMainAxis + interval));
            visibleItemStart = Mathf.Max(0,visibleItemStart);
            var visibleItemEnd = Mathf.CeilToInt(mainAxisFlag * (viewportEndInMainAxis + interval - itemStartPositionInMainAxis) / (itemSizeInMainAxis + interval));
            visibleItemEnd = Mathf.Min(itemCount-1,visibleItemEnd);
            for(var index = visibleItemStart; index <= visibleItemEnd; index ++){
                overlapsItemIndexes.Add(index);
            }
        }

        private float itemSizeInMainAxis{
            get{
                if(this.isMainAxisHorizontal){
                    return _itemSize.x;
                }else{
                    return _itemSize.y;
                }
            }
        }

        private float itemSizeInCrossAxis{
            get{
                if(this.isMainAxisHorizontal){
                    return _itemSize.y;
                }else{
                    return _itemSize.x;
                }
            }
        }

        public bool isMainAxisHorizontal{
            get{
                return _direction == AxisDirection.LeftToRight || _direction == AxisDirection.RightToLeft;
            }
        }

        public int itemCount {
            get{
                return _itemCount;
            }set{
                _itemCount = Mathf.Max(0,value);
            }
        }
    }
}
