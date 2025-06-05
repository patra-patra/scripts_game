using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionRadius = 10f; // ������ ������ ������������� ��������

    public GameObject interactionUI; // UI ������� ��� ��������� ��������������
    public TextMeshPro interactionText; // ����� ��������� ��������������

    void Update()
    {
        InteractionRay();
    }

    void InteractionRay()
    {
        // ������� ������ ��� ������ ����
        Vector3 rayOrigin = transform.position;
        
        // ����������� ���� - ����� ������������ ����������� ������ ��� ������
        Vector3 rayDirection = playerCamera.transform.forward;
        
        // ������� ��� �� ������� ������ � ����������� ������� ������
        Ray ray = new Ray(rayOrigin, rayDirection);
        
        RaycastHit hit;
        bool hitSomething = false;

        if (Physics.Raycast(ray, out hit, interactionRadius))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                hitSomething = true;
                interactionText.text = interactable.GetDescription();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
        }
        interactionUI.SetActive(hitSomething);
    }
}

