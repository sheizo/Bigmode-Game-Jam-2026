using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private RectTransform _rampSelection;

    public void SetRampSprites(Queue<Ramp> ramps){
        int i = 0;
        foreach (Ramp ramp in ramps){
            _rampSelection.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = ramp.RampSprite;
            i++;
        }
    }
}
