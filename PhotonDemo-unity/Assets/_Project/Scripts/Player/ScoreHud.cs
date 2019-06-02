using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ScoreHud : MonoBehaviour
{
    #region Variables

    [Header("Settings")]
    [SerializeField]
    private Slider m_slider;

    #endregion

    public void ScoreAdd(int value)
    {
        m_slider.value += value;
    }

    public void ScoreRemove(int value)
    {
        m_slider.value -= value;
    }

    public void ScoreSet(int value)
    {
        m_slider.value = value;
    }

    public void ClearScore()
    {
        m_slider.value = 0;
    }

}
