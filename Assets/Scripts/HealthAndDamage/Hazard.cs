using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float damageAmount;
    public List<string> tags;

    void OnTriggerEnter(Collider collision)
    {
        if (!tags.Contains(collision.tag)) return;

        Health health = collision.GetComponent<Health>();
        if(health != null)
        {
            health.TakeDamage(damageAmount);
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (!tags.Contains(collision.gameObject.tag)) return;
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
        }
    }
}
