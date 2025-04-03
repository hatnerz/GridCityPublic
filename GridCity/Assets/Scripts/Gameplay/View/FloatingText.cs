using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private double duration = 2f;
    [SerializeField] private double speed = 50f;
    [SerializeField] private Color initializeColor;
    [SerializeField] private float timer;

    public void SetText(string text)
    {
        this.textComponent.text = text;
    }

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        initializeColor = textComponent.color;
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer <= duration)
        {
            transform.Translate(Vector2.up * (float)speed * Time.deltaTime);
            float alpha = Mathf.Lerp(initializeColor.a, 0, (float)(timer / duration));

            textComponent.color = new Color(initializeColor.r, initializeColor.g, initializeColor.b, alpha);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}