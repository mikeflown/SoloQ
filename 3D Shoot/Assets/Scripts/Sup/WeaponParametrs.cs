using UnityEngine;

public class WeaponParametrs : MonoBehaviour
{
    [Header("General")]
    public bool isKnife = false;
    public bool isBomb = false;

    [Header("Ammo")]
    public int ammoInClip = 30;
    public int ammoTotal = 90;
    public int clipSize = 30;

    [Header("Shooting")]
    public float shootCoolDown = 0.1f;
    public float spread = 0.05f;               // Разброс
    public Transform shootPoint;               // Точка спавна вспышки и снаряда

    [Header("Projectile")]
    public GameObject projectilePrefab;        // Префаб снаряда
    public float projectileSpeed = 150f;       // Скорость снаряда

    [Header("Effects")]
    public GameObject[] muzzleFlash;           // Массив вспышек
    public AudioClip[] shootClip;              // Звуки выстрела
    public AudioClip reloadClip;               // Звук перезарядки

    [Header("Reload")]
    public float reloadTime = 2f;              // Время перезарядки

    [HideInInspector] public float lastTimeShoot = -10f;
}