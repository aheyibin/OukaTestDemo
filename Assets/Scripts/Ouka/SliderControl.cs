using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityAction<Vector2> sliderAction;
    public bool isDown;

    public ScrollRect sr;

    //Start is called before the first frame update
    private Vector2 downPosition;

    void Start()
    {
        //sr = GetComponent<ScrollRect>();
        sliderAction = (v2) => { Debug.Log($" vector 2 is x[{v2.x}] y[{v2.y}]"); };
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(
            $"### down ################################################################################################################################################################################################################################################");
        if (!isDown)
        {
            isDown = true;
            downPosition = new Vector2(sr.horizontalNormalizedPosition, sr.verticalNormalizedPosition);
            Debug.Log($" downPostion x[{downPosition.x}] y[{downPosition.y}]");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"### up");
        if (isDown)
        {
            isDown = false;
            Vector2 upPosition = new Vector2(sr.horizontalNormalizedPosition, sr.verticalNormalizedPosition);
            Debug.Log($" upPosition x[{upPosition.x}] y[{upPosition.y}]");
            sliderAction?.Invoke(upPosition - downPosition);
            Debug.Log($" diff {upPosition - downPosition}");
            Debug.Log($" diff x[{upPosition.x - downPosition.x}] y[{upPosition.y - downPosition.y}]");
        }
    }
}