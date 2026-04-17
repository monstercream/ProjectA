using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArcadeVP
{
    public class New_InputManager_ArcadeVP : MonoBehaviour
    {
        public ArcadeVehicleController arcadeVehicleController;

#if ENABLE_INPUT_SYSTEM
        [Header("Input Actions")]
        public InputAction steeringAction;
        public InputAction accelerationAction;
        public InputAction brakeAction;
#endif

        [Header("Input Smoothing")]
        public bool useSmoothing = true;
        public float steeringLerpSpeed = 10f;
        public float accelerationLerpSpeed = 10f;
        public float brakeLerpSpeed = 15f;

        private float currentSteering;
        private float currentAcceleration;
        private float currentBrake;

        private void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            steeringAction?.Enable();
            accelerationAction?.Enable();
            brakeAction?.Enable();
#endif
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            steeringAction?.Disable();
            accelerationAction?.Disable();
            brakeAction?.Disable();
#endif
        }

        private void Update()
        {
            float targetSteering = 0f;
            float targetAcceleration = 0f;
            float targetBrake = 0f;

#if ENABLE_INPUT_SYSTEM
            if (steeringAction != null)
                targetSteering = steeringAction.ReadValue<float>();

            if (accelerationAction != null)
                targetAcceleration = accelerationAction.ReadValue<float>();

            if (brakeAction != null)
                targetBrake = brakeAction.ReadValue<float>();
#else
            targetSteering = Input.GetAxis("Horizontal");
            targetAcceleration = Input.GetAxis("Vertical");
            targetBrake = Input.GetAxis("Jump");
#endif

            if (useSmoothing)
            {
                currentSteering = Mathf.Lerp(currentSteering, targetSteering, Time.deltaTime * steeringLerpSpeed);
                currentAcceleration = Mathf.Lerp(currentAcceleration, targetAcceleration, Time.deltaTime * accelerationLerpSpeed);
                currentBrake = Mathf.Lerp(currentBrake, targetBrake, Time.deltaTime * brakeLerpSpeed);
            }
            else
            {
                currentSteering = targetSteering;
                currentAcceleration = targetAcceleration;
                currentBrake = targetBrake;
            }

            if (arcadeVehicleController != null)
            {
                arcadeVehicleController.ProvideInputs(currentSteering, currentAcceleration, currentBrake);
            }
        }

#if ENABLE_INPUT_SYSTEM
        public void AddDefaultBindings()
        {
            steeringAction = new InputAction("Steering", InputActionType.Value);
            steeringAction.expectedControlType = "Axis";
            steeringAction.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/a")
                .With("Positive", "<Keyboard>/d")
                .With("Negative", "<Keyboard>/leftArrow")
                .With("Positive", "<Keyboard>/rightArrow")
                .With("Negative", "<Gamepad>/leftStick/left")
                .With("Positive", "<Gamepad>/leftStick/right");

            accelerationAction = new InputAction("Acceleration", InputActionType.Value);
            accelerationAction.expectedControlType = "Axis";
            accelerationAction.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/s")
                .With("Positive", "<Keyboard>/w")
                .With("Negative", "<Keyboard>/downArrow")
                .With("Positive", "<Keyboard>/upArrow")
                .With("Negative", "<Gamepad>/leftStick/down")
                .With("Positive", "<Gamepad>/leftStick/up");

            brakeAction = new InputAction("Brake", InputActionType.Button);
            brakeAction.AddBinding("<Keyboard>/space")
                .WithGroup("Keyboard");
            brakeAction.AddBinding("<Gamepad>/buttonSouth")
                .WithGroup("Gamepad");
        }
#endif
    }

#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
    [CustomEditor(typeof(New_InputManager_ArcadeVP))]
    public class New_InputManager_ArcadeVP_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            New_InputManager_ArcadeVP script = (New_InputManager_ArcadeVP)target;

            if (GUILayout.Button("Add Default Bindings"))
            {
                script.AddDefaultBindings();
                EditorUtility.SetDirty(script);
            }
        }
    }
#endif
}
