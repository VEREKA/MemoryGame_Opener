using UnityEngine;

public class Carousel : MonoBehaviour
{
    [Header("Ustawienia Karuzeli")]
    public RectTransform[] images;
    public float speed = 150f;
    public float horizontalSpacing = 300f;
    public float verticalOffset = 50f;
    public float leftResetX = -400f;

    private float jumpDistance;

    void Start()
    {

        jumpDistance = images.Length * horizontalSpacing;

        for (int i = 0; i < images.Length; i++)
        {
            float posX = i * horizontalSpacing;
            
            float posY = (i % 2 == 0) ? verticalOffset : -verticalOffset;

            images[i].anchoredPosition = new Vector2(posX, posY);
        }
    }

    void Update()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].anchoredPosition += Vector2.left * speed * Time.deltaTime;

            if (images[i].anchoredPosition.x <= leftResetX)
            {
                Vector2 newPos = images[i].anchoredPosition;
                newPos.x += jumpDistance; 
                images[i].anchoredPosition = newPos;
            }
        }
    }
}