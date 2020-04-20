using UnityEngine;

public class Toggle : MonoBehaviour
{

    public GameObject go;
    public GameObject[] objects;
    
    public void ToggleObject()
    {
        bool isActive = go.activeSelf;
        foreach(GameObject g in objects)
        {
            g.SetActive(false);
        }
        go.SetActive(!isActive);
    }
}
