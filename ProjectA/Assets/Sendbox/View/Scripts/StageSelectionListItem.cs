using System;
using JsonModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionListItem : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button button;

    public void Display(StageModel stageModel, Action<StageModel> onClicked)
    {
        gameObject.SetActive(true);
        image.sprite = stageModel.ImagePath;
        nameText.text = stageModel.Name;
        button.onClick.AddListener(() => onClicked(stageModel));
    }
}
