using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimatedText : MonoBehaviour
{
    public Text animatedText;
    public int minFontSize = 100;
    public int maxFontSize = 120;
    public float animationSpeed = 1.0f;

    void Start()
    {
        if (animatedText == null)
        {
            animatedText = GetComponent<Text>();
        }
        StartCoroutine(AnimateFontSize());
    }

    IEnumerator AnimateFontSize()
    {
        while (true)
        {
            float pingPongValue = Mathf.PingPong(Time.time * animationSpeed, 1);
            animatedText.fontSize = (int)Mathf.Lerp(minFontSize, maxFontSize, pingPongValue);
            yield return null;
        }
    }
}
