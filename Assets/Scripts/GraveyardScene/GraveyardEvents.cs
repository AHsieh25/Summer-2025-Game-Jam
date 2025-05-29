using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardEvents : MonoBehaviour
{
    public GameObject fadeIn;
    public GameObject profileMunc;
    public GameObject profileGravekeeper;
    public GameObject textBox;
    [SerializeField] AudioSource muncSound;
    [SerializeField] AudioSource gravekeeperSound;
    [SerializeField] string textToSpeak;
    [SerializeField] int currentTextLength;
    [SerializeField] int textLength;
    [SerializeField] GameObject mainTextObject;
    [SerializeField] GameObject nextButton;
    [SerializeField] int eventPos = 0;
    [SerializeField] GameObject charName;

    void Update()
    {
        textLength = TextCreator.charCount;
    }

    void Start()
    {
        StartCoroutine(EventStarter());
    }

    IEnumerator EventStarter()
    {
        // Event 0

        yield return new WaitForSeconds(2);
        fadeIn.SetActive(false);
        profileMunc.SetActive(true);
        yield return new WaitForSeconds(1);
        mainTextObject.SetActive(true);
        textToSpeak = "...Where am I?";
        textBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        currentTextLength = textToSpeak.Length;
        TextCreator.runTextPrint = true;
        muncSound.Play();
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLength == currentTextLength);
        yield return new WaitForSeconds(0.5f);
        nextButton.SetActive(true);
        eventPos = 1;
    }

    IEnumerator EventOne()
    {
        // Event 1
        nextButton.SetActive(false);
        profileMunc.SetActive(false);
        profileGravekeeper.SetActive(true);
        yield return new WaitForSeconds(1);
        textBox.SetActive(true);
        charName.GetComponent<TMPro.TMP_Text>().text = "Gravekeeper";
        textToSpeak = "Good evening. It seems you have risen from the beyond.";
        textBox.GetComponent<TMPro.TMP_Text>().text = textToSpeak;
        currentTextLength = textToSpeak.Length;
        TextCreator.runTextPrint = true;
        gravekeeperSound.Play();
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => textLength == currentTextLength);
        yield return new WaitForSeconds(0.5f);
        nextButton.SetActive(true);
    }

    public void NextButton()
    {
        if (eventPos == 1)
        {
            StartCoroutine(EventOne());
        }
    }
}
