using UnityEngine;

public class RandomPitchAudio : MonoBehaviour
{
    public float MinValue;
    public float MaxValue;
    public AudioSource AudioSource;

    public void Play()
    {
        AudioSource.pitch = Random.Range(MinValue, MaxValue);
        AudioSource.Play();
    }
}
