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

    [Header("Movement & Aim Settings")]
    public float rotationSpeed = 15f;
    public LayerMask aimLayerMask;
    public float transitionSpeed = 5f;

    [Header("Unarmed Camera Settings")]
    public float idleDistance = 3.5f;
    public float idleVerticalOffset = 0.4f;

    [Header("Pistol Camera Settings")]
    public float pistolHorizontalOffset = -1.2f;
    public float pistolDistance = 2.0f;       // Closer
    public float pistolVerticalOffset = 0.6f; // Higher

    [Header("Rifle Camera Settings")]
    public float rifleHorizontalOffset = -1.5f;
    public float rifleDistance = 2.5f;        // Medium
    public float rifleVerticalOffset = 0.7f;  // Higher

    [Header("Weapon Models")]
    public GameObject pistolModel;
    public GameObject rifleModel;

    [Header("UI / Crosshairs")]
    public Image crosshairUI;
    public Sprite idleCrosshair;
    public Sprite pistolCrosshair;
    public Sprite rifleCrosshair;

    // Internal Target Values
    private float _targetHorizontal = 0f;
    private float _targetDistance = 3.5f;
    private float _targetVertical = 0.4f;
    private bool _isUnarmed = true;

    void Start()
    {
        if (followCamera != null)
            _thirdPersonFollow = followCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

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
        if (_thirdPersonFollow == null) return;

        // 1. Move Horizontal (Shoulder side)
        _thirdPersonFollow.ShoulderOffset.x = Mathf.Lerp(_thirdPersonFollow.ShoulderOffset.x, _targetHorizontal, Time.deltaTime * transitionSpeed);

        // 2. Move Vertical (Height)
        _thirdPersonFollow.ShoulderOffset.y = Mathf.Lerp(_thirdPersonFollow.ShoulderOffset.y, _targetVertical, Time.deltaTime * transitionSpeed);

        // 3. Move Distance (Zoom)
        _thirdPersonFollow.CameraDistance = Mathf.Lerp(_thirdPersonFollow.CameraDistance, _targetDistance, Time.deltaTime * transitionSpeed);
    }

    private void HandleAimRotation()
    {
        if (_isUnarmed) return;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimLayerMask))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(999f);

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
        _targetHorizontal = 0f;
        _targetDistance = idleDistance;
        _targetVertical = idleVerticalOffset;

        animator.runtimeAnimatorController = unarmedController;
        SetWeaponModels(false, false);
        UpdateCrosshair(idleCrosshair);
    }

    public void EquipPistol()
    {
        _isUnarmed = false;
        _targetHorizontal = pistolHorizontalOffset;
        _targetDistance = pistolDistance;
        _targetVertical = pistolVerticalOffset;

        animator.runtimeAnimatorController = pistolOverride;
        SetWeaponModels(true, false);
        UpdateCrosshair(pistolCrosshair);
    }

    public void EquipRifle()
    {
        _isUnarmed = false;
        _targetHorizontal = rifleHorizontalOffset;
        _targetDistance = rifleDistance;
        _targetVertical = rifleVerticalOffset;

        animator.runtimeAnimatorController = rifleOverride;
        SetWeaponModels(false, true);
        UpdateCrosshair(rifleCrosshair);
    }

    private void SetWeaponModels(bool pistolActive, bool rifleActive)
    {
        if (pistolModel != null) pistolModel.SetActive(pistolActive);
        if (rifleModel != null) rifleModel.SetActive(rifleActive);
    }

    private void UpdateCrosshair(Sprite newSprite)
    {
        if (crosshairUI == null) return;
        crosshairUI.enabled = (newSprite != null);
        if (newSprite != null) crosshairUI.sprite = newSprite;
    }
}