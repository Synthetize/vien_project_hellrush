using System;
using GLTFast.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;


public class DialogsManager : MonoBehaviour
{
    private TextMeshProUGUI textDisplay;
    private RawImage speakerImage;
    private bool isSpeaking = false;
    readonly List<Action> _actionQueue = new List<Action>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textDisplay = GetComponentInChildren<TextMeshProUGUI>();
        speakerImage = GetComponentInChildren<RawImage>();
        speakerImage.enabled = false;
        textDisplay.enabled = false;
        EnqueueAction(InitialDialog);
    }

    public void EnqueueAction(Action action)
    {
        if (action == null) return;
        _actionQueue.Add(action);
    }

    
    void Update()
    {
        // esegue la prossima azione se non stiamo già mostrando dialoghi
        if (!isSpeaking && _actionQueue.Count > 0)
        {
            var next = _actionQueue[0];
            _actionQueue.RemoveAt(0);
            try { next?.Invoke(); }
            catch (Exception e) { Debug.LogException(e); }
        }
    }





    IEnumerator ShowMoreLines(string[] lines, float initialDelay, float displayDuration, float betweenDelay)
    {
        isSpeaking = true;
        if (textDisplay == null) yield break;
        yield return new WaitForSeconds(initialDelay);
        if (speakerImage != null) speakerImage.enabled = true;
        textDisplay.enabled = true;

        foreach (string line in lines)
        {
            textDisplay.text = line;
            yield return new WaitForSeconds(displayDuration);
            yield return new WaitForSeconds(betweenDelay);
        }

        textDisplay.enabled = false;
        if (speakerImage != null) speakerImage.enabled = false;
        isSpeaking = false;
    }
    


    public void InitialDialog()
    {
        string text = "That's the altar, let's go to check it out.";
        StartCoroutine(ShowMoreLines(new string[] { text }, 3f, 3f, 0f));
    }

    public void AltarDialog()
    {
        string lineOne = "As expected they almost completed the ritual, they are just waiting for the demon king to obtain his full power before summoning him.";
        string lineTwo = "We have to hurry, if they complete the ritual the demon king before it gains its full power.";
        string lineThree = "We need to go into the portals to get the remaining rune to start the ritual.";
        StartCoroutine(ShowMoreLines(new string[] { lineOne, lineTwo, lineThree }, 0f, 3f, 1f));
    }

    public void AltarInteractionTrue()
    {
        StartCoroutine(ShowMoreLines(new string[] { "Great, the ritual has started, now we need to defend the altar from the incoming waves of demons." }, 0f, 3f, 1f));
    }

    public void AltarInteractionFalse()
    {
        StartCoroutine(ShowMoreLines(new string[] { "Nice, It's working, Keep some distance, just in case." }, 0f, 3f, 1f));
    }

    public void FirstWaveCompleted()
    {
        string lineOne = "Good job, go back and go through the next portal to get the other rune.";
        StartCoroutine(ShowMoreLines(new string[] { lineOne }, 0f, 3f, 1f));
    }

    public void SecondWaveCompleted()
    {
        string lineOne = "Well done, now return to the altar to summon the demon king";
        StartCoroutine(ShowMoreLines(new string[] { lineOne }, 0f, 3f, 1f));
    }

    public void BossDefeated()
    {
        string lineOne = "You did it! The demon king has been defeated!";
        string lineTwo = "With his defeat, the dark ritual has been thwarted, and the world is safe once again.";
        StartCoroutine(ShowMoreLines(new string[] { lineOne, lineTwo }, 0f, 3f, 1f));
    }

}
