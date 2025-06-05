using System.Collections.Generic;

public interface IQuestRepository
{
    List<Quest> GetAllQuests();
    Quest GetQuestById(string questId);
    void SaveQuestProgress(Quest quest);
    void SaveAllQuestProgress(List<Quest> quests);
    List<Quest> GetAvailableQuests();
    List<Quest> GetActiveQuests();
    List<Quest> GetCompletedQuests();
    void LoadQuestData();
    void ResetAllQuests();
    bool QuestExists(string questId);
}