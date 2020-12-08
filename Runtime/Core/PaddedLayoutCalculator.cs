using System.Collections.Generic;
using UnityEngine;


namespace MS.UGUI.ViewportedLayouts{
    public class PaddedLayoutCalculator : ILayoutCalculator
    {
        private ILayoutCalculator _innerLayout;

        public PaddedLayoutCalculator(ILayoutCalculator innerLayout){
            _innerLayout = innerLayout;
            this.padding = new RectOffset();
        }

        public RectOffset padding{
            get;set;
        }

        public ILayoutCalculator innerLayout{
            get{
                return _innerLayout;
            }
        }

        public int itemCount {
            get{
                return _innerLayout.itemCount;
            }set{
                _innerLayout.itemCount = value;
            }
        }

        public Rect GetContentRect()
        {
            var innerContentRect = _innerLayout.GetContentRect();
            return new Rect(Vector2.zero,innerContentRect.size + new Vector2(padding.horizontal,padding.vertical));
        }

        public Rect GetItemRect(int index)
        {
            var rect = _innerLayout.GetItemRect(index);
            rect.position += new Vector2(padding.left,padding.bottom);
            return rect;
        }

        public void GetOverlapsInViewport(Rect viewport, ISet<int> overlapsItemIndexes)
        {
            var padding = this.padding;
            viewport.position -= new Vector2(padding.left,padding.bottom);
            innerLayout.GetOverlapsInViewport(viewport,overlapsItemIndexes);
        }
    }
}
