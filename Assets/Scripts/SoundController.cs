using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip[] m_Sounds;

    public void FoodConsumeSound()
    {
        m_AudioSource.PlayOneShot(m_Sounds[Random.Range(0, m_Sounds.Length)]);
    }
}
