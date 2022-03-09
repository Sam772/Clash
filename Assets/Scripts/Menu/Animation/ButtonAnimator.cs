using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Animations;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    
    [SerializeField] private Animator animator;
    private AudioSource audioSource;
    private bool disable;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
       animator.SetBool("selected", true);
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        animator.SetBool("selected", false);
    }

	void PlaySound(AudioClip audio) {
		if (!disable) {
			audioSource.PlayOneShot(audio);
		} else {
			disable = false;
		}
	}
}

