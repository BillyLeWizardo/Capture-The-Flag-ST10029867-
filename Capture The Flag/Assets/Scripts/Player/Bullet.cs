using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) //When the bullet collides with another hitbox
    {
        Destroy(gameObject);
    }
}
