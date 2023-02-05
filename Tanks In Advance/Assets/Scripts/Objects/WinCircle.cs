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
        Debug.Log("In");
        Debug.Log(other.transform.parent.GetComponent<Tank>());
        if(other.transform.parent.GetComponent<Tank>()){
            if(other.transform.parent.GetComponent<Tank>().owner == PlayerNum.Player1){
                numTanksP1++;
                Debug.Log("P1=" + numTanksP1);
            }else{
                numTanksP2++;
                Debug.Log("P2=" + numTanksP2);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Out");
        Debug.Log(other.transform.parent.GetComponent<Tank>());
        if(other.transform.parent.GetComponent<Tank>()){
            if(other.transform.parent.GetComponent<Tank>().owner == PlayerNum.Player1){
                numTanksP1--;
                Debug.Log("P1=" + numTanksP1);
            }else{
                numTanksP2--;
                Debug.Log("P2=" + numTanksP2);
            }
        }
    }
}
