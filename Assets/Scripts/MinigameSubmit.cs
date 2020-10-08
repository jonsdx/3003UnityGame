using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameSubmit : MonoBehaviour
{
    public Signal contextOn;
    public Signal contextOff;
    public GameObject dialogBox;
    public Text dialogText;
    public string dialog;
    public bool playerInRange;
    public GameObject countdown;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerInRange)
        {
                
                dialogText.text = " Submitting !";
                wait(); // 0.3 s
                countdown.GetComponent<CountDown>().SetTime(-1); // update timer here
                dialogBox.SetActive(false);
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {   
            dialogBox.SetActive(true);
            playerInRange = true;
            dialogText.text = dialog;
            contextOn.Raise();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            dialogBox.SetActive(false);
            contextOff.Raise();
        }
    }

    public IEnumerator wait(){yield return new WaitForSeconds(0.3f);}
        
}
