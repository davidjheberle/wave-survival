using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    // Create the pistol.
    public static Pistol Create(Transform parent)
    {
        // Create a game object and add a pistol script to it.
        Pistol pistol = new GameObject("Pistol", typeof(Pistol)).GetComponent<Pistol>();
        pistol.transform.SetParent(parent);
        return pistol;
    }

    // Total bullet in the magazine.
    private int magazineSize = 6;

    // Bullet count in magazine.
    private int magazineCount = 0;

    // List of active bullets fired by this weapon.
    private List<Bullet> activeBullets;

    // List of inactive bullets in the magazine.
    private List<Bullet> inactiveBullets;

    // Self-initialize.
    private void Awake()
    {
        // Create the active bullet list.
        activeBullets = new List<Bullet>();

        // Create the inactive bullet list.
        inactiveBullets = new List<Bullet>();

        // Create and add the pistrol sprite.
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("FFFFFF-1");
        spriteRenderer.color = Color.red;
        spriteRenderer.transform.localScale = new Vector3(5, 5, 1);

        // Start the gun loaded.
        Load();
    }

    // Use the weapon.
    public override void Use()
    {
        // Only shoot if available bullets in magazine.
        if (magazineCount > 0)
        {
            // Shoot a single bullet.
            Bullet bullet;

            // Reuse a bullet if possible.
            if (inactiveBullets.Count > 0)
            {
                bullet = inactiveBullets[0];
                inactiveBullets.RemoveAt(0);
                bullet.Initialize(transform.position, Owner.Direction);
            }
            else
            {
                // Shoot a single bullet.
                bullet = Bullet.Create(ReturnBullet);
                bullet.Initialize(transform.position, Owner.Direction);
            }
            activeBullets.Add(bullet);
            --magazineCount;
        } else
        {
            // Reload.
            Load();
        }
    }

    // Load the gun.
    private void Load()
    {
        magazineCount = magazineSize;
    }

    // Return the bullet to this gun's inactive bullet list.
    public void ReturnBullet(Bullet bullet)
    {
        // Move from the active to inactive list.
        activeBullets.Remove(bullet);
        inactiveBullets.Add(bullet);
    }
}
