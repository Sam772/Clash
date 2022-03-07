using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler {
    
    [SerializeField] private Animator animator;

    public void OnPointerEnter(PointerEventData eventData) {
       animator.SetBool("selected", true);
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        animator.SetBool("selected", false);
        //animator.SetBool("pressed", false);
    }
    public void OnSelect(BaseEventData eventData) {
        //animator.SetBool("pressed", true);
    }
}

