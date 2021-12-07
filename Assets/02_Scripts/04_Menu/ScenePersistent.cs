using System.Collections;
using System.Collections.Generic;
using DA.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersistent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { 
        DontDestroyOnLoad(this);
    }
}
