using UnityEngine;

public class ScenePersistent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { 
        DontDestroyOnLoad(this);
    }
}
