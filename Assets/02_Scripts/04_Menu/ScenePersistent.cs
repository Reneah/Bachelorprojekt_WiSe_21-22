using UnityEngine;

public class ScenePersistent : MonoBehaviour
{
    [SerializeField] private GameObject scenePersistentContent;
    
    // Start is called before the first frame update
    void Start()
    {
        if(!GameObject.Find("scenePersistentContent"))
        {
            GameObject scenePersistentContentInstance = Instantiate(scenePersistentContent);
            scenePersistentContentInstance.name = "scenePersistentContent";
            DontDestroyOnLoad(scenePersistentContentInstance); 
        }
    }
}
