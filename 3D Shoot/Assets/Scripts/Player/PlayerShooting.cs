using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    public TMP_Text ammoText;
    public WeaponParametrs currentWeapon;
    public bool weaponReady = true;
    public Transform cam;

    private AudioSource au;
    private bool isReloading = false;

    void Start()
    {
        if (Camera.main != null) cam = Camera.main.transform;

        au = GetComponent<AudioSource>();
        if (au == null)
        {
            au = gameObject.AddComponent<AudioSource>();
            au.spatialBlend = 0f;
            au.volume = 1f;
        }

        RefreshAmmoText();
    }

    void Update()
    {
        if (currentWeapon == null || GetComponent<PlayerHealth>()?.isDead == true) return;

        if (Input.GetMouseButton(0) && !isReloading)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            Reload();
        }
    }

    public void Shoot()
    {
        if (!weaponReady || currentWeapon == null || currentWeapon.isBomb || isReloading) return;

        bool canShoot = (currentWeapon.ammoInClip > 0 || currentWeapon.isKnife);
        if (!canShoot || currentWeapon.lastTimeShoot > Time.time) return;

        PlayRandomClip(currentWeapon.shootClip);

        if (currentWeapon.muzzleFlash != null && currentWeapon.muzzleFlash.Length > 0 && currentWeapon.shootPoint != null)
        {
            int flashIndex = Random.Range(0, currentWeapon.muzzleFlash.Length);
            Instantiate(currentWeapon.muzzleFlash[flashIndex], currentWeapon.shootPoint.position, currentWeapon.shootPoint.rotation, currentWeapon.shootPoint);
        }

        SpawnProjectile();

        if (!currentWeapon.isKnife)
        {
            currentWeapon.ammoInClip--;
        }

        currentWeapon.lastTimeShoot = Time.time + currentWeapon.shootCoolDown;

        RefreshAmmoText();
    }

    private void SpawnProjectile()
    {
        if (currentWeapon.projectilePrefab == null || currentWeapon.shootPoint == null || cam == null) return;

        Vector3 direction = cam.forward + Random.insideUnitSphere * currentWeapon.spread;
        direction.Normalize();

        GameObject proj = Instantiate(currentWeapon.projectilePrefab, currentWeapon.shootPoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float speed = currentWeapon.projectileSpeed > 0 ? currentWeapon.projectileSpeed : 150f;
            rb.linearVelocity = direction * speed;
        }
    }

    public void Reload()
    {
        if (currentWeapon == null || currentWeapon.isKnife || isReloading) return;
        if (currentWeapon.ammoInClip >= currentWeapon.clipSize || currentWeapon.ammoTotal <= 0) return;

        isReloading = true;

        int needed = currentWeapon.clipSize - currentWeapon.ammoInClip;
        int take = Mathf.Min(needed, currentWeapon.ammoTotal);

        currentWeapon.ammoInClip += take;
        currentWeapon.ammoTotal -= take;

        if (currentWeapon.reloadClip != null)
        {
            au.PlayOneShot(currentWeapon.reloadClip);
        }

        StartCoroutine(ReloadCooldown(currentWeapon.reloadTime));
        RefreshAmmoText();
    }

    private IEnumerator ReloadCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isReloading = false;
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
        {
            int index = Random.Range(0, clips.Length);
            if (clips[index] != null) au.PlayOneShot(clips[index]);
        }
    }

    public void RefreshAmmoText()
    {
        if (ammoText == null || currentWeapon == null) return;

        if (!currentWeapon.isKnife)
        {
            ammoText.text = $"{currentWeapon.ammoInClip}/{currentWeapon.ammoTotal}";
        }
        else
        {
            ammoText.text = "∞";
        }
    }
}