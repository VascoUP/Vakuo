using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    private EventManager _events;
    private AudioSource _source;
    public AudioClip enemiesDyingSound;
    public AudioClip enemiesCollisionSound;
    public AudioClip playerGroundedSound;
    public AudioClip playerPushedSound;

	// Use this for initialization
	private void Start () {
        _events = Utils.GetComponentOnGameObject<EventManager>("Game Manager");
        _source = GetComponent<AudioSource>();
    }

    private void PlayerGrounded()
    {
        PlaySound(playerGroundedSound);
    }

    private void Attack()
    {
        PlaySound(enemiesCollisionSound);
    }

    private void EnemyDeath()
    {
        PlaySound(enemiesDyingSound);
    }

    private void PlayerPushed(GameObject obj1, GameObject obj2)
    {
        AudioSource audioSource = obj2.AddComponent<AudioSource>();
        audioSource.clip = playerPushedSound;
        audioSource.Play();
    }

    private void PlaySound(AudioClip audio)
    {
        _source.clip = audio;
        _source.Play();
    }
    
}
