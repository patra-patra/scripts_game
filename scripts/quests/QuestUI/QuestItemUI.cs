using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button selectButton;
    [SerializeField] private Slider progressSlider;

    [Header("Colors")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color completedColor = Color.green;

    private Quest quest;
    private Action<Quest> onSelectCallback;

    public void Setup(Quest questToDisplay, bool isActive, Action<Quest> selectCallback)
    {
        quest = questToDisplay;
        onSelectCallback = selectCallback;

        if (titleText != null)
            titleText.text = quest.Title;

        if (statusText != null)
            statusText.text = GetStatusText(quest.Status);

        if (progressSlider != null)
        {
            progressSlider.value = quest.GetProgress();
            progressSlider.gameObject.SetActive(isActive);
        }

        if (backgroundImage != null)
            backgroundImage.color = GetStatusColor(quest.Status);

        if (selectButton != null)
            selectButton.onClick.AddListener(() => onSelectCallback?.Invoke(quest));
    }

    private string GetStatusText(QuestStatus status)
    {
        switch (status)
        {
            case QuestStatus.Available:
                return "Available";
            case QuestStatus.Active:
                return "Active";
            case QuestStatus.Completed:
                return "Completed";
            case QuestStatus.Failed:
                return "Failed";
            case QuestStatus.Locked:
                return "Locked";
            default:
                return "";
        }
    }

    private Color GetStatusColor(QuestStatus status)
    {
        switch (status)
        {
            case QuestStatus.Available:
            case QuestStatus.Locked:
                return availableColor;
            case QuestStatus.Active:
                return activeColor;
            case QuestStatus.Completed:
                return completedColor;
            default:
                return availableColor;
        }
    }
}