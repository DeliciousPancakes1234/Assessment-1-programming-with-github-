using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthWithArmor : HealthWithUI
{
    public float armor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void TakeDamage(float damageAmount)
    {
        if(armor > 0)
        {
            armor -= damageAmount;
            damageAmount *= 0.7f;
        }
        base.TakeDamage(damageAmount);
    }
}
