using UnityEngine;
#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
using UnityEditor;
#endif

namespace ArcadeVP
{
    public class InputManager_ArcadeVP : MonoBehaviour
    {
        public ArcadeVehicleController arcadeVehicleController;

        [HideInInspector] public float Horizontal;
        [HideInInspector] public float Vertical;
        [HideInInspector] public float Jump;

        private void Update()
        {
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
            Jump = Input.GetAxis("Jump");

            arcadeVehicleController.ProvideInputs(Horizontal, Vertical, Jump);
        }
    }

#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
    [CustomEditor(typeof(InputManager_ArcadeVP))]
    public class InputManager_ArcadeVP_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            InputManager_ArcadeVP script = (InputManager_ArcadeVP)target;

            if (GUILayout.Button("Upgrade to New Input System"))
            {
                GameObject obj = script.gameObject;
                ArcadeVehicleController avc = script.arcadeVehicleController;

                Undo.RegisterCompleteObjectUndo(obj, "Upgrade Input Manager");

                New_InputManager_ArcadeVP newManager = Undo.AddComponent<New_InputManager_ArcadeVP>(obj);
                newManager.arcadeVehicleController = avc;
                newManager.AddDefaultBindings();

                Undo.DestroyObjectImmediate(script);
            }
        }
    }
#endif
}
