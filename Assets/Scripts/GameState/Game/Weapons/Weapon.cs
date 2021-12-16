using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {
    // Amount of damage a hit from the weapon deals.
    [SerializeField]
    protected float damage = 0;

    // Owner of this weapon. Null if none.
    public Player Owner {
        get;
        set;
    }

    // Initialize the weapon.
    public abstract void Initialize(Transform parent, Player owner);

    // Use the weapon.
    public abstract void Use();

}
