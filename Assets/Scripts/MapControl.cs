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

public class MapControl : MonoBehaviour
{
    FBhelper helper;
    public GameObject door_w1s2;
    string user_progress = "w0s0";
    string id = "John"; // later see how to change it
    bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {   
        door_w1s2.SetActive(false);
        helper = gameObject.AddComponent(typeof(FBhelper)) as FBhelper;
        helper.getUser(id);
        // user_progress = helper.Get_progress();
    }

    void Awake(){
    }

    // Update is called once per frame
    void Update()
    {   
        if(playerInRange)
        {   user_progress = helper.Get_progress();          /// scare this is resource consuming
            door_w1s2.SetActive(compare_progress("w1s2"));}
    }
    
    public void helper_save_progress(){
        helper.Save_progress("some string");
    }    
    public void helper_get_user(){
        helper.getUser();
    }    
    public void helper_get_progress(){
        user_progress = helper.Get_progress();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log(" === collider detected player =====");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }



    private bool compare_progress(string to_compare)
    {
        int user_w = Int32.Parse(user_progress.Substring(1, 1));
        int user_s = Int32.Parse(user_progress.Substring(3, 1));

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

}
