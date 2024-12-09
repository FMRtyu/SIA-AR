using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonDelay : MonoBehaviour
{
    public static void EnableButtonAfterDelay(Button button, float delay, MonoBehaviour caller)
    {
        caller.StartCoroutine(EnableButtonAfterDelayCoroutine(button, delay));
    }

    private static IEnumerator EnableButtonAfterDelayCoroutine(Button button, float delay)
    {
        yield return new WaitForSeconds(delay);
        button.interactable = true;
    }

    public static void ApplyDelayToAllButtons(float delay, MonoBehaviour caller)
    {
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button button in allButtons)
        {
            Button currentButton = button;
            currentButton.onClick.AddListener(() => {
                currentButton.interactable = false;
                EnableButtonAfterDelay(currentButton, delay, caller);
            });
        }
    }
}
