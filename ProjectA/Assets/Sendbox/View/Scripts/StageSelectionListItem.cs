using System;
using System.Threading.Tasks;
using JsonModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionListItem : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button button;

    public async Task Display(IAddressableManager addressableManager, StageModel stageModel, Action<StageModel> onClicked)
    {
        image.sprite = await addressableManager.LoadAssetAsync<Sprite>(stageModel.ImagePath);
        nameText.text = stageModel.Name;
        button.onClick.AddListener(() => onClicked(stageModel));
        gameObject.SetActive(true);
    }
}
