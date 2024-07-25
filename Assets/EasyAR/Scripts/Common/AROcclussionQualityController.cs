using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

[RequireComponent(typeof(AROcclusionManager))]
public class AROcclussionQualityController : MonoBehaviour
{
    private AROcclusionManager aROcclusionManager;
    public GameObject qualityPanel;
    private Transform bestButton;
    private bool supportDepth = false;
    public GameObject warningPanel;

    private void Awake()
    {
        aROcclusionManager = GetComponent<AROcclusionManager>();
    }

    private void Start()
    {
        qualityPanel.SetActive(false);
        warningPanel.SetActive(false);
        bestButton = qualityPanel.transform.Find("Best");

#if UNITY_IOS
        bestButton.gameObject.GetComponent<Button>().enabled = false;
        #endif

        var occlusionDescriptors = new List<XROcclusionSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors(occlusionDescriptors);

        if (occlusionDescriptors.Count > 0)
        {
            foreach (var occlusionDescriptor in occlusionDescriptors)
            {
                if (occlusionDescriptor.supportsEnvironmentDepthImage || occlusionDescriptor.supportsEnvironmentDepthConfidenceImage
                    || occlusionDescriptor.supportsHumanSegmentationDepthImage || occlusionDescriptor.supportsHumanSegmentationStencilImage)
                {
                    supportDepth = true;
                }
            }
        }
    }

    public void ChangeQualityToMedium()
    {
        aROcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Medium;
        ShowQualityPanel(false);
    }

    public void ChangeQualityToFastest()
    {
        aROcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Fastest;
        ShowQualityPanel(false);
    }

    public void ChangeQualityToBest()
    {
        aROcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Best;
        ShowQualityPanel(false);
    }

    public void ChangeQualityToNoOcclussion()
    {
        aROcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Disabled;
        ShowQualityPanel(false);
    }

    public void ShowQualityPanel(bool show)
    {
        if (supportDepth)
        {
            qualityPanel.SetActive(show);
        }
        else
        {
            //show warning panel for 2 seconds
            warningPanel.SetActive(true);
            StartCoroutine(ShowWarningPanel());
        }
        
    }

    IEnumerator ShowWarningPanel()
    {
        yield return new WaitForSeconds(2);
        warningPanel.SetActive(false);
    }
}

