using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Controllers")]
    public Animator animator;
    public RuntimeAnimatorController unarmedController;
    public AnimatorOverrideController pistolOverride;

    [Header("Camera Configuration")]
    public CinemachineVirtualCamera followCamera;
    private Cinemachine3rdPersonFollow _thirdPersonFollow;

    [Header("Settings")]
    public float pistolCameraOffset = -1.2f;
    public float transitionSpeed = 5f; // Higher = faster camera movement
    public GameObject pistolModel;

    private float _targetOffset = 0f;

    void Start()
    {
        if (followCamera != null)
            _thirdPersonFollow = followCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    void Update()
    {
        // 1. Handle Input
        if (Keyboard.current.digit1Key.wasPressedThisFrame) EquipUnarmed();
        if (Keyboard.current.digit2Key.wasPressedThisFrame) EquipPistol();

        // 2. Smoothly move the camera offset every frame
        if (_thirdPersonFollow != null)
        {
            _thirdPersonFollow.ShoulderOffset.x = Mathf.Lerp(
                _thirdPersonFollow.ShoulderOffset.x,
                _targetOffset,
                Time.deltaTime * transitionSpeed
            );
        }
    }

    void EquipUnarmed()
    {
        animator.runtimeAnimatorController = unarmedController;
        if (pistolModel != null) pistolModel.SetActive(false);
        _targetOffset = 0f; // Set the goal to center
    }

    void EquipPistol()
    {
        animator.runtimeAnimatorController = pistolOverride;
        if (pistolModel != null) pistolModel.SetActive(true);
        _targetOffset = pistolCameraOffset; // Set the goal to the side
    }
}