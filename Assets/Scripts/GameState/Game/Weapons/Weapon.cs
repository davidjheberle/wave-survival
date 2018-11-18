using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // Amount of damage a hit from the weapon deals.
    [SerializeField]
    protected float damage = 0;

    // Use the weapon.
    public abstract void Use();
}
