using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowTextureMovement : MonoBehaviour
{
    // Scroll main texture based on time

    [SerializeField] private float m_scrollSpeed = 0.5f;
    RawImage m_rawImage;

    void Start()
    {
        m_rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        float offset = Time.time * m_scrollSpeed;
        m_rawImage.uvRect = new Rect(0, offset, 1, 1); ;
    }
}
