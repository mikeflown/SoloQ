using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public int pointsCost;
    public float spawnWeight = 1f;
}

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public EnemySpawnData[] enemyTypes;

    [Header("Points System")]
    public float basePointsPerSecond = 1f;
    public float pointsGrowthRate = 0.5f;

    [Header("UI")]
    public TMP_Text pointsText;

    private float currentPoints = 0f;
    private float gameTime = 0f;

    void Start()
    {
        if (spawnPoints.Length == 0) Debug.LogError("Добавь точки спавна!");
        if (enemyTypes.Length == 0) Debug.LogError("Добавь префабы врагов!");
        UpdatePointsUI();
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        float pps = basePointsPerSecond + (gameTime / 60f) * pointsGrowthRate;
        currentPoints += pps * Time.deltaTime;

        if (currentPoints >= GetMinCost())
        {
            SpawnRandomEnemy();
        }

        UpdatePointsUI();
    }

    private int GetMinCost()
    {
        int min = int.MaxValue;
        foreach (var type in enemyTypes)
        {
            if (type.pointsCost < min) min = type.pointsCost;
        }
        return min;
    }

    private void SpawnRandomEnemy()
    {
        float totalWeight = 0f;
        foreach (var type in enemyTypes)
        {
            if (currentPoints >= type.pointsCost) totalWeight += type.spawnWeight;
        }

        float random = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var type in enemyTypes)
        {
            if (currentPoints < type.pointsCost) continue;

            currentWeight += type.spawnWeight;
            if (random <= currentWeight)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(type.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                currentPoints -= type.pointsCost;
                Debug.Log($"Spawned {type.enemyPrefab.name} for {type.pointsCost} points");
                break;
            }
        }
    }

    public void AddPoints(float amount)
    {
        currentPoints += amount;
        Debug.Log($" +{amount} points! Total: {currentPoints}");
    }

    private void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            pointsText.text = "Points: " + Mathf.RoundToInt(currentPoints);
        }
    }
}