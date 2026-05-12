using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IAddressableManager : ISystem
{
    // 단일 에셋 로드
    public Task<T> LoadAssetAsync<T>(string key) where T : class;

    // 라벨로 여러 에셋 로드
    public Task<IList<T>> LoadAssetsAsync<T>(string label) where T : class;

    // 씬 로드
    public Task LoadSceneAsync(string key, bool additive = false);

    // 프리팹 인스턴스화
    public Task<GameObject> InstantiateAsync(string key, Transform parent = null);
    public Task<GameObject> InstantiateAsync(string key, Vector3 position, Quaternion rotation, Transform parent = null);

    // 인스턴스 해제 (Destroy + Release 동시 처리)
    public Task ReleaseInstanceAsync(GameObject instance);

    // 전체 해제
    public void ReleaseAll();
}