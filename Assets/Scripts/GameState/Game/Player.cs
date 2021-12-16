using UnityEngine;
using System;

public class Player : MonoBehaviour {
    public GameObject pistolTemplate;

    // Weapon slot enum.
    public enum WeaponSlot {
        Primary,
        Secondary
    }
    private WeaponSlot activeWeaponSlot;
    private WeaponSlot ActiveWeaponSlot {
        get {
            return activeWeaponSlot;
        }
        set {
            activeWeaponSlot = value;
        }
    }

    // The player's primary weapon.
    public Weapon PrimaryWeapon {
        get;
        private set;
    }

    // The player's secondary weapon.
    public Weapon SecondaryWeapon {
        get;
        private set;
    }

    // Vector that represents the direction the player is facing.
    private Vector3 direction;
    public Vector3 Direction {
        get {
            return direction;
        }
        private set {
            direction = value;
        }
    }

    // Self-initialize.
    private void Awake() {
        // Set the active weapon slot.
        ActiveWeaponSlot = WeaponSlot.Primary;

        // Equip a pistol.
        PrimaryWeapon = Instantiate(pistolTemplate).GetComponent<Pistol>();
        PrimaryWeapon.Initialize(transform, this);

        // Start facing down. Towards the camera.
        Direction = Vector3.down;
    }

    public void Initialize(Transform parent) {
        transform.SetParent(parent);
    }

    // Update.
    private void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Update direction.
        float absX = Mathf.Abs(x);
        float absY = Mathf.Abs(y);
        if (absX >= absY) {
            if (x < 0) {
                // Face left.
                Direction = Vector3.left;
            } else if (x > 0) {
                // Face right.
                Direction = Vector3.right;
            }
        } else if (absY > absX) {
            if (y < 0) {
                // Face down.
                Direction = Vector3.down;
            } else if (y > 0) {
                // Face up.
                Direction = Vector3.up;
            }
        }

        if (Input.GetButtonDown("Fire1")) {
            switch (ActiveWeaponSlot) {
                case WeaponSlot.Primary:
                    PrimaryWeapon.Use();
                    break;

                case WeaponSlot.Secondary:
                    break;
            }
        }
    }

    // Equip a weapon in the active slot.
    private void EquipWeapon(Weapon weapon) {
        switch (ActiveWeaponSlot) {
            case WeaponSlot.Primary:
                PrimaryWeapon = weapon;
                break;

            case WeaponSlot.Secondary:
                SecondaryWeapon = weapon;
                break;
        }
    }
}
