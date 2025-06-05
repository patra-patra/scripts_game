using UnityEngine;

public class QuestLocationTrigger : MonoBehaviour
{
    [Header("Location Settings")]
    public string locationName = "Выход из леса";
    public Vector3 targetPosition = new Vector3(50, 0, 100);

    [Header("Trigger Settings")]
    public bool useCollider = true;
    public float triggerRadius = 5f;

    private bool hasTriggered = false;

    void Start()
    {
        if (useCollider)
        {
            // Создаем триггер коллайдер
            SphereCollider trigger = gameObject.GetComponent<SphereCollider>();
            if (trigger == null)
                trigger = gameObject.AddComponent<SphereCollider>();

            trigger.isTrigger = true;
            trigger.radius = triggerRadius;
        }
    }

    void Update()
    {
        if (!useCollider)
        {
            CheckPlayerPosition();
        }
    }

    void CheckPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, targetPosition);
        if (distance <= triggerRadius && !hasTriggered)
        {
            TriggerLocation();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (useCollider && other.CompareTag("Player") && !hasTriggered)
        {
            TriggerLocation();
        }
    }

    void TriggerLocation()
    {
        hasTriggered = true;
        Debug.Log($"Достигли: {locationName}");

        // Отправляем событие достижения локации
        QuestEvents.TriggerLocationReached(targetPosition);
    }

    void OnDrawGizmosSelected()
    {
        // Показываем область триггера
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        // Показываем целевую позицию
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, 1f);
    }
}