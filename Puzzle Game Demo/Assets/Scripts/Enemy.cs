using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform bar;
    public SpriteRenderer border;
    public int currentHealth;

    public int health;
    public int damage;
    private Vector3 scale;
    private EnemyManager enemyManager;

    // Start is called before the first frame update
    void Start()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        bar = this.gameObject.transform.GetChild(0).GetChild(2);
        border = this.gameObject.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
        bar.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 1f);
        scale = new Vector3(1f, 1f, 1f);
        bar.localScale = scale;
        SetEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        bar.localScale = scale;
        if (scale.x == 0f)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 0.7f);
            enemyManager.DestroyAllEnemies();
        }
    }

    public void SetScale(int damageDealt)
    {
        float damageAsRatio = (float)damageDealt / (float)health;
        scale -= new Vector3(damageAsRatio, 0f, 0f);
        if  (scale.x < 0f)
        {
            scale = new Vector3(0f, 1f, 1f);
        }
        currentHealth -= damageDealt;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    public void SetEnemy()
    {
        for (int i = 0; i < enemyManager.enemies.Length; i++)
        {
            if (enemyManager.enemies[i].enemyGameObject.tag == this.gameObject.tag)
            {
                health = enemyManager.enemies[i].enemyHealth;
                damage = enemyManager.enemies[i].enemyDamage;
                currentHealth = health;
            }
        }
    }
}
