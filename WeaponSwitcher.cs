using UnityEngine;
using UnityEngine.UI; // Required for Image component
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Controllers")]
    public Animator animator;
    public RuntimeAnimatorController unarmedController;
    public AnimatorOverrideController pistolOverride;
    public AnimatorOverrideController rifleOverride;

    [Header("Camera Configuration")]
    public CinemachineVirtualCamera followCamera;
    private Cinemachine3rdPersonFollow _thirdPersonFollow;

    [Header("Settings")]
    public float pistolCameraOffset = -1.2f;
    public float rifleCameraOffset = -1.5f;
    public float transitionSpeed = 5f;

    [Header("Weapon Models")]
    public GameObject pistolModel;
    public GameObject rifleModel;

    [Header("UI / Crosshairs")]
    public Image crosshairUI;        // Drag your UI Image here
    public Sprite idleCrosshair;     // Dot or circle
    public Sprite pistolCrosshair;   // Small cross
    public Sprite rifleCrosshair;    // Larger cross or bracket

    private float _targetOffset = 0f;

    void Start()
    {
        if (followCamera != null)
            _thirdPersonFollow = followCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        // Initialize state
        EquipUnarmed();
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) EquipUnarmed();
        if (Keyboard.current.digit2Key.wasPressedThisFrame) EquipPistol();
        if (Keyboard.current.digit3Key.wasPressedThisFrame) EquipRifle();

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
        SetWeaponModels(false, false);
        UpdateCrosshair(idleCrosshair);
        _targetOffset = 0f;
    }

    void EquipPistol()
    {
        animator.runtimeAnimatorController = pistolOverride;
        SetWeaponModels(true, false);
        UpdateCrosshair(pistolCrosshair);
        _targetOffset = pistolCameraOffset;
    }

    void EquipRifle()
    {
        animator.runtimeAnimatorController = rifleOverride;
        SetWeaponModels(false, true);
        UpdateCrosshair(rifleCrosshair);
        _targetOffset = rifleCameraOffset;
    }

    private void SetWeaponModels(bool pistolActive, bool rifleActive)
    {
        if (pistolModel != null) pistolModel.SetActive(pistolActive);
        if (rifleModel != null) rifleModel.SetActive(rifleActive);
    }

    private void UpdateCrosshair(Sprite newSprite)
    {
        if (crosshairUI == null) return;

        if (newSprite == null)
        {
            // If no sprite is provided, hide the crosshair
            crosshairUI.enabled = false;
        }
        else
        {
            crosshairUI.enabled = true;
            crosshairUI.sprite = newSprite;
        }
    }
}