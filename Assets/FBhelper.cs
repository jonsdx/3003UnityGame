using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;


public class FBhelper : MonoBehaviour
{
    DatabaseReference reference;
    string input_str;
    string temp_out = "-";
    string json_ds="--";
    float time_per_qn;
    float[] ar_time;
    string section_passed;
    List<string> qn_pointers = new List<string>();
    List<string> long_qns = new List<string>();
    string[] ar_qn;
    List<int> answers = new List<int>();
    int[] ar_ans;
    Dictionary<string, dynamic> dict_sec;
    Dictionary<string, dynamic> dict_qn;
    string difficulty = "easy"; // later need to allow user choose ??? how ??
    

    // Start is called before the first frame update
    void Start()
    {   
        // on_start.text = "welcome!";
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unity-firebase-test1-5fbe2.firebaseio.com/");
        // on_start.text = "line 26";
        // Get the root reference location of the database. 
        // on_start.text = "line 29";

    }

    // Update is called once per frame
    void Update()
    {
        // upload scores !!!!!!
    }

    public void Debug_line(){
        Debug.Log("---------- this is FirebaseQn object-----------------------");
    }


    public void get_status(){
        Debug.Log("============= current object status ============== "+ System.Environment.NewLine +
        " len of List qn_pointers = "+ qn_pointers.Count + System.Environment.NewLine + 
        " len of List long_qns = "+ long_qns.Count + System.Environment.NewLine + 
        " len of List answers = "+ answers.Count + System.Environment.NewLine + 
        " len of Array ar_qn = "+ ar_qn.Length + System.Environment.NewLine
        // " len of Array ar_time = "+ ar_time.Length + System.Environment.NewLine + 
        // " len of Array ar_ans = "+ ar_ans.Length + System.Environment.NewLine
        );

    }


    public string[] Get_Qn_Array(){
        ar_qn = long_qns.ToArray();
        Debug.Log("in get qn array, qn list len = "+long_qns.Count+"  section is: "+section_passed);
        return ar_qn;
    }

    public void Get_Qn_Array_debug(){
        ar_qn = long_qns.ToArray();
        Debug.Log("in get qn array, len = "+ar_qn.Length);
    }

    public int Get_Qn_Array_len(){
        ar_qn = long_qns.ToArray();
        Debug.Log("in get qn array, len = "+ar_qn.Length);
        return ar_qn.Length;
    }

    public int[] Get_Ans_Array(){
        ar_ans  = answers.ToArray();
        Debug.Log("in get qn array, len = "+ar_ans.Length);
        return ar_ans;
    }

    public float[] Get_Time_Array(){
        List<float>times = new List<float>();
        for(int i =0; i<long_qns.Count; i++){
            times.Add(time_per_qn);
        }
        ar_time = times.ToArray();
        Debug.Log("in get time array, len = "+ar_time.Length);
        return ar_time;
    }

    public void Loop_getQn(){
        for (int i = 0; i< qn_pointers.Count; i++){
                FirebaseDatabase.DefaultInstance.GetReference("Questions").Child(qn_pointers[i]).ValueChanged += Script_ValueChanged_Question;
        }
    }

    public void getQn() // for testing
    {
        // on_start.text = "line 42";
        input_str = qn.text.ToString(); // is the pointer to qn
        FirebaseDatabase.DefaultInstance.GetReference("Questions").Child(input_str).ValueChanged += Script_ValueChanged_Question;

        // FirebaseDatabase.DefaultInstance.GetReference("Questions").Child(input_str).GetValueAsync().ContinueWith(task => {
        // if (task.IsFaulted) {
        //   // Handle the error...
        //   }
        // else if (task.IsCompleted) {
        //     DataSnapshot snapshot = task.Result;
            
        //     json_ds = snapshot.GetRawJsonValue();
        //     dict_qn = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_ds);

        //     Debug.Log("------"+dict_qn.Count.ToString());
            
        //     foreach(var item in dict_qn)
        //         {       string str = item.ToString();
        //                 Debug.Log("inside dict ------"+item.Key+"  ---- "+item.Value.ToString());
        //                 if(item.Key=="Question"){
        //                     str_long_qn = "====  "+item.Value.ToString()+"   Choices : "+str_long_qn;
        //                 }
                        
        //                 if (item.Key.Length<2){
        //                     str_long_qn +=("  " + ABCto123(item.Key) + ": "+ item.Value.ToString());  // prepare for case where options are ABCD
        //                 }
        //                 long_qns.Add(str_long_qn);
        //         }
        //     Debug.Log(str_long_qn);
        //     LoadedText.text = json_ds;
        //     }
        // LoadedText.text = "out sied else "+json_ds;    
        // });
    }

