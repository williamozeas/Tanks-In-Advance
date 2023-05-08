using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;

    public MapName selectedMap;
    public int roundCnt;

    public static DataManager Instance()
    {
        return instance;
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}