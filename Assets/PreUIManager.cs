using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PreUIManager : MonoBehaviour
{
    public Slider ChangeWindwCountSlider;

    public List<ModularHDB> ModHDBs = new();
    public TMP_Text ChangeWindowAmountText;

    public Slider ChangeTargetFloorSlider;
    public TMP_Text ChangeTargetFloorText;
    public void OnChangeWindowCount()
    {
        foreach(ModularHDB M in ModHDBs)
        {
            M.RefreshAllWindowCount((int)ChangeWindwCountSlider.value);
        }
        ChangeWindowAmountText.text = ChangeWindwCountSlider.value.ToString("F0");
    }

    public void OnOverrideTargetFloors()
    {
        ModHDBs[0].ChangeFloors((int)ChangeTargetFloorSlider.value);
        ChangeTargetFloorText.text = ChangeTargetFloorSlider.value.ToString();
    }
}
