using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using System;

public class InputScript : MonoBehaviour
{
    DatabaseReference reference;
    public InputField Email;
    public InputField Name;
    public Text LoadedText;
    public Text on_start;
    string inputname;
    string [] questions;
    FBhelper helper;
    bool helper_got_sec = false;

    // Start is called before the first frame update
    void Start()
    {
        on_start.text = "welcome!";
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unity-firebase-test1-5fbe2.firebaseio.com/");
        on_start.text = "line 26";
        // Get the root reference location of the database. 
        on_start.text = "line 29";
        helper = gameObject.AddComponent(typeof(FBhelper)) as FBhelper;
        helper_sec();
        helper_got_sec = true;
        Debug.Log("try to run helper loop START");
        while (helper_got_sec != true){
            System.Threading.Thread.Sleep(1);
            continue;
        }
        if (helper_got_sec = true) {
            helper_loop();
            Debug.Log("try to run helper loop finish");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // upload scores !!!!!!
    }
    
    public void helper_status(){
        helper.get_status();
    }

    public void helper_sec(){
        helper.getSec("w5s3");
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
        LoadedText.text = len.ToString();
        if (len == 999) LoadedText.text = "shit len in here never get update";
    }

    public void helper_qn_arr_return(){
        questions = helper.Get_Qn_Array();
        Debug.Log("the len of arr returned = "+questions.Length.ToString());
        string joined = string.Join("-", questions);
        LoadedText.text = joined;
    }

    public void SaveData()
    {
        on_start.text = "line 42";
        inputname = Name.text.ToString();
        reference.Child("Users").Child(inputname).Child("Email").SetValueAsync(Email.text.ToString());
        reference.Child("Users").Child(inputname).Child("Name").SetValueAsync(inputname);
    }

    public void LoadData()
    {
        on_start.text = "line 50";
        inputname = Name.text.ToString();
        reference.Child("Temp").Child("time_last_run_Script").SetValueAsync(DateTime.Now.ToString());
        FirebaseDatabase.DefaultInstance.GetReference("Users").ValueChanged += Script_ValueChanged;
    }


    private void Script_ValueChanged (object sender, ValueChangedEventArgs e)
    {
        LoadedText.text = e.Snapshot.Child(inputname).Child("Email").GetValue(true).ToString();
    }
}
