using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image checkmarkImage;
    [SerializeField] private Slider progressSlider;

    [Header("Colors")]
    [SerializeField] private Color incompleteColor = Color.white;
    [SerializeField] private Color completedColor = Color.green;

    public void Setup(QuestObjective objective)
    {
        if (descriptionText != null)
            descriptionText.text = objective.Description;

        if (progressText != null)
            progressText.text = objective.GetProgressText();

        if (checkmarkImage != null)
        {
            checkmarkImage.gameObject.SetActive(objective.IsCompleted);
            checkmarkImage.color = objective.IsCompleted ? completedColor : incompleteColor;
        }

        if (progressSlider != null)
        {
            progressSlider.value = objective.GetProgressPercentage();
            progressSlider.gameObject.SetActive(!objective.IsCompleted && objective.RequiredProgress > 1);
        }

        // Изменяем цвет текста в зависимости от статуса
        if (descriptionText != null)
            descriptionText.color = objective.IsCompleted ? completedColor : incompleteColor;
    }
}