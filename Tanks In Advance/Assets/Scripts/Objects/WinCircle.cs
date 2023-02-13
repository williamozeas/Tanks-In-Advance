using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCircle : MonoBehaviour
{
    public int numTanksP1 = 0;
    public int numTanksP2 = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.parent == null) return;
        
        if(other.transform.parent.GetComponent<Tank>()){  //If tank component exists (ie != NULL)
            if(other.transform.parent.GetComponent<Tank>().owner == PlayerNum.Player1){
                numTanksP1++;
            }else{
                numTanksP2++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.parent == null) return;
        
        if(other.transform.parent.GetComponent<Tank>()){  //If tank component exists (ie != NULL)
            if(other.transform.parent.GetComponent<Tank>().owner == PlayerNum.Player1){
                numTanksP1--;
            }else{
                numTanksP2--;
            }
        }
    }
}
