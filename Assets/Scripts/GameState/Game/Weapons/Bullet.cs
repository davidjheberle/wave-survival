using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Create the pistol.
    public static Bullet Create(Vector3 spawnLocation)
    {
        // Create a game object and add a bullet script to it.
        GameObject bulletGameObject = new GameObject("Bullet", typeof(Bullet));
        bulletGameObject.transform.position = spawnLocation;
        return bulletGameObject.GetComponent<Bullet>();
    }

    // Self-initialize.
    private void Awake()
    {
        // Create and add the bullet sprite.
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("FFFFFF-1");
        spriteRenderer.color = Color.blue;
        spriteRenderer.transform.localScale = new Vector3(5, 5, 1);
    }
}
