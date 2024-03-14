using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PreUIManager : MonoBehaviour
{
    public Slider ChangeWindwCountSlider;

    public List<ModularHDB> ModHDBs = new();

    public void OnChangeWindowCount()
    {
        foreach(ModularHDB M in ModHDBs)
        {
            M.RefreshAllWindowCount((int)ChangeWindwCountSlider.value);
        }
    }
}
