using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.UGUI.ViewportedLayouts{
    [System.Serializable]
    public class FlexGridLayoutSetting{

        [SerializeField]
        private AxisDirection _flexDirection;

        [SerializeField]
        private CrossAxisDirection _crossAxisDirection;

        [SerializeField]
        private AlignItems _alignItemsInMainAxis = AlignItems.FlexStart;

        // [SerializeField]
        // private AlignItemsCrossAxis _alignItemsInCrossAxis = AlignItemsCrossAxis.FlexStart;

        [SerializeField]
        private Vector2 _itemSize = new Vector2(100,100);

        [SerializeField]
        private Vector2 _interval = new Vector2(0,0);

        [SerializeField]
        private FlexSizeType _flexSizeType = FlexSizeType.MatchTransform;

        [SerializeField]
        private float _flexSize = 300;

        [SerializeField]
        private RectOffset _padding;

        public AxisDirection flexDirection{
            get{
                return _flexDirection;
            }
        }

        public AlignItems alignItemsInMainAxis{
            get{
                return _alignItemsInMainAxis;
            }
        }

        // public AlignItemsCrossAxis alignItemsInCrossAxis{
        //     get{
        //         return _alignItemsInCrossAxis;
        //     }
        // }

        public CrossAxisDirection crossAxisDirection{
            get{
                return _crossAxisDirection;
            }
        }

        public float flexSize{
            get{
                return _flexSize;
            }
        }

        public FlexSizeType flexSizeType{
            get{
                return _flexSizeType;
            }
        }

        public Vector2 itemSize{
            get{
                return _itemSize;
            }
        }

        public Vector2 interval{
            get{
                return _interval;
            }
        }

        public RectOffset padding{
            get{
                return _padding;
            }
        }
    }

    public enum FlexSizeType{
        MatchTransform,
        Custom,
    }
}
