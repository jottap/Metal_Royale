using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingTextAnimation : MonoBehaviour
{
    [SerializeField] private float m_timeLeft = 0.1f;
    private TextMeshProUGUI m_tmpLoading;

    private Coroutine m_textAnimation;

    private void Awake()
    {
        m_tmpLoading = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if(m_textAnimation != null) StopCoroutine(m_textAnimation);
        m_textAnimation = StartCoroutine(TextAnimation_CO());
    }

    private void OnDisable()
    {
        if (m_textAnimation != null) StopCoroutine(m_textAnimation);
    }

    private IEnumerator TextAnimation_CO() {

        while (true) {
            m_tmpLoading.text = "Loading.";
            yield return new WaitForSeconds(m_timeLeft);
            m_tmpLoading.text = "Loading..";
            yield return new WaitForSeconds(m_timeLeft);
            m_tmpLoading.text = "Loading...";
            yield return new WaitForSeconds(m_timeLeft);
        }

    }
}
