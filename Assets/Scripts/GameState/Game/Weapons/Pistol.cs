using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    // Create the pistol.
    public static Pistol Create(Transform parent)
    {
        // Create a game object and add a pistol script to it.
        GameObject pistolGameObject = new GameObject("Pistol", typeof(Pistol));
        pistolGameObject.transform.SetParent(parent);
        return pistolGameObject.GetComponent<Pistol>();
    }

    // List of active bullets fired by this weapon.
    private List<Bullet> activeBullets;

    // Self-initialize.
    private void Awake()
    {
        // Create the active bullet list.
        activeBullets = new List<Bullet>();

        // Create and add the pistrol sprite.
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("FFFFFF-1");
        spriteRenderer.color = Color.red;
        spriteRenderer.transform.localScale = new Vector3(5, 5, 1);
    }

    // Use the weapon.
    public override void Use()
    {
        // Shoot a single bullet.
        Bullet bullet = Bullet.Create(transform.position);
    }
}
