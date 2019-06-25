using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyList
{
    public string enemyName;
    public GameObject enemyGameObject;
    public int enemyHealth;
    public int enemyDamage;
    public int enemyRarity;
    public string enemyDefeatSound;

    public EnemyList(GameObject newEnemy, string newName,
        int newHealth, int newDamage, int newRarity, string newSound)
    {
        enemyName = newName;
        enemyGameObject = newEnemy;
        enemyHealth = newHealth;
        enemyDamage = newDamage;
        enemyRarity = newRarity;
        enemyDefeatSound = newSound;
    }
}
