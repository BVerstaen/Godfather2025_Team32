using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private SoundEnum _soundToPlay;
    public void PlaySound()
    {
        if(SoundManager.Instance)
            SoundManager.Instance.PlaySound(_soundToPlay);
    }
}
