using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public Vector2 playerPosition;
    public VectorValue playerStorage;
    public GameObject character;
    private Vector3 scaleChange;
    private InputField Username, Password;
    FBhelper fb;
    
    // Start is called before the first frame update
    void Start()
    {   
        fb = gameObject.AddComponent(typeof(FBhelper)) as FBhelper;
        character = GameObject.Find("Student_Default");
        scaleChange = new Vector3(0.4f, 0.4f, 1f);
        Username = GameObject.Find("Username").GetComponent<InputField>();
        Password = GameObject.Find("Password").GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame(){
        //start of simple auth
        if(fb.authChecker(Username, Password)){
            Debug.Log(Username.text + Password.text + " Sign-in successful!");
            SceneManager.LoadScene("SampleScene");
            character.transform.localScale = scaleChange;
            character.transform.position = playerPosition;
            character.GetComponent<PlayerMovement>().StartMove();

        }else{
            Debug.Log(Username.text + Password.text + " Failure to sign in.");
            //SceneManager.LoadScene("SampleScene");
            character.transform.localScale = scaleChange;
            character.transform.position = playerPosition;
            character.GetComponent<PlayerMovement>().StartMove();
        }

    }

    public void Quit(){
        Application.Quit();
    }
}
