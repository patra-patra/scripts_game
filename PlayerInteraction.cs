using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionRadius = 10f; // Радиус поиска интерактивных объектов

    public GameObject interactionUI; // UI элемент для подсказки взаимодействия
    public TextMeshPro interactionText; // Текст подсказки взаимодействия

    void Update()
    {
        InteractionRay();
    }

    void InteractionRay()
    {
        // Позиция игрока как начало луча
        Vector3 rayOrigin = transform.position;
        
        // Направление луча - можно использовать направление камеры или игрока
        Vector3 rayDirection = playerCamera.transform.forward;
        
        // Создаем луч из позиции игрока в направлении взгляда камеры
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

