using UnityEngine;

public class Player : MonoBehaviour
{
    // Weapon slot enum.
    public enum WeaponSlot
    {
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

    // Speed in the x direction.
    private const float SPEED_X = 10;

    // Speed in the y direction.
    private const float SPEED_Y = 10;

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

    // Create the player.
    public static Player Create(Transform parent)
    {
        // Create a game object and add a player script to it.
        GameObject playerGamObject = new GameObject("Player", typeof(Player));
        playerGamObject.transform.SetParent(parent);
        return playerGamObject.GetComponent<Player>();
    }

    // Self-initialize.
    private void Awake()
    {
        // Create and add the player sprite.
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("FFFFFF-1");
        spriteRenderer.transform.localScale = new Vector3(25, 25, 1);

        // Set the active weapon slot.
        ActiveWeaponSlot = WeaponSlot.Primary;

        // Equip a pistol.
        PrimaryWeapon = Pistol.Create(transform);
    }

    // Update.
    private void Update()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * SPEED_X;
        float y = Input.GetAxis("Vertical") * Time.deltaTime * SPEED_Y;
        Debug.LogFormat("x: {0}, y: {1}", x, y);
        transform.Translate(x, y, 0);

        // TODO Set animation group from the direction the player is facing.

        if (Input.GetButtonDown("Fire1"))
        {
            switch (ActiveWeaponSlot)
            {
                case WeaponSlot.Primary:
                    PrimaryWeapon.Use();
                    break;

                case WeaponSlot.Secondary:
                    break;
            }
        }
    }

    // Equip a weapon in the active slot.
    private void EquipWeapon(Weapon weapon)
    {
        switch (ActiveWeaponSlot)
        {
            case WeaponSlot.Primary:
                PrimaryWeapon = weapon;
                break;

            case WeaponSlot.Secondary:
                SecondaryWeapon = weapon;
                break;
        }
    }
}
