using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    public static void PlayTapSound(AudioSource buttonSFX, AudioClip tapSound)
    {
        if (buttonSFX.isPlaying)
        {
            buttonSFX.Stop();
        }

        buttonSFX.clip = tapSound;
        buttonSFX.Play();
    }

    public static void ApplySFXToAllButtons(AudioSource buttonSFX, AudioClip tapSound)
    {
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button button in allButtons)
        {
            Button currentButton = button;
            currentButton.onClick.AddListener(() => {
                PlayTapSound(buttonSFX, tapSound);
            });
        }
    }
}
