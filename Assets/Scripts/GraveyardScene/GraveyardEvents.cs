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
        // event 0

        yield return new WaitForSeconds(2);
        fadeIn.SetActive(false);
        profileMunc.SetActive(true);
        
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

        
    }

    IEnumerator EventOne()
    {
        nextButton.SetActive(false);
        textBox.SetActive(true);
        yield return new WaitForSeconds(2);
        profileGravekeeper.SetActive(true);
        yield return new WaitForSeconds(2);
        gravekeeperSound.Play();
    }

    public void NextButton()
    {
        if (eventPos == 1)
        {
            StartCoroutine(EventOne());
        }
    }
}
