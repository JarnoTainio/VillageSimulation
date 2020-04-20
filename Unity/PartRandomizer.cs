using UnityEngine;

public class PartRandomizer : MonoBehaviour
{
    public Sprite[] sprites;
    public Color[] colors;

    // Start is called before the first frame update
    public void Randomize(System.Random r)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprites[(int)r.Next(sprites.Length)];
        sr.color = colors[(int)r.Next(colors.Length)];
    }
}
