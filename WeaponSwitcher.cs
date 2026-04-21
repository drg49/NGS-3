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
    [Tooltip("Negative moves player right. Try -1.0 or -1.5 for a strong offset.")]
    public float pistolCameraOffset = -1.0f;
    public GameObject pistolModel;

    void Start()
    {
        if (followCamera != null)
            _thirdPersonFollow = followCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) EquipUnarmed();
        if (Keyboard.current.digit2Key.wasPressedThisFrame) EquipPistol();
    }

    void EquipUnarmed()
    {
        animator.runtimeAnimatorController = unarmedController;
        if (pistolModel != null) pistolModel.SetActive(false);
        if (_thirdPersonFollow != null) _thirdPersonFollow.ShoulderOffset.x = 0f;
    }

    void EquipPistol()
    {
        animator.runtimeAnimatorController = pistolOverride;
        if (pistolModel != null) pistolModel.SetActive(true);

        // Push the player significantly to the right of the screen
        if (_thirdPersonFollow != null)
            _thirdPersonFollow.ShoulderOffset.x = pistolCameraOffset;
    }
}