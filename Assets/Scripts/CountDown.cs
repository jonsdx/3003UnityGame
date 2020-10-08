using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    public int second_total;
    public int second;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTime(int time){
        second_total = time;
        second = time;
    }

    public int GetTime(){
        return second;
    }

    public int GetTime_dec1(){
        return second;
        second=second -1;
    }

    public IEnumerator Count(){
        
        while(second!=0)
        {   second = second - 1; 
            yield return new WaitForSeconds(1.0f);}
        }


}
