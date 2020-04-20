using UnityEngine;

public class CopyColor : MonoBehaviour
{
    public SpriteRenderer other;
    
    void Start()
    {
        if (other != null)
        {
            GetComponent<SpriteRenderer>().color = other.color;
        }
    }
}
