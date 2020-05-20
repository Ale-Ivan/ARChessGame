using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ScaleController : MonoBehaviour
{
    ARSessionOrigin m_ARSessionOrigin;
    public Slider scaleSlider;
    private Vector3 initialScale;

    private void Awake()
    {
        m_ARSessionOrigin = GetComponent<ARSessionOrigin>();
        initialScale = m_ARSessionOrigin.transform.localScale;
    }

    void Start()
    {
        scaleSlider.onValueChanged.AddListener(OnSliderVariableChanged);
    }

    public void OnSliderVariableChanged(float value)
    {
        if (scaleSlider != null)
        {
            m_ARSessionOrigin.transform.localScale = initialScale / value;
        }
    }
}
