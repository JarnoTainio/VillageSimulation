using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour
{
    float duration;
    float time;
    SpriteRenderer sr;

    private void Start()
    {
        duration = Dummy.instance.iconLifeTime * .9f;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (time < duration)
        {
            float d = UnityEngine.Time.deltaTime; ;
            time += d;
            float step = d / duration;
            transform.localPosition += new Vector3(0, step, 0);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1.25f - (time / duration)));
        }
    }
}
