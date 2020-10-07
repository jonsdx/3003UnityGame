using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Vector2 playerPosition;
    public VectorValue playerStorage;
    public GameObject character;
    private Vector3 scaleChange;

    // Start is called before the first frame update
    void Start()
    {   
        character = GameObject.Find("Student_Default");
        scaleChange = new Vector3(0.4f, 0.4f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame(){
        SceneManager.LoadScene("SampleScene");
        GameObject.FindWithTag("Player").transform.localScale = scaleChange;
        playerStorage.initialValue = playerPosition;
        character.GetComponent<PlayerMovement>().StartMove();
    }

    public void Quit(){
        Application.Quit();
    }
}
