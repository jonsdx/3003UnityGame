using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WrongAnswer : MonoBehaviour
{
    public string sceneToLoad;
    public Vector2 playerPosition;
    public VectorValue playerStorage;
    public GameObject dialogBox;
    public Text dialogText;
    public GameObject collision;

    public IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {   
            dialogBox.SetActive(true);
            dialogText.text = "Incorrect Answer! You have been kicked out of the hall.";

            GameObject.Find("Student_Default").GetComponent<PlayerMovement>().StopMove();
            yield return new WaitForSeconds(2.5f);

            dialogBox.SetActive(false);
            GameObject.Find("Student_Default").transform.position = playerPosition;
            SceneManager.LoadScene(sceneToLoad);
            GameObject.Find("Student_Default").GetComponent<PlayerMovement>().StartMove();
            collision.SetActive(true);
        }
    }
}
