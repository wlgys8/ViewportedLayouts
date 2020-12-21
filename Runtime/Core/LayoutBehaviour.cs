// #define MUV_INTERNAL_DEBUG
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace MS.UGUI.ViewportedLayouts{
    using CommonUtils;

    public abstract class LayoutBehaviourT<TView>:LayoutBehaviour where TView:Component {
        // private static readonly ConditionalLogger logger = LogFactory.GetConditionalLogger<LayoutBehaviourT<TView>>();
        
        [System.Diagnostics.Conditional("MUV_INTERNAL_DEBUG")]
        private static void InternalDebug(string msg){
            Debug.Log(msg);
        }

        private IObjectPool<TView> _viewPool = new ObjectPool<TView>();
        private Dictionary<int,TView> _visibleItemViews = new Dictionary<int, TView>();

        [SerializeField]
        private bool _controlItemWidth = true;

        [SerializeField]
        private bool _controlItemHeight=  true;

        private TView RequestItemView(){
            if(_viewPool.Count > 0){
                return _viewPool.Request();
            }else{
                return OnInstantiateItemView();
            }   
        }

        private void ReleaseItemView(TView itemView){
            _viewPool.Release(itemView);
            itemView.gameObject.SetActive(false);
        }

        public bool HasView(int index){
            return _visibleItemViews.ContainsKey(index);
        }

        public TView GetView(int index){
            return _visibleItemViews[index];
        }

        protected override void OnItemVisibleChanged(int index, bool visible)
        {
            InternalDebug($"item visible changed,index = {index},visible = {visible}");
            if(visible){
                var itemView = RequestItemView();
                var itemTrans = itemView.transform as RectTransform;
                itemTrans.SetParent(content,false);
                itemView.gameObject.SetActive(true);
                _visibleItemViews.Add(index,itemView);
            }else{
                TView itemView;
                if(_visibleItemViews.TryGetValue(index,out itemView)){
                    _visibleItemViews.Remove(index);
                }
                this.ReleaseItemView(itemView);
            }
        }

        protected override void OnItemLayout(int index)
        {
            var itemRect = this.GetItemRect(index);
            var itemTrans = _visibleItemViews[index].transform as RectTransform;
            itemTrans.localPosition = itemRect.position + itemTrans.pivot * itemRect.size;
            if(_controlItemWidth){
                itemTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,itemRect.width);
            }
            if(_controlItemHeight){
                itemTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,itemRect.height);
            }
        }

        protected abstract TView OnInstantiateItemView();
    }
    

    public enum ViewportUpdateMode{
        Manual,
        EveryFrame,
    }


    public abstract class LayoutBehaviour : UIBehaviour
    {
    
        [SerializeField]
        private RectTransform _viewport;

        [SerializeField]
        private RectTransform _content;

        [SerializeField]
        private ViewportUpdateMode _viewportUpdateMode = ViewportUpdateMode.EveryFrame;

        [SerializeField]
        private int _itemCount = 1;

        private Rect _viewportRect;
        private ILayoutCalculator _layoutCalculator;

        private HashSet<int> _visibleItems;

        private bool _layoutConfigDirty = false;
        private bool _layoutDirty = false;
        private bool _viewportDirty = false;
        private bool _itemsDirty = false;
        public event ItemViewUpdateEvent itemViewUpdateEvent;


        protected override void Awake(){
            base.Awake();
        }

#if UNITY_EDITOR
        protected override void OnValidate(){
            base.OnValidate();
            this.SetLayoutConfigDirty();
        }
#endif

        public void AddItemViewUpdate(ItemViewUpdateEvent callback){
            this.itemViewUpdateEvent += callback;
        }

        public void RemoveItemViewUpdate(ItemViewUpdateEvent callback){
            this.itemViewUpdateEvent -= callback;
        }

        protected void SetLayoutConfigDirty(){
            _layoutConfigDirty = true;
        }

        public void SetLayoutDirty(){
            _layoutDirty = true;
        }

        protected void SetViewportDirty(){
            _viewportDirty = true;
        }

        /// <summary>
        /// If SetItemsDirty called, OnItemUpdate wil be invoked later for each visible item.
        /// </summary>
        public void SetItemsDirty(){
            _itemsDirty = true;
        }


        public RectTransform viewport{
            get{
                return _viewport;
            }
        }

        public RectTransform content{
            get{
                if(!_content){
                    _content = this.transform as RectTransform;
                }
                return _content;
            }
        }

        internal ILayoutCalculator layoutCalculator{
            get{
                if(_layoutCalculator == null){
                    _layoutCalculator = OnLayoutCalculatorCreate();
                    this.ConfigurateLayoutCalculator();
                }
                return _layoutCalculator;
            }
        }

        /// <summary>
        /// get the item position/size rect in content space
        /// </summary>
        public Rect GetItemRect(int index){
            var contentPosition = this.content.rect.position;
            var itemRect = this.layoutCalculator.GetItemRect(index);
            itemRect.position += contentPosition;
            return itemRect;            
        }

        /// <summary>
        /// update the content transform size immediately.
        /// </summary>
        public void UpdateContentSize(){
            var contentRect = layoutCalculator.GetContentRect();
            this.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,contentRect.height);
            this.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,contentRect.width);
        }

        private void UpdateViewport(Rect viewportRect){
            if(_viewportRect == viewportRect){
                return;
            }
            _viewportRect = viewportRect;
            this.SetViewportDirty();
        }

        protected void UpdateViewport(){
            if(viewport == null){
                return;
            }
            this.UpdateViewport(this.CalculateViewportRectInContentSpace());
        }
        
        private Rect CalculateViewportRectInContentSpace(){
            var viewportPivotPosInContentSpace = (Vector2) content.worldToLocalMatrix.MultiplyPoint3x4(viewport.position);
            var viewportRect = viewport.rect;
            var viewportInContentSpace = new Rect( viewportRect.position + viewportPivotPosInContentSpace - content.rect.position,viewportRect.size);
            return viewportInContentSpace;
        }

        private void UpdateViews(){
            var turnToVisibleItems = SetPool<int>.Request();
            try{
                if(_viewportDirty){
                    InternalDebug("calculate item views visible");
                    if(_visibleItems == null){
                        _visibleItems = SetPool<int>.Request(); 
                    }
                    var newVisibleItems = SetPool<int>.Request();
                    this.layoutCalculator.GetOverlapsInViewport(_viewportRect,newVisibleItems);
                    var oldVisibleItems = _visibleItems;
                    try{
                        foreach(var index in oldVisibleItems){
                            if(!newVisibleItems.Contains(index)){
                                //change to invisible
                                OnItemVisibleChanged(index,false);
                            }
                        }
                        foreach(var index in newVisibleItems){
                            if(!oldVisibleItems.Contains(index)){
                                //change to visible
                                OnItemVisibleChanged(index,true);
                                turnToVisibleItems.Add(index);
                            }
                        }
                    }finally{
                        _visibleItems = newVisibleItems;
                        SetPool<int>.Release(oldVisibleItems);
                    }
                }

                if(_itemsDirty){
                    foreach(var index in _visibleItems){
                        OnItemUpdate(index);
                    }
                }else{
                    foreach(var index in turnToVisibleItems){
                        OnItemUpdate(index);
                    }
                }

                if(_layoutDirty){
                    InternalDebug("calculate item views layout");
                    foreach(var index in _visibleItems){
                        OnItemLayout(index);
                    }
                }else{
                    foreach(var index in turnToVisibleItems){
                        OnItemLayout(index);
                    }
                }
            }finally{
                _viewportDirty = false;
                _itemsDirty = false;
                _layoutDirty = false;
                SetPool<int>.Release(turnToVisibleItems);
            }
        }

        private void DoUpdateIfDirty(){
            if(_layoutConfigDirty){
                _layoutConfigDirty = false;
                if(_layoutCalculator != null){
                    this.ConfigurateLayoutCalculator();
                }
            }
            if(_viewportUpdateMode == ViewportUpdateMode.EveryFrame){
                this.UpdateViewport();
            }
            this.UpdateViews();
        }

        public void UpdateImmediately(){
            this.DoUpdateIfDirty();
        }

        protected virtual void Update(){
            DoUpdateIfDirty();
        }

        private void ConfigurateLayoutCalculator(){
            InternalDebug("ConfigurateLayoutCalculator");
            this.OnLayoutCalculatorConfigurate(_layoutCalculator);
            this.SetLayoutDirty();
            this.UpdateContentSize();
        }

        protected abstract void OnItemVisibleChanged(int index,bool visible);

        protected virtual void OnItemUpdate(int index){
            if(this.itemViewUpdateEvent != null){
                this.itemViewUpdateEvent(index);
            }
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

        protected abstract void OnItemLayout(int index);

        protected abstract ILayoutCalculator OnLayoutCalculatorCreate();


        protected abstract void OnLayoutCalculatorConfigurate(ILayoutCalculator calculator);

     
            
        [System.Diagnostics.Conditional("MUV_INTERNAL_DEBUG")]
        private static void InternalDebug(string msg){
            Debug.Log(msg);
        }
    }
}
