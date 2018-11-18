using UnityEngine;

public class Player : MonoBehaviour
{
    // Create the player.
    public static Player Create(Transform parent)
    {
        // Create a game object and add a player script to it.
        Player player = new GameObject("Player", typeof(Player)).GetComponent<Player>();
        player.transform.SetParent(parent);
        return player;
    }

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

    // Vector that represents the direction the player is facing.
    private Vector3 direction;
    public Vector3 Direction {
        get {
            return direction;
        }
        private set {
            direction = value;
            // TODO Set animation group from the direction the player is facing.
        }
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
        PrimaryWeapon.Owner = this;

        // Start facing down. Towards the camera.
        Direction = Vector3.down;
    }

    // Update.
    private void Update()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * SPEED_X;
        float y = Input.GetAxis("Vertical") * Time.deltaTime * SPEED_Y;
        transform.Translate(x, y, 0);

        // Update direction.
        float absX = Mathf.Abs(x);
        float absY = Mathf.Abs(y);
        if (absX > absY)
        {
            if (x < 0)
            {
                // Face left.
                Direction = Vector3.left;
            } else if (x > 0)
            {
                // Face right.
                Direction = Vector3.right;
            }
        } else if (absY > absX)
        {
            if (y < 0)
            {
                // Face down.
                Direction = Vector3.down;
            } else if (y > 0)
            {
                // Face up.
                Direction = Vector3.up;
            }
        }

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
