using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;

using TMPro;


public class ARPlacementAndPlaneDetectionController : MonoBehaviour
{

    ARPlaneManager m_ARPlaneManager;
    ARPlacementManager m_ARPlacementManager;

    public TextMeshProUGUI informUiPanelText;

    public GameObject placeButton;
    public GameObject adjustButton;
    public GameObject searchForBattleButton;
    public GameObject scaleSlider;

    private void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        m_ARPlacementManager = GetComponent<ARPlacementManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        placeButton.SetActive(true);
        scaleSlider.SetActive(true);

        adjustButton.SetActive(false);
        searchForBattleButton.SetActive(false);

        informUiPanelText.text = "Move phone to detect planes and place the chess board.";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = false;
        m_ARPlacementManager.enabled = false;

        SetAllPlanesActiveOrDeactive(false);

        placeButton.SetActive(false);
        scaleSlider.SetActive(false);

        adjustButton.SetActive(true);
        searchForBattleButton.SetActive(true);

        informUiPanelText.text = "Great! You placed the Chess Board. Now search for games.";
    }

    public void EnableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = true;
        m_ARPlacementManager.enabled = true;

        SetAllPlanesActiveOrDeactive(true);

        placeButton.SetActive(true);
        scaleSlider.SetActive(true);

        adjustButton.SetActive(false);
        searchForBattleButton.SetActive(false);

        informUiPanelText.text = "Move phone to detect planes and place the chess board.";
    }

    private void SetAllPlanesActiveOrDeactive(bool value)
    {
        foreach (var plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }
}
