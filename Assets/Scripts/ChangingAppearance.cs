using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingAppearance : MonoBehaviour
{
    public SpriteRenderer part;
    public Sprite[] options;
    public AnimatorOverrideController[] Anims;
    public int partIndex;

    void Update(){
        part.sprite = options[partIndex];
        GetComponent<Animator>().runtimeAnimatorController = Anims[partIndex] as RuntimeAnimatorController;
    }

    public void swap(){
        if (partIndex < options.Length - 1){
            partIndex++;
        } else {
            partIndex = 0;
        }
    }
}
