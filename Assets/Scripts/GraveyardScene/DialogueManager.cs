using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject fadeIn;

    [SerializeField] GameObject muncProfile;
    [SerializeField] GameObject gkProfile;
    private GameObject profileImage;

    [SerializeField] TextAsset inkJSON;
    [SerializeField] GameObject textBox;
    [SerializeField] GameObject dialogueText;
    [SerializeField] GameObject nameText;
    [SerializeField] string textToSpeak;
    [SerializeField] int currentTextLength;
    [SerializeField] int textLength;

    [SerializeField] GameObject nextButton;

    [SerializeField] AudioSource muncSound;
    [SerializeField] AudioSource gkSound;
    private AudioSource audioSource;

    [SerializeField] private string dialogueKnotName;
    private Story story;
    private bool dialoguePlaying = false;

    private void Update()
    {
        textLength = TextCreator.charCount;
    }

    void Awake()
    {
        profileImage = muncProfile;
        audioSource = muncSound;
        story = new Story(inkJSON.text);
        EnterDialogue(dialogueKnotName);
        
    }

    private void Start()
    {
        StartCoroutine(StartDialogue());
    }

    public void OnNextButton()
    {
        textBox.SetActive(false);
        ContinueStory();
    }

    private void EnterDialogue(string knotName)
    {
        if (dialoguePlaying)
        {
            return;
        }

        dialoguePlaying = true;
        textBox.SetActive(true);
        profileImage.SetActive(true);

        // Bind external Ink functions:
        story.BindExternalFunction("swapProfile", (string who) => {
            DoSwapProfile(who);
        });

        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("Knot name was empty when entering dialogue");
        }
    }

    private void ExitDialogue()
    {
        story.UnbindExternalFunction("swapProfile");
        Debug.Log("Exiting dialogue");
        dialoguePlaying = false;
        textBox.SetActive(false);
        profileImage.SetActive(false);
    }

    void ContinueStory()
    {
        nextButton.SetActive(false);
        if (!story.canContinue)
        {
            ExitDialogue();
        }
        else
        {
            string line = story.Continue();
            Debug.Log("Ink says: " + line);

            // Typewriter effect
            StartCoroutine(DoTextWithDelay(line));
        }
    }

    // Switches profile and audio sound based on the speaker
    void DoSwapProfile(string who)
    {
        // Hide both
        profileImage.SetActive(false);

        // Show only the one we want
        if (who == "Gravekeeper")
        {
            profileImage = gkProfile;
            audioSource = gkSound;
        }
        else
        {
            profileImage = muncProfile;
            audioSource = muncSound;
        }
        profileImage.SetActive(true);

        // Update the name label
        nameText.GetComponent<TMPro.TMP_Text>().text = who;
    }

    IEnumerator StartDialogue ()
    {
        yield return new WaitForSeconds(2);
        fadeIn.SetActive(false);
        ContinueStory();
    }

    IEnumerator DoTextWithDelay (string line)
    {
        textBox.SetActive(true);
        nextButton.SetActive(false);
        dialogueText.GetComponent<TMPro.TMP_Text>().text = line;
        currentTextLength = line.Length;
        TextCreator.runTextPrint = true;
        audioSource.Play();
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLength == currentTextLength);
        yield return new WaitForSeconds(0.5f);
        nextButton.SetActive(true);
    }
}