using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using   System;

public class ScreenShakeActions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;
    }

    private void SwordAction_OnAnySwordHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake();
    }
    private void GrenadeProjectile_OnAnyGrenadeExploded (object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
    }
}
