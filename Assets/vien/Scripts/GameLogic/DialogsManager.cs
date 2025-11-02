using System;
using GLTFast.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Collections;
using NUnit.Framework;

public class DialogsManager : MonoBehaviour
{
    private TextMeshProUGUI textDisplay;
    private RawImage speakerImage;
    private bool isSpeaking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textDisplay = GetComponentInChildren<TextMeshProUGUI>();
        speakerImage = GetComponentInChildren<RawImage>();
        speakerImage.enabled = false;
        textDisplay.enabled = false;
        InitialDialog();

    }

    // Update is called once per frame
    void Update()
    {

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

    public void AltarInteraction(bool hasEnoughItems)
    {
        if (hasEnoughItems)
        {
            StartCoroutine(ShowMoreLines(new string[] { "Nice, It's working, I suggest to keep some distance, just in case." }, 0f, 3f, 1f));
        }
        else
        {
            StartCoroutine(ShowMoreLines(new string[] { "You need to collect both runes to proceed." }, 0f, 3f, 1f));
        }
    }
    

    
    IEnumerator ShowMoreLines(string[] lines, float initialDelay, float displayDuration, float betweenDelay)
    {
        isSpeaking = true;
        if (speakerImage != null) speakerImage.enabled = true;
        if (textDisplay == null) yield break;

        yield return new WaitForSeconds(initialDelay);

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

}
