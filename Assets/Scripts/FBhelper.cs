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

    // ==== below is for get user ======
    string id = "John"; // later get from default student
    string progress = "w0s0"; // world x section x that he has cleared , here is w1s0 means at the start
    Dictionary<string, dynamic> dict_user;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    int attempt_count;

=======
    public bool returnCatch = false;
>>>>>>> Stashed changes
=======
    public bool returnCatch = false;
>>>>>>> Stashed changes
=======
    public bool returnCatch = false;
>>>>>>> Stashed changes
    // Start is called before the first frame update
    
    void Start()
    {   

        reference = FirebaseDatabase.DefaultInstance.RootReference;
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unity-firebase-test1-5fbe2.firebaseio.com/");
        // Get the root reference location of the database. 

    }

    // Update is called once per frame

    void Update()
    {
        // upload scores !!!!!!
    }

    public void Debug_line(){
        Debug.Log("---------- this is FirebaseQn object-----------------------");
    }

    public void Set_difficulty(string str){
        difficulty = str; // easy medium hard
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

// ======================== For main menu simple auth ==============================================
    public bool authChecker(InputField Username, InputField Password){
        //simple authentication
        string username = Username.text;
        string password = Password.text;

        FirebaseDatabase.DefaultInstance.GetReference("Users").GetValueAsync().ContinueWith(task => {
            
            if (task.IsFaulted) {
            // Handle the error...
            }   
            else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result.Child(username);
            // Do something with snapshot...
                json_ds = snapshot.GetRawJsonValue();
                dict_sec = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_ds);
                foreach(KeyValuePair<string, dynamic> entry in dict_sec){
                    Debug.Log(entry.Value);
                    if(entry.Key == "Password"){
                        returnCatch = !returnCatch;
                        //return returnCatch;
                    }
                }
            }
        });

        Debug.Log(returnCatch);
        return returnCatch;
        
    }
    
// ======================== For persistentCharData updates ==========================================

    public void updateCharacterData(){
        //modify persistent data
        
    }

// ======================== Functions to return data for game =======================================

    public string Get_progress(){
        Debug.Log("progress is "+ progress);
        return progress;
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

///===============functions to Write to FireBase ====================================================

    public void Save_progress(string prog, string uid = "-"){
        if (uid == "-") uid = id;
        else id = uid;
        reference.Child("Users").Child(uid).Child("progress_str").SetValueAsync(prog);
    }

    public void Save_score(int score, string time = "-",string uid = "-"){
        if (uid == "-") uid = id;
        else id = uid;
        string now = DateTime.Now.ToString();
        if (time == "-") time = now;
        reference.Child("Users").Child(uid).Child("History").Child(section_passed).Child((attempt_count).ToString()).Child("Score").SetValueAsync(score);
        reference.Child("Users").Child(uid).Child("History").Child(section_passed).Child((attempt_count).ToString()).Child("Time").SetValueAsync(time);
    }


///========== functions to READ from firebase and update class attributes ============================

    public void Loop_getQn(){
        for (int i = 0; i< qn_pointers.Count; i++){
                FirebaseDatabase.DefaultInstance.GetReference("Questions").Child(qn_pointers[i]).ValueChanged += Script_ValueChanged_Question;
        }
    }

    public void getQn() // for testing
    {
        FirebaseDatabase.DefaultInstance.GetReference("Questions").Child(input_str).ValueChanged += Script_ValueChanged_Question;
    }

    public void getSec(string str = "-") /// for debug  old param:     string str = "w5s3"
    {
        section_passed = str;
        FirebaseDatabase.DefaultInstance.GetReference("Sections").Child(str).ValueChanged += Script_ValueChanged_Section;
        Debug.Log("==== in helper ==== after getSec, "+qn_pointers.Count.ToString()+" questions found, time per qn = "+ time_per_qn.ToString());
        return;
    }

    public void getUser(string uid = "-") // to grab the entire user
    {   if (uid == "-") uid = id;
        else id = uid;
        FirebaseDatabase.DefaultInstance.GetReference("Users").Child(uid).ValueChanged += Script_ValueChanged_User;
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

    bool compare_progress(string to_compare) // self bigger is true, self smaller is false
    {
        int user_w = Int32.Parse(progress.Substring(1, 1));
        int user_s = Int32.Parse(progress.Substring(3, 1));

        int w = Int32.Parse(to_compare.Substring(1, 1));
        int s = Int32.Parse(to_compare.Substring(3, 1)); 

        int user_ws = user_w*10+user_s;
        int ws = w*10+s;

        if (user_w > w) return true;

        else if(user_w == w){
            if(user_s < s-1) return false;
            else return true;
        } 
        
        else if(user_w < w){
            if((user_w == w-1) && ((user_s==3) && (s==1)) ) return true;  // assuming 1 world 3 sections, so if he finished w1s3 can go w2s1;
            return false;
        }
        return false;
    }

/// ------------------------ THE CHOSEN way of get data functions -----------------

    // public void Script_ValueChanged_History (object sender, ValueChangedEventArgs e)
    // {
    //     json_ds = e.Snapshot.GetRawJsonValue();
    //     dict_user = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_ds);
    //     progress = dict_user["progress_str"];
    //     Debug.Log("user file copied from firebase");
    //     return;
    // }
    
    public void Script_ValueChanged_User (object sender, ValueChangedEventArgs e)
    {
        json_ds = e.Snapshot.GetRawJsonValue();
        dict_user = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_ds);
        progress = dict_user["progress_str"];
        attempt_count = dict_user["History"][section_passed].Count;   /// if never attemp will get error
        Debug.Log("user file copied from firebase, attempt count is "+attempt_count);
        return;
    }
    
    public void Script_ValueChanged_Section (object sender, ValueChangedEventArgs e)
    {
        temp_out = "";
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
    }

}

        