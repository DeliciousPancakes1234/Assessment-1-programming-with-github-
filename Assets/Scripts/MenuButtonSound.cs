using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonSound : MonoBehaviour
{

    public AudioSource soundPlayer;
    public AudioClip hoverSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MenuSoundEffect()
    {
        soundPlayer.Play();
    }
    
    public void HoverSound()
    {
        soundPlayer.PlayOneShot(hoverSound);
    }
}
