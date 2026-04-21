using UnityEngine;
using UnityEngine.InputSystem; // Add this line at the top!

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Controllers")]
    public Animator animator;
    public RuntimeAnimatorController unarmedController;
    public AnimatorOverrideController pistolOverride;

    [Header("Weapon Objects")]
    public GameObject pistolModel;

    void Update()
    {
        // New Input System way of checking keys
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            EquipUnarmed();
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            EquipPistol();
        }
    }

    void EquipUnarmed()
    {
        animator.runtimeAnimatorController = unarmedController;
        if (pistolModel != null) pistolModel.SetActive(false);
        Debug.Log("Switched to Unarmed");
    }

    void EquipPistol()
    {
        animator.runtimeAnimatorController = pistolOverride;
        if (pistolModel != null) pistolModel.SetActive(true);
        Debug.Log("Switched to Pistol");
    }
}