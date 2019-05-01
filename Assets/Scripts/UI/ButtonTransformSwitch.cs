using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTransformSwitch : MonoBehaviour {
    public RectTransform bigTranform;
    public RectTransform littleTranform;
    RectTransform thisTranform;

    private void OnEnable()
    {
        thisTranform = GetComponent<RectTransform>();
    }


   public void SwitchTransform(bool useBig)
    {
        if (useBig)
        {
            thisTranform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bigTranform.rect.width);
            thisTranform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   bigTranform.rect.height);
        } else
        {
            thisTranform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, littleTranform.rect.width);
            thisTranform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   littleTranform.rect.height);
        }
    }
}
