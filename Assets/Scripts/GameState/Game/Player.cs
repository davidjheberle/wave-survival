using UnityEngine;

public class Player : MonoBehaviour
{
    private const float WIDTH = .25f;
    private const float HEIGHT = .25f;

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

    // Speed.
    private const float SPEED = 10;

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

    // Collision AABB.
    public AABB AABB {
        get {
            // Create a collision AABB.
            return new AABB(transform.position, new Vector3(WIDTH / 2f, HEIGHT / 2f, 0));
        }
    }

    // Public initialize.
    public void Initialize(Vector3 position)
    {
        transform.position = position;
    }

    // Self-initialize.
    private void Awake()
    {
        // Create and add the player sprite.
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("FFFFFF-1");
        spriteRenderer.transform.localScale = new Vector3(WIDTH * 100, HEIGHT * 100, 1);

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
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 velocity = new Vector3(x, y) * SPEED * Time.deltaTime;

        // If not jumping.
        // Apply gravity.
        velocity.y += -9.8f * Time.deltaTime;

        transform.Translate(velocity);

        // Check if off-screen and reset if necessary.
        CheckBounds();

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

    // Check the position relative to the viewport.
    private void CheckBounds()
    {
        // Return if the main camera is null.
        if (Camera.main == null)
        {
            return;
        }

        // Check position relative to the viewport.
        AABB aabb = AABB;
        Vector3 viewportMin = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 viewportMax = Camera.main.ViewportToWorldPoint(Vector3.one);
        Vector3 newPosition = aabb.Center;

        bool adjustmentRequired = false;
        if (aabb.Min.x < viewportMin.x)
        {
            newPosition.x = viewportMin.x + aabb.Extents.x;
            adjustmentRequired = true;
        }
        else if (aabb.Max.x > viewportMax.x)
        {
            newPosition.x = viewportMax.x - aabb.Extents.x;
            adjustmentRequired = true;
        }
        if (aabb.Min.y < viewportMin.y)
        {
            newPosition.y = viewportMin.y + aabb.Extents.y;
            adjustmentRequired = true;
        }
        else if (aabb.Max.y > viewportMax.y)
        {
            newPosition.y = viewportMax.y - aabb.Extents.y;
            adjustmentRequired = true;
        }

        // Modify the transform's position if an adjustment is required.
        if (adjustmentRequired)
        {
            transform.position = newPosition;
        }
    }
}
