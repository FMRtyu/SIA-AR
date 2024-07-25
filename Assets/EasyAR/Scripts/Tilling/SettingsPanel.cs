using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    /// <summary>
    /// set settings panel hide
    /// </summary>
    bool isHiddenSettings;

    /// <summary>
    /// Reference for settings panel 
    /// </summary>
    public GameObject settingsPanel;

    /// <summary>
    /// Reference for settings panel 
    /// </summary>
    public Button settingsButton;

    // Start is called before the first frame update
    void Start()
    {
        isHiddenSettings = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHiddenSettings)
        {
            settingsPanel.SetActive(true);
            settingsButton.interactable = false;
        }
        else
        {
            settingsPanel.SetActive(false);
            settingsButton.interactable = true;
        }
    }

    public void HideSettings()
    {
        isHiddenSettings = !isHiddenSettings;
    }
}
