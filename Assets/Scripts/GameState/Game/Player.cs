using UnityEngine;

public class Player : MonoBehaviour
{
    private const float WIDTH = .25f;
    private const float HEIGHT = .25f;

    private const float SPEED = 10;
    private const float GRAVITY = -100;
    private const float JUMP = 15;

    private enum PlayerState
    {
        None,
        Idle,
        Jump,
        Fall
    }
    private PlayerState state = PlayerState.Fall;
    private PlayerState State
    {
        get { return state; }
        set
        {
            state = value;
            switch (state)
            {
                case PlayerState.Idle:
                    this.Velocity = Vector2.zero;
                    break;

                case PlayerState.Fall:
                    break;

                case PlayerState.Jump:
                    this.Velocity += new Vector2(0, JUMP);
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
    private WeaponSlot ActiveWeaponSlot
    {
        get
        {
            return activeWeaponSlot;
        }
        set
        {
            activeWeaponSlot = value;
        }
    }

    // The player's primary weapon.
    public Weapon PrimaryWeapon
    {
        get;
        private set;
    }

    // The player's secondary weapon.
    public Weapon SecondaryWeapon
    {
        get;
        private set;
    }

    private Vector2 jumpVelocity = Vector3.zero;

    // Vector that represents the direction the player is facing.
    private Vector2 direction;
    public Vector2 Direction
    {
        get
        {
            return direction;
        }
        private set
        {
            direction = value;
            // TODO Set animation group from the direction the player is facing.
        }
    }

    // Velocity.
    public Vector2 Velocity
    {
        get;
        private set;
    }

    // Collision AABB.
    public AABB AABB
    {
        get
        {
            // Create a collision AABB.
            return new AABB(transform.position, new Vector3(WIDTH / 2f, HEIGHT / 2f, 0));
        }
    }

    // Public initialize.
    public void Initialize(Vector2 position)
    {
        this.transform.position = position;
    }

    // Self-initialize.
    private void Awake()
    {
        // Create and add the player sprite.
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("FFFFFF-1");
        spriteRenderer.transform.localScale = new Vector3(WIDTH * 100, HEIGHT * 100);

        // Set the active weapon slot.
        this.ActiveWeaponSlot = WeaponSlot.Primary;

        // Equip a pistol.
        this.PrimaryWeapon = Pistol.Create(transform);
        this.PrimaryWeapon.Owner = this;

        // Start facing down. Towards the camera.
        this.Direction = Vector2.down;
    }

    // Update.
    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (state.Equals(PlayerState.Idle) &&
            Input.GetButtonDown("Jump"))
        {
            this.Jump();
        }
        this.Velocity = new Vector2(x * SPEED, this.Velocity.y);

        switch (state)
        {
            case PlayerState.Fall:
                // Apply gravity.
                this.Velocity += new Vector2(0, GRAVITY) * Time.deltaTime;
                break;

            case PlayerState.Jump:
                // Apply gravity.
                this.Velocity += new Vector2(0, GRAVITY) * Time.deltaTime;
                this.Fall();
                break;
        }

        // Calculate broad test player AABB and velocity.
        Vector2 va = this.Velocity * Time.deltaTime;

        // Test walls...
        foreach (Wall wall in CollisionResolver.GetWalls())
        {
            // Sweep test.
            Sweep sweep = this.AABB.SweepAABB(wall.AABB, Vector2.zero - va);
            if (sweep.hit != null)
            {
                this.Idle();
                Debug.Log("HIT");

                // Stop the player in the collision direction.
                // Find the direction the player came from.
                // Move the player out of the wall in that direction.
            }
            else
            {
                this.transform.Translate(va);
            }
        }

        // Check if off-screen and reset if necessary.
        this.CheckBounds();

        // Update direction.
        float absX = Mathf.Abs(x);
        float absY = Mathf.Abs(y);
        if (absX >= absY)
        {
            if (x < 0)
            {
                // Face left.
                this.Direction = Vector2.left;
            }
            else if (x > 0)
            {
                // Face right.
                this.Direction = Vector2.right;
            }
        }
        else if (absY > absX)
        {
            if (y < 0)
            {
                // Face down.
                this.Direction = Vector2.down;
            }
            else if (y > 0)
            {
                // Face up.
                this.Direction = Vector2.up;
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            switch (this.ActiveWeaponSlot)
            {
                case WeaponSlot.Primary:
                    this.PrimaryWeapon.Use();
                    break;

                case WeaponSlot.Secondary:
                    break;
            }
        }
    }

    // Enter the idle state.
    public void Idle()
    {
        this.State = PlayerState.Idle;
    }

    // Enter the jump state.
    public void Jump()
    {
        this.State = PlayerState.Jump;
    }

    // Enter the fall state.
    public void Fall()
    {
        this.State = PlayerState.Fall;
    }

    // Equip a weapon in the active slot.
    private void EquipWeapon(Weapon weapon)
    {
        switch (this.ActiveWeaponSlot)
        {
            case WeaponSlot.Primary:
                this.PrimaryWeapon = weapon;
                break;

            case WeaponSlot.Secondary:
                this.SecondaryWeapon = weapon;
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
        AABB aabb = this.AABB;
        Vector3 viewportMin = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 viewportMax = Camera.main.ViewportToWorldPoint(Vector3.one);
        Vector3 newPosition = aabb.position;

        bool adjustmentRequired = false;
        if (aabb.Min.x < viewportMin.x)
        {
            newPosition.x = viewportMin.x + aabb.half.x;
            adjustmentRequired = true;
        }
        else if (aabb.Max.x > viewportMax.x)
        {
            newPosition.x = viewportMax.x - aabb.half.x;
            adjustmentRequired = true;
        }
        if (aabb.Min.y < viewportMin.y)
        {
            newPosition.y = viewportMin.y + aabb.half.y;
            adjustmentRequired = true;
            this.Idle();
        }
        else if (aabb.Max.y > viewportMax.y)
        {
            newPosition.y = viewportMax.y - aabb.half.y;
            adjustmentRequired = true;
        }

        // Modify the transform's position if an adjustment is required.
        if (adjustmentRequired)
        {
            this.transform.position = newPosition;
        }
    }
}
