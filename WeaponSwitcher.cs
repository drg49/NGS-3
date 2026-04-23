using UnityEngine;
using UnityEngine.UI;
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
    private Transform _mainCamTransform;

    [Header("Movement & Aim Settings")]
    [Tooltip("How fast the player rotates to face the crosshair")]
    public float rotationSpeed = 15f;
    public LayerMask aimLayerMask;
    public float transitionSpeed = 5f;

    [Header("Weapon Specific Offsets")]
    public float pistolCameraOffset = -1.2f;
    public float rifleCameraOffset = -1.5f;

    [Header("Weapon Models")]
    public GameObject pistolModel;
    public GameObject rifleModel;

    [Header("UI / Crosshairs")]
    public Image crosshairUI;
    public Sprite idleCrosshair;
    public Sprite pistolCrosshair;
    public Sprite rifleCrosshair;

    private float _targetOffset = 0f;
    private bool _isUnarmed = true;

    void Start()
    {
        // Cache references
        if (followCamera != null)
            _thirdPersonFollow = followCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        _mainCamTransform = Camera.main.transform;

        // Start in Unarmed state
        EquipUnarmed();
    }

    void Update()
    {
        HandleInput();
        HandleCameraLerp();
        HandleAimRotation();
    }

    private void HandleInput()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) EquipUnarmed();
        if (Keyboard.current.digit2Key.wasPressedThisFrame) EquipPistol();
        if (Keyboard.current.digit3Key.wasPressedThisFrame) EquipRifle();
    }

    private void HandleCameraLerp()
    {
        if (_thirdPersonFollow != null)
        {
            _thirdPersonFollow.ShoulderOffset.x = Mathf.Lerp(
                _thirdPersonFollow.ShoulderOffset.x,
                _targetOffset,
                Time.deltaTime * transitionSpeed
            );
        }
    }

    private void HandleAimRotation()
    {
        // Don't force rotation if we aren't holding a weapon
        if (_isUnarmed) return;

        // 1. Raycast from the center of the screen into the world
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimLayerMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            // If we hit nothing (the sky), pick a point far away
            targetPoint = ray.GetPoint(999f);
        }

        // 2. Calculate direction (ignoring vertical Y so the player doesn't lean over)
        Vector3 aimDirection = (targetPoint - transform.position);
        aimDirection.y = 0;

        if (aimDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void EquipUnarmed()
    {
        _isUnarmed = true;
        animator.runtimeAnimatorController = unarmedController;
        SetWeaponModels(false, false);
        UpdateCrosshair(idleCrosshair);
        _targetOffset = 0f;
    }

    public void EquipPistol()
    {
        _isUnarmed = false;
        animator.runtimeAnimatorController = pistolOverride;
        SetWeaponModels(true, false);
        UpdateCrosshair(pistolCrosshair);
        _targetOffset = pistolCameraOffset;
    }

    public void EquipRifle()
    {
        _isUnarmed = false;
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
            crosshairUI.enabled = false;
        }
        else
        {
            crosshairUI.enabled = true;
            crosshairUI.sprite = newSprite;
        }
    }
}