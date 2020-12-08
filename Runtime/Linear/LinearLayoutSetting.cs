using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MS.UGUI.ViewportedLayouts{

    [System.Serializable]
    public class LinearLayoutSetting
    {
        
        [SerializeField]
        private Vector2 _itemSize = new Vector2(100,100);

        [SerializeField]
        private float _interval;

        [SerializeField]
        private AxisDirection _flexDirection = AxisDirection.UpToDown;

        [SerializeField]
        private bool _itemCrossAxisSizeMatchParent = false;

        [SerializeField]
        private RectOffset _padding;


        public RectOffset padding{
            get{
                return _padding;
            }
        }

        public AxisDirection flexDirection{
            get{
                return _flexDirection;
            }
        }        

        public bool isMainAxisHorizontal{
            get{
                return flexDirection == AxisDirection.LeftToRight || flexDirection == AxisDirection.RightToLeft;
            }
        }

        public float interval{
            get{
                return _interval;
            }
        }

        public Vector2 itemSize{
            get{
                return _itemSize;
            }
        }

        public bool itemCrossAxisSizeMatchParent{
            get{
                return _itemCrossAxisSizeMatchParent;
            }
        }
    }
}
