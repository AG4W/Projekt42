using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager getInstance { get; private set; }

    [SerializeField]AudioSource music;
    [SerializeField]AudioSource oneshots;

    AudioSource[] sources;

    //se till att vi alltid har en,
    //och att den överlever scene loads
    void Awake()
    {
        if (getInstance != null)
            Destroy(this.gameObject);
        else
        {
            getInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {
        sources = new AudioSource[] { music, oneshots };
    }

    //wrappers
    public static void Play(SFXType type, AudioClip sound, float pitch = 1f)
    {
        getInstance.PlayInternal(type, sound, pitch);
    }
    public static void Stop(SFXType type)
    {
        getInstance.StopInternal(type);
    }

    public void PlayInternal(SFXType type, AudioClip sound, float pitch = 1f)
    {
        sources[(int)type].pitch = pitch;
        sources[(int)type].clip = sound;
        sources[(int)type].Play();
    }
    public void StopInternal(SFXType type)
    {
        sources[(int)type].Stop();
    }
}
public enum SFXType
{
    Music,
    Oneshot,
}
