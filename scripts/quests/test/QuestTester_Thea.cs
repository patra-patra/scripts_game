using UnityEngine;

public class QuestTester_TheaJourney : MonoBehaviour
{
    [Header("Test Keys")]
    [SerializeField] private KeyCode collectSuppliesKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode talkMerchantKey = KeyCode.Alpha2;
    [SerializeField] private KeyCode talkOldWomanKey = KeyCode.Alpha3;
    [SerializeField] private KeyCode reachForestExitKey = KeyCode.Alpha4;
    [SerializeField] private KeyCode killWolfKey = KeyCode.Alpha5;
    [SerializeField] private KeyCode reachLibraryKey = KeyCode.Alpha6;

    private void Update()
    {
        if (Input.GetKeyDown(collectSuppliesKey))
        {
            QuestEvents.TriggerItemCollected("travel_supplies");
            Debug.Log("Собран предмет для путешествия");
        }

        if (Input.GetKeyDown(talkMerchantKey))
        {
            QuestEvents.TriggerNPCTalk("merchant", "directions");
            QuestManager.Instance.GetComponent<QuestObjectiveManager>()
                .UpdateCustomObjective("path_to_library", "talk_to_locals", 1);
            Debug.Log("Поговорили с торговцем");
        }

        if (Input.GetKeyDown(talkOldWomanKey))
        {
            QuestEvents.TriggerNPCTalk("old_woman", "warning");
            QuestManager.Instance.GetComponent<QuestObjectiveManager>()
                .UpdateCustomObjective("path_to_library", "talk_to_locals", 1);
            Debug.Log("Поговорили с пожилой женщиной");
        }

        if (Input.GetKeyDown(reachForestExitKey))
        {
            QuestEvents.TriggerLocationReached(new Vector3(50, 0, 100));
            Debug.Log("Достигли выхода из леса");
        }

        if (Input.GetKeyDown(killWolfKey))
        {
            QuestEvents.TriggerEnemyKilled("forest_wolf");
            Debug.Log("Убили лесного волка");
        }

        if (Input.GetKeyDown(reachLibraryKey))
        {
            QuestEvents.TriggerLocationReached(new Vector3(200, 0, 200));
            Debug.Log("Достигли библиотеки-башни");
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 200));
        GUILayout.Label("=== Квест: Путь к новой работе ===");
        GUILayout.Label($"1 - Собрать припасы ({collectSuppliesKey})");
        GUILayout.Label($"2 - Поговорить с торговцем ({talkMerchantKey})");
        GUILayout.Label($"3 - Поговорить с пожилой женщиной ({talkOldWomanKey})");
        GUILayout.Label($"4 - Достичь выхода из леса ({reachForestExitKey})");
        GUILayout.Label($"5 - Убить волка ({killWolfKey})");
        GUILayout.Label($"6 - Достичь библиотеки ({reachLibraryKey})");
        GUILayout.EndArea();
    }
}