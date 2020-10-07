using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowQuestion : MonoBehaviour
{
    public bool playerInRange;
    public GameObject portal;
    public GameObject dialogBox;
    public Text dialogText;
    public GameObject timer;
    public Text timeLeft;
    public GameObject [] options;
    public string [] questions;
    public int [] answers;
    public float [] questionTime;
    public bool quizStart;
    private int i;
    private int inbetweenTime;

    
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        quizStart = false;
        Time.timeScale = 1.0f;
        inbetweenTime = 5;
        timer.SetActive(false);
        ResetHall();
        portal.SetActive(false);
        portal.GetComponent<Collider2D>().enabled = false;
        if (!GetComponent<Collider2D>().isTrigger)
        {
            GetComponent<Collider2D>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            dialogBox.SetActive(true);
        } else {
            dialogBox.SetActive(false);
        }
    }

    private IEnumerator StartQuiz(){
        GetComponent<Collider2D>().enabled = true;
        quizStart = true;
        timer.SetActive(true);
        while (i<questions.Length)
        {
            dialogText.text = "Processing...";
            for (int j=inbetweenTime;j>inbetweenTime-2;j--)
            {
                timeLeft.text = j.ToString();
                yield return new WaitForSeconds(1.0f);
            }
            dialogText.text = "Get ready for next question!";
            for (int j=inbetweenTime-2;j>0;j--)
            {
                timeLeft.text = j.ToString();
                yield return new WaitForSeconds(1.0f);
            }
            ResetHall();
            dialogText.text = questions[i];
            for (int k=(int)questionTime[i];k>0;k--)
            {
                timeLeft.text = k.ToString();
                yield return new WaitForSeconds(1.0f);
            }
            CheckAnswer();
            i++;
        }
        if (i == questions.Length)
        {
            dialogText.text = "End of Quiz!";
            for (int j=inbetweenTime;j>0;j--)
            {
                timeLeft.text = j.ToString();
                yield return new WaitForSeconds(1.0f);
            }
            ResetHall();
            timer.SetActive(false);
            dialogText.text = "Quiz is over, Get out of here!";
            portal.SetActive(true);
            portal.GetComponent<Collider2D>().enabled = true;
        }
    }

    private void CheckAnswer()
    {
        for (int j=0;j<options.Length;j++)
        {
            if (j+1 == answers[i])
            {
                options[j].GetComponent<Collider2D>().enabled = true;
                options[j].SetActive(true);
            } 
        }
    }

    private void ResetHall(){
        for (int j=0;j<options.Length;j++)
        {
            options[j].SetActive(false);
            options[j].GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (!quizStart)
            {
                StartCoroutine(StartQuiz());
            } 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            dialogBox.SetActive(false);
        }
    }
}
