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
    // ===== YL added for copy over code approach ============================
    DatabaseReference reference;
    public string section; // need to know how to get from scene
    string json_ds = "--";
    float time_per_qn;
    string section_passed;
    public static List<string> qn_pointers = new List<string>();
    public static List<string> long_qns = new List<string>();
    public string last_long_qn = "never update";
    public string last_pointer = "never update";
    public static List<int> lst_answers = new List<int>();
    public static List<float> times = new List<float>();
    Dictionary<string, dynamic> dict_sec;
    Dictionary<string, dynamic> dict_qn;
    string difficulty = "easy"; 
    // YL added for importing class approach =================================
    FBhelper helper;
    
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
    
    public void helper_sec(){
        helper.getSec(section);
    }

    public void helper_loop(){
        helper.Loop_getQn();
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

    /// ======================= YL added COPY approach ===========================
//     public string[] Get_Qn_Array(){
//         Debug.Log("in get qn array, qn list len = "+long_qns.Count+"  section is: "+section_passed);
//         string[] ar_qn = long_qns.ToArray();
//         // for (int i =0; i<10; i++){
//         // System.Threading.Thread.Sleep(1);
//         // Debug.Log("in get qn array, qn list len = "+long_qns.Count+"  section is: "+section_passed);
//         // getSec(section_passed);
//         // Loop_getQn();
//         // ar_qn = long_qns.ToArray();
//         // Debug.Log("in get qn array, len = "+ar_qn.Length);
//         // if (ar_qn.Length >0) break;}
//         Debug.Log("in get qn array, len = "+ar_qn.Length+"  section is: "+section_passed);

//         return ar_qn;
//     }

//     public void Get_Qn_Array_debug(){
//         questions = long_qns.ToArray();
//         Debug.Log("in get qn array, len = "+questions.Length);
//     }

//     public void Get_Ans_Array(){
//         answers  = lst_answers.ToArray();
//         Debug.Log("in get qn array, len = "+answers.Length);

//     }

//     public void Get_Time_Array(){
//         List<float>times = new List<float>();
//         for(int i =0; i<long_qns.Count; i++){
//             times.Add(time_per_qn);
//         }
//         questionTime = times.ToArray();
//         Debug.Log("in get time array, len = "+questionTime.Length);
        
//     }

//     public void Loop_getQn(){
//         for (int i = 0; i< qn_pointers.Count; i++){
//             // FirebaseDatabase.DefaultInstance.GetReference("Questions").Child(qn_pointers[i]).ValueChanged += Script_ValueChanged_Question;
//             getQn(qn_pointers[i]);}
//         Debug.Log("in loop_getQn(), questions list len = "+long_qns.Count);
//     }


//     public void getSec(string str = "-") /// for debug  old param:     string str = "w5s3"
//     {   section_passed = str;
//         string pointer;
//         // FirebaseDatabase.DefaultInstance.GetReference("Sections").Child(str).ValueChanged += Script_ValueChanged_Section;

//         // DataSnapshot snapshot = FirebaseDatabase.DefaultInstance.GetReference("Sections").Child(str).GetValueAsync().Snapshot;
//         // json_ds = snapshot.GetRawJsonValue();

//         // FirebaseDatabase.DefaultInstance.GetReference("Sections").Child(str).GetValueAsync().ContinueWith(task => {
//         // if (task.IsFaulted) {
//         //   // Handle the error...
//         //   }
//         // else if (task.IsCompleted) {
//         //     DataSnapshot snapshot = task.Result;
//         //     json_ds = snapshot.GetRawJsonValue();
//         //     dict_sec = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_ds);

//         //     Debug.Log("------"+dict_sec[difficulty]["time_per_qn"]);

//         //     foreach(var item in dict_sec[difficulty]["questions"])
//         //         {       pointer = item.ToString();
//         //                 if(IsDigitsOnly(pointer))
//         //             {   Debug.Log("questions ------"+pointer);
//         //                 qn_pointers.Add(pointer);
//         //                 last_pointer = pointer;}
//         //         }
//         //     time_per_qn = Convert.ToSingle(dict_sec[difficulty]["time_per_qn"]);
//         //     Debug.Log(qn_pointers.Count.ToString()+" questions found, time per qn = "+ time_per_qn.ToString());            

//         //     }
//         // });
//         Debug.Log("outside task: "+qn_pointers.Count.ToString()+" questions found, time per qn = "+ time_per_qn.ToString()+"json_ds is: "+ json_ds);            
//     }


//  // ------------------------------- helper functions-------------------------

//     Type GetType<T>(T x) { return typeof(T); }  // possible output "System.Int64"  or "System.String"

//     bool IsDigitsOnly(string str)
//     {
//         foreach (char c in str)
//         {  if (c < '0' || c > '9')
//                 return false;}
//         if(str.Length<1) return false;
//         return true;
//     }

//     string ABCto123(string str)
//     {
//         if(str=="A") return "1";
//         if(str=="B") return "2";
//         if(str=="C") return "3";
//         if(str=="D") return "4";
//         else return str;
//     }

//   /// ------------------------ diff way of get data functions -----------------

//     public void Script_ValueChanged_Section (object sender, ValueChangedEventArgs e)
//     {
//         json_ds = e.Snapshot.GetRawJsonValue();
//         dict_sec = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_ds);

//         Debug.Log("------"+dict_sec[difficulty]["time_per_qn"]);

//         foreach(var item in dict_sec[difficulty]["questions"])
//             {       string pointer = item.ToString();
//                     if(IsDigitsOnly(pointer))
//                 {   Debug.Log("questions ------"+pointer);
//                     qn_pointers.Add(pointer);}
//             }
//         time_per_qn = Convert.ToSingle(dict_sec[difficulty]["time_per_qn"]);
//         Debug.Log(qn_pointers.Count.ToString()+" questions found, time per qn = "+ time_per_qn.ToString());
//     }

//     public void Script_ValueChanged_Question (object sender, ValueChangedEventArgs e)
//     {
//             // on_start.text = "line 166";
//             string str_long_qn = " ";
            
//             DataSnapshot snapshot = e.Snapshot;
            
//             json_ds = snapshot.GetRawJsonValue();
//             dict_qn = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_ds);

//             Debug.Log("------"+dict_qn.Count.ToString());
            
//             foreach(var item in dict_qn)
//                 {   string str = item.ToString();
//                     Debug.Log("inside dict ------"+item.Key+"  ---- "+item.Value.ToString() + GetType(item.Value).ToString());
//                     if(item.Key=="Question"){
//                         str_long_qn = "====  "+item.Value.ToString()+"   Choices : "+str_long_qn;
//                     }
                    
//                     if (item.Key.Length<2){
//                         str_long_qn +=("  " + ABCto123(item.Key) + ": "+ item.Value.ToString());  // prepare for case where options are ABCD
//                     }

//                     if(item.Key=="Answer"){
//                         int result;
//                         if(GetType(item.Value).ToString()=="System.String")
//                         {   string converted = ABCto123(item.Value);
//                             result = Int32.Parse(converted);}  // huh here is int32 -- but get from firebase is 64????
//                         else{
//                         result = Convert.ToInt32(item.Value);
//                         //result = Int32.Parse(item.Value.ToString()); // prepare for long type of number from FB
//                         }
//                         lst_answers.Add(result);
//                         Debug.Log("for answer ------"+item.Key+"  ---- "+item.Value.ToString()+ "  " + GetType(item.Value).ToString()+"   "+ GetType(result).ToString());

//                     }
                        
//                 }
//             long_qns.Add(str_long_qn);
//             Debug.Log(str_long_qn);
//     }
//     public void getQn(string qn_num) // for testing
//     {
//         // FirebaseDatabase.DefaultInstance.GetReference("Questions").Child(input_str).ValueChanged += Script_ValueChanged_Question;

//         FirebaseDatabase.DefaultInstance.GetReference("Questions").Child(qn_num).GetValueAsync().ContinueWith(task => {
//         if (task.IsFaulted) {
//           // Handle the error...
//           }
//         else if (task.IsCompleted) {
//             DataSnapshot snapshot = task.Result;
//             string str_long_qn = " ";            
//             json_ds = snapshot.GetRawJsonValue();
//             dict_qn = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_ds);

//             Debug.Log("------"+dict_qn.Count.ToString());
            
//             foreach(var item in dict_qn)
//                 {   string str = item.ToString();
//                     Debug.Log("inside dict ------"+item.Key+"  ---- "+item.Value.ToString() + GetType(item.Value).ToString());
//                     if(item.Key=="Question"){
//                         str_long_qn = "====  "+item.Value.ToString()+"   Choices : "+str_long_qn;
//                     }
                    
//                     if (item.Key.Length<2){
//                         str_long_qn +=("  " + ABCto123(item.Key) + ": "+ item.Value.ToString());  // prepare for case where options are ABCD
//                     }

//                     if(item.Key=="Answer"){
//                         int result;
//                         if(GetType(item.Value).ToString()=="System.String")
//                         {   string converted = ABCto123(item.Value);
//                             result = Int32.Parse(converted);}  // huh here is int32 -- but get from firebase is 64????
//                         else{
//                         result = Convert.ToInt32(item.Value);
//                         //result = Int32.Parse(item.Value.ToString()); // prepare for long type of number from FB
//                         }
//                         lst_answers.Add(result);
//                         Debug.Log("for answer ------"+item.Key+"  ---- "+item.Value.ToString()+ "  " + GetType(item.Value).ToString()+"   "+ GetType(result).ToString());

//                     }
                        
//                 }
//             long_qns.Add(str_long_qn);
//             Debug.Log(str_long_qn);
//             }
//         });
//     }

}

