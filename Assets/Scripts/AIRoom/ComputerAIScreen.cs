using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComputerAIScreen : MonoBehaviour {

    public static ComputerAIScreen instance;

    public UnityEvent OnLoadingCompleted = new UnityEvent();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        ChessAIGameManager.instance.OnPreAIGameSetupDone.AddListener(BeginDisplayingOnComputerScreen);
    }

    private void BeginDisplayingOnComputerScreen() {
        StartCoroutine("BootingAndLoadingUp");
    }

    IEnumerator BootingAndLoadingUp() {

        // Power on
        transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 0; i <= 118; i++)
        {
            transform.GetChild(0).GetComponent<Image>().color = new Color(6/255.0f, 70/255.0f, 12/255.0f, i/255.0f);
            yield return null;
        }

        string[] logTexts = {"", "Instantiating the Chess Engine",
            "Instantiating the Chess Engine",
            "Instantiating the Chess Engine",
            "Configuring the engine parameters",
            "Checking for the board drivers",
            "Checking for the board drivers",
            "Establishing the communication channels",
            "Establishing the communication channels",
            "Establishing the communication channels",
            "Doing some final checks" };

        // Booting up engine text blinking and displaying log messages
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        for (int numberOfLoadingIterations = 0; numberOfLoadingIterations <= 10; numberOfLoadingIterations++)
        {
            transform.GetChild(2).gameObject.GetComponent<Text>().text = logTexts[numberOfLoadingIterations];
            for (int i = 0; i <= 255; i += 12)
            {
                transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, i / 255.0f);
                yield return null;
            }

            for (int i = 255; i >= 0; i -= 12)
            {
                transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, i / 255.0f);
                yield return null;
            }
        }

        //changing the loading text and making it visible\
        transform.GetChild(1).gameObject.GetComponent<Text>().color = Color.white;
        transform.GetChild(1).gameObject.GetComponent<Text>().text = "Game has started!";

        // game started part for a small duration
        transform.GetChild(2).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.7f);

        transform.GetChild(1).gameObject.SetActive(false);
       
        // invoking this event after the loading is completed for the electronics and other things to respond
        OnLoadingCompleted.Invoke();
    }
}
