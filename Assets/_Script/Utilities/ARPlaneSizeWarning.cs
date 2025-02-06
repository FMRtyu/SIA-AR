using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ARPlaneSizeWarning
{
    private static CanvasGroup warningText;
    private static bool isShowing = false;

    public static void Initialize(CanvasGroup canvasGroup)
    {
        warningText = canvasGroup;
        warningText.alpha = 0; // Hide initially
    }

    public static void ShowWarning()
    {
        if (isShowing || warningText == null) return;
        isShowing = true;

        LeanTween.alphaCanvas(warningText, 1, 0.2f) // Fade in fast
            .setOnComplete(() =>
            {
                LeanTween.alphaCanvas(warningText, 0, 0.4f) // Fade out slow
                    .setDelay(1.5f)
                    .setOnComplete(() =>
                    {
                        isShowing = false;
                    });
            });
    }

    public static void HideWarning()
    {
        if (warningText == null) return;

        LeanTween.cancel(warningText.gameObject);
        LeanTween.alphaCanvas(warningText, 0, 0.2f); // Hide immediately
        isShowing = false;
    }
}
