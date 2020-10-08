using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System.Linq;


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
    // ===== YL added for FBHelper ============================
    DatabaseReference reference;
    public string section; // need to know how to get from scene
    string difficulty = "easy"; 
    string id = "John";
    FBhelper helper;
    // YL added for count down and scoring =================================
    public GameObject countdown;
    public List<int> scores = new List<int>();
    string time_stamp = DateTime.Now.ToString();
    
    // Start is called before the first frame update
    void Start()
    {   helper = gameObject.AddComponent(typeof(FBhelper)) as FBhelper;

        // reference = FirebaseDatabase.DefaultInstance.RootReference;
        // // Set up the Editor before calling into the realtime database.
        // FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unity-firebase-test1-5fbe2.firebaseio.com/");

        // getSec(section);
        // Loop_getQn();
        // System.Threading.Thread.Sleep(3);
        // Get_Qn_Array();
        // Get_Ans_Array();
        // Get_Time_Array();

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
        helper.getSec(section);
        helper.getUser(id);
    }

    void Awake(){
    section = SceneManager.GetActiveScene().name;
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

        // Get_Qn_Array();
        // Get_Ans_Array();
        // Get_Time_Array();

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
            dialogBox.SetActive(true);
            dialogText.text = questions[i];

            // yl implememt counter ======
            int k=(int)questionTime[i];
            countdown.GetComponent<CountDown>().SetTime(k);
            int time_score = 0; // last
            while(countdown.GetComponent<CountDown>().GetTime()>=0)
            {   int now = countdown.GetComponent<CountDown>().GetTime();
                Debug.Log(" === Counter time is: "+now);
                timeLeft.text = now.ToString();
                time_score = now;
                countdown.GetComponent<CountDown>().SetTime(now-1);
                yield return new WaitForSeconds(1.0f);
            }
            scores.Add(time_score);

            // for (int k=(int)questionTime[i];k>0;k--)    /// this int k needs to be set to 0 by other objects / scripts. cannot doint like that
            // {   
            //     countdown.GetComponent<CountDown>().SetTime(k);
            //     Debug.Log(" === Counter time is: "+countdown.GetComponent<CountDown>().GetTime());
            //     timeLeft.text = k.ToString();
            //     yield return new WaitForSeconds(1.0f);
            // }

            CheckAnswer();
            i++;
        }
        if (i == questions.Length)
        {   dialogBox.SetActive(true);
            dialogText.text = "End of Quiz! Congratulations for finishing, young programmer.";
            for (int j=inbetweenTime;j>0;j--)
            {
                timeLeft.text = j.ToString();
                yield return new WaitForSeconds(1.0f);
            }
            ResetHall();
            timer.SetActive(false);
            dialogBox.SetActive(true);
            // helper.Save_progress(section);
            check_write_progress();
            dialogText.text = "Score = "+scores.Sum().ToString()+"  Quiz is over, Get out of here through that pink-ish purple hole!";
            helper.Save_score(scores.Sum());
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
            // helper_sec();
            // helper_loop();
            helper_qn_arr();
            helper_qn_arr_return();
            helper_ans_arr_return();
            helper_time_arr_return();

            Debug.Log(" !!!!!!!! ======= after calling all helpr fn, len of question = "+questions.Length);

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

    /// ===================YL import approach ==========================
    
    public int get_score(){
        if (scores.Count==1) return 0;
        return scores.Take(scores.Count-1).Sum();
    }
    
    public void helper_sec(){
        helper.getSec(section);
    }

    public void helper_loop(){
        helper.Loop_getQn();
    }

    void check_write_progress(){
        string user_progress = helper.Get_progress();
        int user_w = Int32.Parse(user_progress.Substring(1, 1));
        int user_s = Int32.Parse(user_progress.Substring(3, 1));
        int w = Int32.Parse(section.Substring(1, 1));
        int s = Int32.Parse(section.Substring(3, 1)); 
        if((user_w == w) && (user_s == s-1)) helper.Save_progress(section);
        else if((user_w == w-1) && (user_s == 3)) helper.Save_progress(section); // assume each world has 3 section
    }

    public void helper_qn_arr(){
        helper.Get_Qn_Array_debug();
    }

    public void helper_qn_arr_len(){
        int len = 999;
        len = helper.Get_Qn_Array_len();
        Debug.Log(len.ToString());
    }

    public void helper_qn_arr_return(){
        questions = helper.Get_Qn_Array();
        Debug.Log("the len of arr returned = "+questions.Length.ToString());
        string joined = string.Join("-", questions);
        Debug.Log(joined);
    }
    public void helper_ans_arr_return(){
        answers = helper.Get_Ans_Array();
    }

    public void helper_time_arr_return(){
        questionTime = helper.Get_Time_Array();
    }


}

