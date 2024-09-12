using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ButtonDelay
{
    /// <summary>
    /// start coroutine to delay button
    /// </summary>
    /// <param name="button">button to delay</param>
    /// <returns></returns>
    public static IEnumerator EnabledBTNAfterSecond(Button button)
    {
        if (button.interactable == true)
        {
            button.interactable = false;
            yield return new WaitForSeconds(0.5f);
            button.interactable = true;
        }
    }
}
