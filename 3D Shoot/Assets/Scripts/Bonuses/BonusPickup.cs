using UnityEngine;
using System.Collections;

public class BonusPickup : MonoBehaviour
{
    public BonusType bonusType = BonusType.Heal;
    public float rotationSpeed = 90f;
    public AudioClip pickupSound;
    public ParticleSystem pickupEffect;

    private AudioSource audioSource;
    private bool isPickedUp = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (!isPickedUp)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPickedUp || !other.CompareTag("Player")) return;

        isPickedUp = true;

        if (pickupSound != null) audioSource.PlayOneShot(pickupSound);
        if (pickupEffect != null) pickupEffect.Play();

        ApplyBonus(other.gameObject);
        Destroy(gameObject, 1f);
    }

    private void ApplyBonus(GameObject playerObj)
    {
        PlayerHealth playerHealth = playerObj.GetComponent<PlayerHealth>();
        PlayerShooting playerShooting = playerObj.GetComponent<PlayerShooting>();
        WeaponParametrs weapon = playerShooting?.currentWeapon;

        if (playerHealth == null || playerHealth.isDead || playerShooting == null) return;

        switch (bonusType)
        {
            case BonusType.Heal:
                playerHealth.Heal(50f);
                Debug.Log("Health Restored! +50 HP");
                break;

            case BonusType.Ammo:
                if (weapon != null)
                {
                    weapon.ammoInClip = weapon.clipSize;
                    weapon.ammoTotal = Mathf.Min(weapon.ammoTotal + 30, weapon.clipSize * 3);
                    playerShooting.RefreshAmmoText();
                    Debug.Log("Ammo Refilled!");
                }
                break;

            case BonusType.SpeedBoost:
                if (weapon != null)
                {
                    float originalCooldown = weapon.shootCoolDown;
                    weapon.shootCoolDown /= 2f;
                    StartCoroutine(InfiniteAmmoBoost(playerShooting, weapon, 10f));
                    Debug.Log("Speed Boost activated!");
                }
                break;
        }
    }

    private IEnumerator InfiniteAmmoBoost(PlayerShooting shooting, WeaponParametrs weapon, float duration)
    {
        weapon.ammoInClip = weapon.clipSize;
        yield return new WaitForSeconds(duration);
        weapon.shootCoolDown *= 2f;
        shooting.RefreshAmmoText();
        Debug.Log("Speed Boost ended!");
    }
}