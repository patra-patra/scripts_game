using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class DoorOpened : MonoBehaviour, IInteractable
{
    public Animator doorAnimator;
    public bool isOpen;

    void Start()
    {
        

        if (isOpen)
        {
            doorAnimator.SetBool("isOpen", true);
        }
    }

    public string GetDescription()
    {
        if (isOpen) return "Закрыть дверь [E]";
        return "Открыть дверь [Е]";
    }

    public void Interact()
    {
        isOpen = !isOpen;
        
        // Проверка на null перед использованием
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("isOpen", true);
        }
        else
        {
            doorAnimator.SetBool("isOpen", false);
        }
    }
}
