using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.UGUI.ViewportedLayouts{


    /// <summary>
    /// 计算一个空间区域内的格子布局。默认坐标原点(0,0)在左下角
    /// </summary>
    public class FlexGridLayoutCalculator:ILayoutCalculator{


        [System.Diagnostics.Conditional("MUV_INTERNAL_DEBUG")]
        private static void InternalDebug(string msg){
            Debug.Log(msg);
        }
 
        private Vector2 _itemSize = new Vector2(100,100);

        /// <summary>
        /// 主轴方向最大扩展尺寸
        /// </summary>
        private float _flexMaxSize = 400;

        private AxisDirection _flexDirection = AxisDirection.LeftToRight;

        private AlignItems _alignItemsInMainAxis = AlignItems.FlexStart;

        // private AlignItemsCrossAxis _alignItemsInCrossAxis = AlignItemsCrossAxis.FlexStart;

        private CrossAxisDirection _crossAxisDirection = CrossAxisDirection.Default;

        private Vector2 _interval = new Vector2(0,0);

        //cache values
        private int? _itemCountInMainAxis = null;
        private float? _contentSizeInCrossAxis = null;
        private int _itemCount = 0;

        public void SetDirty(){
            SetItemCountInMainAxisDirty();
            SetContentSizeInCrossAxisDirty();
        }

        private void SetItemCountInMainAxisDirty(){
            _itemCountInMainAxis = null;
            SetContentSizeInCrossAxisDirty();
        }

        private void SetContentSizeInCrossAxisDirty(){
            _contentSizeInCrossAxis = null;
        }

        /// <summary>
        /// 设置/获取需要布局的Item数量
        /// </summary>
        /// <value></value>
        public int itemCount{
            get{
                return _itemCount;
            }set{
                _itemCount = value;
            }
        }

        // public AlignItemsCrossAxis alignItemsInCrossAxis{
        //     get{
        //         return _alignItemsInCrossAxis;
        //     }set{
        //         _alignItemsInCrossAxis = value;
        //     }
        // }


        public AlignItems alignItemsInMainAxis{
            get{
                return _alignItemsInMainAxis;
            }set{
                _alignItemsInMainAxis = value;
            }
        }

        public AxisDirection flexDirection{
            get{
                return _flexDirection;
            }set{
                _flexDirection = value;
                SetItemCountInMainAxisDirty();
            }
        }

        public CrossAxisDirection crossAxisDirection{
            get{
                return _crossAxisDirection;
            }set{
                _crossAxisDirection = value;
            }
        }


        public Vector2 itemSize{
            get{
                return _itemSize;
            }set{
                _itemSize = value;
            }
        }

    
        public float flexMaxSize{
            set{
                _flexMaxSize = value;
                SetItemCountInMainAxisDirty();
            }get{
                return _flexMaxSize;
            }
        }

        public Vector2 interval{
            get{
                return _interval;
            }set{
                _interval = value;
                SetItemCountInMainAxisDirty();
            }
        }

        private float itemSizeInMainAxis{
            get{
                if(isFlexHorizontal){
                    return _itemSize.x;
                }else{
                    return _itemSize.y;
                }
            }
        }

        private float itemSizeInCrossAxis{
            get{
                if(isFlexHorizontal){
                    return _itemSize.y;
                }else{
                    return _itemSize.x;
                }
            }
        }

        private float intervalSizeInMainAxis{
            get{
                return isFlexHorizontal?_interval.x:_interval.y;
            }
        }

        private float intervalSizeInCrossAxis{
            get{
                return isFlexHorizontal?_interval.y:_interval.x;
            }
        }

        /// <summary>
        /// 计算主轴方向的Item数量
        /// </summary>
        /// <returns></returns>
        public int CalculateItemCountInMainAxis(){
            if( _itemCountInMainAxis == null){
                var itemSizeInMainAxis =  this.itemSizeInMainAxis;
                var intervalSizeInMainAxis = this.intervalSizeInMainAxis;
                var itemCountInMainAxis = Mathf.FloorToInt((flexMaxSize + intervalSizeInMainAxis) / (itemSizeInMainAxis + intervalSizeInMainAxis));
                if(itemCountInMainAxis <= 0){
                    itemCountInMainAxis = 1;
                }
                _itemCountInMainAxis = itemCountInMainAxis;
                return itemCountInMainAxis;
            }
            return _itemCountInMainAxis.Value;
        }

        /// <summary>
        /// 计算交叉轴方向的尺寸
        /// </summary>
        public float CalculateContentSizeInCrossAxis(){
            if(_contentSizeInCrossAxis == null){
                var itemCountInMainAxis = this.CalculateItemCountInMainAxis();
                var lineCount = Mathf.CeilToInt( this._itemCount *1f / itemCountInMainAxis );
                var contentSizeInCrossAxis = lineCount * itemSizeInCrossAxis + (lineCount - 1) * intervalSizeInCrossAxis;
                _contentSizeInCrossAxis = contentSizeInCrossAxis;
                return contentSizeInCrossAxis;
            }
            return _contentSizeInCrossAxis.Value;
        }

        public Rect GetContentRect(){
            if(this.isFlexHorizontal){
                return new Rect(Vector2.zero, new Vector2(flexMaxSize,CalculateContentSizeInCrossAxis()));
            }else{
                return new Rect(Vector2.zero, new Vector2(CalculateContentSizeInCrossAxis(),flexMaxSize));
            }
        }

        public bool isFlexHorizontal{
            get{
                return _flexDirection == AxisDirection.LeftToRight || _flexDirection == AxisDirection.RightToLeft;
            }
        }

        /// <summary>
        /// x为主轴,y为副轴
        /// </summary>
        private Vector2Int axisDirection{
            get{
                var result = Vector2Int.one;
                switch(_flexDirection){
                    case AxisDirection.LeftToRight:
                    case AxisDirection.DownToUp:
                    result.x = 1;
                    break;
                    case AxisDirection.RightToLeft:
                    case AxisDirection.UpToDown:
                    result.x = -1;
                    break;
                }
                var isFlexHorizontal = this.isFlexHorizontal;
                switch(crossAxisDirection){
                    case CrossAxisDirection.Default:
                    result.y = 1;
                    break;
                    case CrossAxisDirection.Reverse:
                    result.y = -1;
                    break;
                }
                return result;
            }
        }

        public Rect GetItemRect(int itemIndex){
            var position = CalculateItemCenterPosition(itemIndex);
            return new Rect(position - itemSize / 2,itemSize);
        }
 
        private float CalculateItemStartPositionInMainAxis(){
            var maxItemCountInMainAxis = this.CalculateItemCountInMainAxis();
            var itemCountInMainAxis = Mathf.Min(maxItemCountInMainAxis,this.itemCount);
            ///主坐标排完Item后留下的空位
            var leftSpaceInMainAxis = flexMaxSize  - (itemCountInMainAxis * itemSizeInMainAxis) - (itemCountInMainAxis - 1) * intervalSizeInMainAxis;
            var result = 0f;
            switch(_alignItemsInMainAxis){
                case AlignItems.FlexStart:
                result = 0;
                break;
                case AlignItems.FlexEnd:
                result = leftSpaceInMainAxis;
                break;
                case AlignItems.Center:
                result = leftSpaceInMainAxis / 2;
                break;
            }
            if(_flexDirection == AxisDirection.RightToLeft || _flexDirection == AxisDirection.UpToDown){
                result = flexMaxSize - result;
            }
  
            return result;
        }

        /// <summary>
        /// 以左下为原点，计算出Items在CrossAxis的启始偏移
        /// </summary>
        private float CalculateItemStartPositionInCrossAxis(){
            switch(crossAxisDirection){
                case CrossAxisDirection.Default:
                return 0;
                case CrossAxisDirection.Reverse:
                return this.CalculateContentSizeInCrossAxis();
            }
            return 0;
        }
        /// <summary>
        /// 以左下为原点，计算出Items在CrossAxis的结束偏移
        /// </summary>
        private float CalculateItemEndPositionInCrossAxis(){
            switch(crossAxisDirection){
                case CrossAxisDirection.Default:
                return this.CalculateContentSizeInCrossAxis();
                case CrossAxisDirection.Reverse:
                return 0;
            }
            return 0;            
        }

        private Vector2 CalculateItemCenterPosition(int itemIndex){
            var itemCountInMainAxis = this.CalculateItemCountInMainAxis();
            var indexInCrossAxis = Mathf.FloorToInt(itemIndex / itemCountInMainAxis);
            var indexInMainAxis = itemIndex % itemCountInMainAxis;
            var itemSizeInCrossAxis = this.itemSizeInCrossAxis;
            var direction = this.axisDirection;
            var posStartInCrossAxis = this.CalculateItemStartPositionInCrossAxis();
            var posInCrossAxis = posStartInCrossAxis + direction.y * (itemSizeInCrossAxis / 2 + indexInCrossAxis * (itemSizeInCrossAxis + intervalSizeInCrossAxis));
            var posStartInMainAxis = this.CalculateItemStartPositionInMainAxis();
            var posInMainAxis = posStartInMainAxis + direction.x * (itemSizeInMainAxis / 2 + indexInMainAxis * (itemSizeInMainAxis + intervalSizeInMainAxis));
            if(isFlexHorizontal){
                return new Vector2(posInMainAxis,posInCrossAxis);
            }else{
                return new Vector2(posInCrossAxis,posInMainAxis);
            }
        }

        private void ConvertViewportToFlexSpace(Rect viewport,ref ViewportInfo info){
            switch(flexDirection){
                case AxisDirection.LeftToRight:
                info.startInMainAxis = viewport.xMin;
                info.endInMainAxis = viewport.xMax;
                break;
                case AxisDirection.RightToLeft:
                info.startInMainAxis = viewport.xMax;
                info.endInMainAxis = viewport.xMin;
                break;
                case AxisDirection.UpToDown:
                info.startInMainAxis = viewport.yMax;
                info.endInMainAxis = viewport.yMin;
                break;
                case AxisDirection.DownToUp:
                info.startInMainAxis = viewport.yMin;
                info.endInMainAxis = viewport.yMax;
                break;
            }

            if(isFlexHorizontal){
                switch(crossAxisDirection){
                    case CrossAxisDirection.Default:
                    info.startInCrossAxis = viewport.yMin;
                    info.endInCrossAxis = viewport.yMax;
                    break;
                    case CrossAxisDirection.Reverse:
                    info.startInCrossAxis = viewport.yMax;
                    info.endInCrossAxis = viewport.yMin;
                    break;
                }     
            }else{
                switch(crossAxisDirection){
                    case CrossAxisDirection.Default:
                    info.startInCrossAxis = viewport.xMin;
                    info.endInCrossAxis = viewport.xMax;
                    break;
                    case CrossAxisDirection.Reverse:
                    info.startInCrossAxis = viewport.xMax;
                    info.endInCrossAxis = viewport.xMin;
                    break;
                }  
            }
        }



        /// <summary>
        /// 计算在指定区域内，可见的items
        /// viewport是以布局左下角作为原点计算出的可见区域
        /// </summary>
        public void GetOverlapsInViewport(Rect viewport,ISet<int> resultIndexes){
            resultIndexes.Clear();
            var axisDirection = this.axisDirection;
            
            ViewportInfo viewportInFlexSpace = new ViewportInfo();
            ConvertViewportToFlexSpace(viewport,ref viewportInFlexSpace);
            var viewportStartInMainAxis = viewportInFlexSpace.startInMainAxis;
            var viewportEndInMainAxis = viewportInFlexSpace.endInMainAxis;
            var viewportStartInCrossAxis = viewportInFlexSpace.startInCrossAxis;
            var viewportEndInCrossAxis = viewportInFlexSpace.endInCrossAxis;

            InternalDebug($"[GetOverlapsInViewport] axisDir = {axisDirection}, count = {itemCount}, originalViewport = {viewport}");
            InternalDebug($"[GetOverlapsInViewport] viewportStartInMainAxis:{viewportStartInMainAxis},viewportEndInMainAxis:{viewportEndInMainAxis},startInCrossAxis:{viewportStartInCrossAxis},endInCrossAxis{viewportEndInCrossAxis}");
            var itemStartPosInMainAxis = this.CalculateItemStartPositionInMainAxis();
            var itemCountInMainAxis = this.CalculateItemCountInMainAxis();
            var itemEndPosInMainAxis = itemStartPosInMainAxis + axisDirection.x * (this.itemSizeInMainAxis * itemCountInMainAxis + this.intervalSizeInMainAxis * (itemCountInMainAxis - 1));

            var itemStartPosInCrossAxis = this.CalculateItemStartPositionInCrossAxis();
            var itemEndPosInCrossAxis = this.CalculateItemEndPositionInCrossAxis();

            InternalDebug($"itemStartPosInMainAxis:{itemStartPosInMainAxis},itemEndPosInMainAxis:{itemEndPosInMainAxis}," + 
            $"itemStartPosInCrossAxis:{itemStartPosInCrossAxis},itemEndPosInCrossAxis:{itemEndPosInCrossAxis}");

            if( (viewportStartInMainAxis - itemEndPosInMainAxis) * axisDirection.x >= 0 || (viewportEndInMainAxis - itemStartPosInMainAxis ) * axisDirection.x <= 0 ){
                return;
            }

            InternalDebug($"itemCountInMainAxis:{itemCountInMainAxis}");

            var visibleItemIndexMinInMainAxis =  Mathf.FloorToInt(axisDirection.x *((viewportStartInMainAxis - itemStartPosInMainAxis) 
            + this.intervalSizeInMainAxis) / (this.itemSizeInMainAxis + this.intervalSizeInMainAxis));
            visibleItemIndexMinInMainAxis = Mathf.Clamp(visibleItemIndexMinInMainAxis,0,itemCountInMainAxis - 1);

            var visibleItemIndexMaxInMainAxis =  Mathf.FloorToInt(axisDirection.x *(viewportEndInMainAxis - itemStartPosInMainAxis) 
            / (this.itemSizeInMainAxis + this.intervalSizeInMainAxis));;
            visibleItemIndexMaxInMainAxis = Mathf.Clamp(visibleItemIndexMaxInMainAxis,0,itemCountInMainAxis - 1);

            var visibleItemIndexMinInCrossAxis =  Mathf.FloorToInt(axisDirection.y *((viewportStartInCrossAxis - itemStartPosInCrossAxis) 
            + this.intervalSizeInCrossAxis) / (this.itemSizeInCrossAxis + this.intervalSizeInCrossAxis));

            var visibleItemIndexMaxInCrossAxis =  Mathf.FloorToInt(axisDirection.y *(viewportEndInCrossAxis - itemStartPosInCrossAxis)
             / (this.itemSizeInCrossAxis + this.intervalSizeInCrossAxis));;

            InternalDebug($@"visibleItemIndexRangeInMainAxis = [{visibleItemIndexMinInMainAxis},{visibleItemIndexMaxInMainAxis}],
            visibleItemIndexRangeInCrossAxis = [ {visibleItemIndexMinInCrossAxis},{visibleItemIndexMaxInCrossAxis}]");
            
            for(var j = visibleItemIndexMinInCrossAxis; j <= visibleItemIndexMaxInCrossAxis;j ++){
                for(var i = visibleItemIndexMinInMainAxis; i <= visibleItemIndexMaxInMainAxis; i ++){
                    var itemIndex = j * itemCountInMainAxis + i;
                    if(itemIndex >= this._itemCount){
                        continue;
                    }
                    if(itemIndex < 0){
                        continue;
                    }
                    resultIndexes.Add(itemIndex);
                }
            }
        }

        private struct ViewportInfo{
            public float startInMainAxis;
            public float endInMainAxis;
            public float startInCrossAxis;
            public float endInCrossAxis;
        }
    }

}
