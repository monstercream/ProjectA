using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

public class AddressablesBuildScript
{
    public static void BuildAddressables()
    {
        AddressableAssetSettings.BuildPlayerContent();
    }
}