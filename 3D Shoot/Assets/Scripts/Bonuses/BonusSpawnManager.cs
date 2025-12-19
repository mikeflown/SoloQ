using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BonusSpawnData
{
    public GameObject bonusPrefab;
    public float spawnChance = 0.3f; // Шанс этого типа (сумма всех должна быть ~1)
}

public class BonusSpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] bonusSpawnPoints; // Точки спавна

    [Header("Bonus Types")]
    public BonusSpawnData[] bonusTypes; // Heal, Ammo, SpeedBoost

    [Header("Spawn Timer")]
    public float minSpawnTime = 20f; // Мин. интервал
    public float maxSpawnTime = 40f; // Макс. интервал

    private List<GameObject> activeBonuses = new List<GameObject>(); // Текущие бонусы на сцене

    void Start()
    {
        if (bonusSpawnPoints.Length == 0)
        {
            Debug.LogError("BonusSpawnManager: Добавь точки спавна бонусов!");
        }
        if (bonusTypes.Length == 0)
        {
            Debug.LogError("BonusSpawnManager: Добавь типы бонусов!");
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // Удаляем все предыдущие бонусы перед спавном нового
            ClearAllBonuses();

            SpawnRandomBonus();
        }
    }

    void ClearAllBonuses()
    {
        foreach (GameObject bonus in activeBonuses)
        {
            if (bonus != null)
            {
                Destroy(bonus);
            }
        }
        activeBonuses.Clear();
    }

    void SpawnRandomBonus()
    {
        // Выбираем случайную точку
        Transform spawnPoint = bonusSpawnPoints[Random.Range(0, bonusSpawnPoints.Length)];

        // Выбираем тип бонуса по шансам
        float totalChance = 0f;
        foreach (var type in bonusTypes)
        {
            totalChance += type.spawnChance;
        }

        float random = Random.Range(0f, totalChance);
        float current = 0f;
        GameObject selectedPrefab = bonusTypes[0].bonusPrefab;

        foreach (var type in bonusTypes)
        {
            current += type.spawnChance;
            if (random <= current)
            {
                selectedPrefab = type.bonusPrefab;
                break;
            }
        }

        // Спавним
        Vector3 spawnPos = spawnPoint.position + Vector3.up * 0.5f;
        GameObject newBonus = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        activeBonuses.Add(newBonus);

        Debug.Log($"Spawned bonus: {selectedPrefab.name} at {spawnPoint.name}");
    }
}