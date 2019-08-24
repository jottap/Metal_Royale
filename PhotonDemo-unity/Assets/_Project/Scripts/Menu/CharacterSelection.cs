using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public static int charId = 0;

    [SerializeField] private ScrollRect m_scrollRectChars;
    [SerializeField] private RectTransform[] m_charsList;

    private Coroutine m_movimentCoroutine;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        if (m_movimentCoroutine != null) StopCoroutine(m_movimentCoroutine); 
        m_movimentCoroutine = StartCoroutine(Moviment());
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            RightButton();
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            LeftButton();
        }

    }

    [PunRPC]
    public void RightButton() {
        int numList = m_charsList.Length-1;
        if (charId < numList) charId++;
    }

    [PunRPC]
    public void LeftButton() {
        if (charId > 0) charId--;
    }

    private IEnumerator Moviment()
    {
        float scrollDist = 1f / (m_charsList.Length - 1);

        while (true) {

            
            float scrollPos = (float)Math.Round(scrollDist * ((float)charId), 2);

            float curPos = (float)Math.Round(m_scrollRectChars.horizontalNormalizedPosition, 2);

            yield return new WaitForEndOfFrame();

            while (scrollPos != curPos)
            {
                Debug.LogWarning(">>> >>> >>> " + curPos + " - " + scrollPos);

                m_scrollRectChars.horizontalNormalizedPosition = curPos < scrollPos ?
                                                                    curPos + 0.01f :
                                                                    curPos - 0.01f
                                                                    ;

                yield return new WaitForEndOfFrame();

                scrollPos = (float)Math.Round(scrollDist * ((float)charId), 2);

                curPos = (float)Math.Round(m_scrollRectChars.horizontalNormalizedPosition, 2);

            }

        }

    }

}
