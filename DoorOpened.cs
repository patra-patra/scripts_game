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
        if (isOpen) return "������� ����� [E]";
        return "������� ����� [�]";
    }

    public void Interact()
    {
        isOpen = !isOpen;
        
        // �������� �� null ����� ��������������
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
