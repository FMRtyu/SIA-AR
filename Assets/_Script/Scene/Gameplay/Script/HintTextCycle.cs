using SIAairportSecurity.Training;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintTextCycle : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public GameObject debugText;
    public bool testHint;
    public string[] messages;
    private int index = 0;
    private Coroutine textChangeCoroutine;
    public float hintTimeCycle;

    private void Start()
    {
        if (testHint)
        {
            hintTimeCycle = 3;
            debugText.SetActive(true);
            Debug.Log("set as debug");
            StopTextCycle();
            StartTextCycle();
        }
        else
        {
            debugText.SetActive(false);
        }

        GamePlayController gamePlayController = FindAnyObjectByType<GamePlayController>();
        if (gamePlayController != null)
        {
            gamePlayController.onStateChange += OnGameStateChange;
        }
    }

    private void OnGameStateChange(GameState newGameState)
    {
        if (newGameState == GameState.Scanning)
        {
            StartTextCycle();
        }
        else
        {
            StopTextCycle();
        }
    }

    private void OnEnable()
    {
        StartTextCycle();
    }

    private void OnDisable()
    {
        StopTextCycle();
    }

    private void StartTextCycle()
    {
        index = 0;
        if (messages.Length > 0 && textComponent != null && textChangeCoroutine == null)
        {
            textComponent.text = messages[index];
            textChangeCoroutine = StartCoroutine(ChangeTextRoutine());
        }
    }

    private void StopTextCycle()
    {
        if (textChangeCoroutine != null)
        {
            StopCoroutine(textChangeCoroutine);
            textChangeCoroutine = null;
        }
    }

    private IEnumerator ChangeTextRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(hintTimeCycle);
            index = (index + 1) % messages.Length;
            textComponent.text = messages[index];
        }
    }
}