    public void getSec(string str = "-") /// for debug  old param:     string str = "w5s3"
    {
        // on_start.text = "line 50";
        // input_str = section.text.ToString();
        // if(str != "-") input_str = str;
        section_passed = str;
        // if (input_str.Length>1) str = input_str;
        FirebaseDatabase.DefaultInstance.GetReference("Sections").Child(str).ValueChanged += Script_ValueChanged_Section;
        Debug.Log("==== in helper ==== after getSec, "+qn_pointers.Count.ToString()+" questions found, time per qn = "+ time_per_qn.ToString());
        return;
    }


// ------------------------------- helper functions-------------------------

    Type GetType<T>(T x) { return typeof(T); }  // possible output "System.Int64"  or "System.String"

    bool IsDigitsOnly(string str)
    {
        foreach (char c in str)
        {  if (c < '0' || c > '9')
                return false;}
        if(str.Length<1) return false;
        return true;
    }

    string ABCto123(string str)
    {
        if(str=="A") return "1";
        if(str=="B") return "2";
        if(str=="C") return "3";
        if(str=="D") return "4";
        else return str;
    }

/// ------------------------ diff way of get data functions -----------------

    public void Script_ValueChanged_Section (object sender, ValueChangedEventArgs e)
    {
        temp_out = "";
        string str_ds;
        string json_ds;
        json_ds = e.Snapshot.GetRawJsonValue();
        dict_sec = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_ds);
        List<string> ls_ppointer_in_func = new List<string>();
        Debug.Log("------"+dict_sec[difficulty]["time_per_qn"]);

        foreach(var item in dict_sec[difficulty]["questions"])
            {       string pointer = item.ToString();
                    if(IsDigitsOnly(pointer))
                {   Debug.Log("questions ------"+pointer);
                    qn_pointers.Add(pointer);
                    // LoadedText.text = pointer;
                    ls_ppointer_in_func.Add(pointer);    
                    Debug.Log("in loop ---- "+pointer + "  len of list gloabl = "+qn_pointers.Count+"  len of list local = "+ls_ppointer_in_func.Count);
                    }
            }
        time_per_qn = Convert.ToSingle(dict_sec[difficulty]["time_per_qn"]);
        Debug.Log("outside loop ----  len of list gloabl = "+qn_pointers.Count+"  len of list local = "+ls_ppointer_in_func.Count);

        Debug.Log(qn_pointers.Count.ToString()+" questions found, time per qn = "+ time_per_qn.ToString());
        
        Debug.Log(" LOOP within SEC running ");
        Loop_getQn();
        Debug.Log(" LOOP within SEC run finish ");
        return;
    }

    public void Script_ValueChanged_Question (object sender, ValueChangedEventArgs e)
    {
            // on_start.text = "line 166";
            // input_str = qn.text.ToString();
            string str_long_qn = " ";
            
            DataSnapshot snapshot = e.Snapshot;
            
            json_ds = snapshot.GetRawJsonValue();
            dict_qn = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_ds);

            Debug.Log("------"+dict_qn.Count.ToString());
            
            foreach(var item in dict_qn)
                {   string str = item.ToString();
                    Debug.Log("inside dict ------"+item.Key+"  ---- "+item.Value.ToString() + GetType(item.Value).ToString());
                    if(item.Key=="Question"){
                        str_long_qn = "====  "+item.Value.ToString()+"   Choices : "+str_long_qn;
                    }
                    
                    if (item.Key.Length<2){
                        str_long_qn +=("  " + ABCto123(item.Key) + ": "+ item.Value.ToString());  // prepare for case where options are ABCD
                    }

                    if(item.Key=="Answer"){
                        int result;
                        if(GetType(item.Value).ToString()=="System.String")
                        {   string converted = ABCto123(item.Value);
                            result = Int32.Parse(converted);}  // huh here is int32 -- but get from firebase is 64????
                        else{
                        result = Convert.ToInt32(item.Value);
                        //result = Int32.Parse(item.Value.ToString()); // prepare for long type of number from FB
                        }
                        answers.Add(result);
                        Debug.Log("for answer ------"+item.Key+"  ---- "+item.Value.ToString()+ "  " + GetType(item.Value).ToString()+"   "+ GetType(result).ToString());

                    }
                        
                }
            long_qns.Add(str_long_qn);
            Debug.Log(str_long_qn);
        //     LoadedText.text = json_ds;
            
        // LoadedText.text = "out sied else "+json_ds;    
        
        // LoadedText.text = "in side script value changed , long_qns list len = "+long_qns.Count.ToString();
    }

}

        
