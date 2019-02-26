using UnityEngine;

public class Player : MonoBehaviour
{
    private const float WIDTH = .25f;
    private const float HEIGHT = .25f;

    private const float SPEED = 10;
    private const float GRAVITY = -15;
    private const float FALL = -15;
    private const float JUMP = 20;

    private enum PlayerState
    {
        None,
        Idle,
        Jumping,
        Falling
    }
    private PlayerState state = PlayerState.Falling;
    private PlayerState State {
        get { return state; }
        set {
            state = value;
            switch (state)
            {
                case PlayerState.Idle:
                    break;

                case PlayerState.Falling:
                    break;

                case PlayerState.Jumping:
                    jumpVelocity.y = JUMP;
                    break;
            }
        }
    }

    // Create the player.
    public static Player Create(Transform parent)
    {
        // Create a game object and add a player script to it.
        Player player = new GameObject("Player", typeof(Player)).GetComponent<Player>();
        player.transform.SetParent(parent);

        CollisionResolver.AddPlayer(player);

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

    private Vector3 jumpVelocity = Vector3.zero;

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

    // Velocity.
    public Vector3 Velocity {
        get;
        private set;
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

        if (state.Equals(PlayerState.Idle) &&
            Input.GetButtonDown("Jump"))
        {
            State = PlayerState.Jumping;
        }
        Vector3 velocity = new Vector3(x, 0) * SPEED * Time.deltaTime;

        switch (state)
        {
            case PlayerState.Falling:
                // Apply gravity.
                velocity.y += GRAVITY * Time.deltaTime;
                break;

            case PlayerState.Jumping:
                // Apply jump velocity.
                velocity += jumpVelocity * Time.deltaTime;

                // Apply gravity.
                velocity.y += GRAVITY * Time.deltaTime;

                // Decrease jump velocity.
                jumpVelocity.y += FALL * Time.deltaTime;
                if (jumpVelocity.y <= 0)
                {
                    jumpVelocity = Vector3.zero;
                    State = PlayerState.Falling;
                }
                break;
        }

        Velocity = velocity;
        transform.Translate(Velocity);

        // Check if off-screen and reset if necessary.
        CheckBounds();

        // Update direction.
        float absX = Mathf.Abs(x);
        float absY = Mathf.Abs(y);
        if (absX >= absY)
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
            State = PlayerState.Idle;
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
