using UnityEngine;
using System.Collections;
using System;

public class ProjectileWeapon : Weapon {

    // TODO: Weapon accuracy: put rotation noise on spawn point
    public Transform spawnPoint;
    public Transform bulletPref;

    public int maxBulletCount = 6;
    int cbulletCount = 0;

    public float reloadRate = 1;
    public float fireRate = 0.1f;
    //float time;

    bool reloading = false;

    public int bulletDamage = 1;
    public Ai source;

    float reloadTime;

    public bool IsLoaded() {
        if (!HasBullets()) {
            bool ready = Reload();
            return ready;
        }
        return true;
    }

    void Awake() {
        cbulletCount = maxBulletCount;
    }

    public bool Reload() {
        // t: pass, f: wait
        if (!reloading) {
            reloading = true;
            reloadTime = Time.time + reloadRate;
        }

        return ReloadDone();
    }

    private bool ReloadDone() {
        if (reloading && Time.time > reloadTime) {
            cbulletCount = maxBulletCount;
            reloading = false;

            time = Time.time;// make sure firing is possible after reload

            return true;
        }
        return false;
    }

    public bool HasBullets() {
        return cbulletCount > 0;
    }

    public bool IsReady() {
        return Time.time > time;
    }

    [Obsolete("not used")]
    public override bool Attack() {
        if (IsLoaded() && IsReady())
            return Fire();
        return false;
    }

    public bool Fire() {
        Debug.DrawRay(transform.position, transform.right * 5, Color.red, 0.2f);
        Transform bullet = Instantiate(bulletPref);
        bullet.position = spawnPoint.position;
        bullet.rotation = spawnPoint.rotation;
        bullet.GetComponent<Bullet>().Init(source.alliance, bulletDamage);
        time = Time.time + fireRate;
        cbulletCount--;
        return true;
    }

    internal void Aim(Vector3 targetPos) {
        //transform
        transform.right = targetPos - transform.position;
    }
}
