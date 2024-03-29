﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon {
    public GameObject bulletTemplate;

    // Total bullet in the magazine.
    private int magazineSize = 6;

    // Bullet count in magazine.
    private int magazineCount = 0;

    // List of active bullets fired by this weapon.
    private List<Bullet> activeBullets;

    // List of inactive bullets in the magazine.
    private List<Bullet> inactiveBullets;

    // Self-initialize.
    private void Awake() {
        // Create the active bullet list.
        activeBullets = new List<Bullet>();

        // Create the inactive bullet list.
        inactiveBullets = new List<Bullet>();

        // Start the gun loaded.
        Load();
    }

    // Initialize the weapon.
    public override void Initialize(Transform parent, Player owner) {
        transform.SetParent(parent);
        Owner = owner;
    }

    // Use the weapon.
    public override void Use() {
        // Only shoot if available bullets in magazine.
        if (magazineCount > 0) {
            // Shoot a single bullet.
            Bullet bullet;

            // Reuse a bullet if possible.
            if (inactiveBullets.Count > 0) {
                bullet = inactiveBullets[0];
                inactiveBullets.RemoveAt(0);
                bullet.Initialize(Game.Instance.transform, transform.position, Owner.Direction, ReturnBullet);
            } else {
                // Shoot a single bullet.
                //bullet = Bullet.Create(Owner.transform.parent, ReturnBullet);
                GameObject go = Instantiate(bulletTemplate);
                bullet = go.GetComponent<Bullet>();
                bullet.Initialize(Game.Instance.transform, transform.position, Owner.Direction, ReturnBullet);
            }
            activeBullets.Add(bullet);
            --magazineCount;
        } else {
            // Reload.
            Load();
        }
    }

    // Load the gun.
    private void Load() {
        magazineCount = magazineSize;
    }

    // Return the bullet to this gun's inactive bullet list.
    public void ReturnBullet(Bullet bullet) {
        // Move from the active to inactive list.
        activeBullets.Remove(bullet);
        inactiveBullets.Add(bullet);
    }
}
