using System.Globalization;
using TMPro;
using UnityEngine;

public class fps : MonoBehaviour
{
    public TMP_Text fpsText;
    public float deltaTime;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString(CultureInfo.InvariantCulture);
    }
}