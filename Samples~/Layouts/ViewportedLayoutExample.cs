using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MS.UGUI.ViewportedLayouts;
using TMPro;
using UnityEngine.UI;

public class ViewportedLayoutExample : MonoBehaviour
{
    public ScrollRect linearScroll;
    public SimpleFlexGridLayout simpleGridLayout;
    public SimpleLinearLayout simpleLinearLayout;

    void Start(){
        simpleGridLayout.itemViewUpdateEvent += ((index)=>{
            var view = simpleGridLayout.GetView(index);
            view.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = index.ToString();
        });

        simpleLinearLayout.itemViewUpdateEvent += ((index)=>{
            var view = simpleLinearLayout.GetView(index);
            view.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = index.ToString();
        });

        simpleLinearLayout.UpdateImmediately();
        linearScroll.verticalNormalizedPosition = 1;
    }
}
