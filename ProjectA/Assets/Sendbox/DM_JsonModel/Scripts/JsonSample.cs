using System.Collections.Generic;
using UnityEngine;

namespace JsonModel
{
    public class JsonSample : MonoBehaviour
    {
        [SerializeField] private List<TextAsset> textAssets;

        private DM_Network.DM_Network network;

        private async void Awake()
        {
            network = new DM_Network.DM_Network();
            if (!await network.LoginAsync("7789B", "42779113-0012-58F2-939B-0870AFAE582E"))
            {
                Debug.LogWarning($"Failed");
            }

            if (!await network.RequestAsync("ServerTest"))
            {
                Debug.LogWarning($"Failed");
            }

            if (!await network.TitleDataRequestAsync())
            {
                Debug.LogWarning($"Failed");
            }

            if (!await network.UserDataRequestAsync())
            {
                Debug.LogWarning($"Failed");
            }
        }

        public void Start()
        {
            foreach (var textAsset in textAssets)
            {
                Debug.LogWarning(textAsset.name);
                //JsonDataManager.Instance.SetData(textAsset.name, textAsset.text);
            }

            // var cModel = JsonDataManager.Instance.GetData<CharacterModel>("character", "character_1");
            // var mModel = JsonDataManager.Instance.GetData<MissionModel>("mission", "mission_1");
            // var lModel = JsonDataManager.Instance.GetData<LevelModel>("level", "level_2");
            //Debug.LogWarning(lModel.TotalExp);

            // var cModel = JsonDataManager.Instance.GetData<CharacterModel>("item", "weapon_1");
            // var cModel = JsonDataManager.Instance.GetData<CharacterModel>("level", "level_1");
            // var cModel = JsonDataManager.Instance.GetData<CharacterModel>("login_bonus", "day_1");
            // var cModel = JsonDataManager.Instance.GetData<CharacterModel>("skill", "skill_1");
            // var cModel = JsonDataManager.Instance.GetData<CharacterModel>("world", "world_1");
        }
    }
}