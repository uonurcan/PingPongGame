using UnityEngine;

public class DestroyKickEffect : MonoBehaviour
{
    /**
      * This script will execute when the kick effect is instantiated.
      * First playing the hit sound.
      * Second, destroying kick effect object when its animation is completed. 
      */

    [SerializeField] private AudioClip hitSound;

    private void Start()
    {
        // Play hit sound
        AudioSource _audioSource = this.GetComponent<AudioSource>();
        _audioSource.PlayOneShot(hitSound);
        // ....
    }

    public void DestroyEffect()
    {
        Destroy(this.gameObject);
    }
}
