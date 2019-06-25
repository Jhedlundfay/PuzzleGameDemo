using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyList[] enemies = new EnemyList[6];
    private Board board;
    public int width;
    public int height;
    public float rarityMultiplier;
    public int movesPerTurn;
    public int remainingMoves;
    public GameObject[] allEnemies;
    public int enemiesDefeated;

    private Transform[] allIndicators;
    private float[] normalizedRarities;
    private DamageManager damageManager;
    private SoundManager soundManager;

    //array[0] will be health, array[1]  will be damage array[2] will be rarity


    // Start is called before the first frame update
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        enemiesDefeated = 0;
        damageManager = FindObjectOfType<DamageManager>();
        board = FindObjectOfType<Board>();
        allEnemies = new GameObject[width];
        allIndicators = new Transform[width];
        remainingMoves = movesPerTurn;
        normalizedRarities = new float[6];
        float sumRarities = 0.0f;
        for (int j = 0; j < enemies.Length; j++)
        {
            sumRarities += enemies[j].enemyRarity * rarityMultiplier;
        }
        for (int j = 0; j < enemies.Length; j++)
        {
            normalizedRarities[j] = enemies[j].enemyRarity / sumRarities;
            for (int k = 0; k < j; k++)
            {
                normalizedRarities[j] += normalizedRarities[k];
            }
        }
        SetUp();
        allIndicators[0].gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void TurnAction()
    {
        for (int i = 0; i < width; i++)
        {
            if (allIndicators[i].gameObject.GetComponent<SpriteRenderer>().enabled)
            {
                allIndicators[i].gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (i == remainingMoves % width)
            {
                allIndicators[i].gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        if (board.currentState == GameState.Enemy)
        {
            for (int i = 0; i < width; i++)
            {
                damageManager.InflictDamage(allEnemies[i].GetComponent<Enemy>().damage);
            }
            remainingMoves = movesPerTurn;
            board.currentState = GameState.PlayerMove;
        }
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            Vector2 tempPosition;
            if (i == 0)
            {
                tempPosition = new Vector2((i - 1) * 2f, height);
            }
            else
            {
                tempPosition = new Vector2(((width - i) - 1) * 2f, height);
            }
            float randomFloat = Random.Range(0.0f, 1.0f);
            StartCoroutine(SpawnEnemy(tempPosition, randomFloat, i));
        }
    }

    private IEnumerator SpawnEnemy(Vector2 tempPosition, float randomFloat, int i)
    {
        yield return new WaitForSeconds(.5f);
        Debug.Log("Spawning a new Enemy");
        GameObject enemy;
        if (randomFloat < normalizedRarities[0])
        {
            enemy = Instantiate(enemies[0].enemyGameObject, tempPosition, Quaternion.identity);
        }
        else if (randomFloat >= normalizedRarities[0] && randomFloat < normalizedRarities[1])
        {
            enemy = Instantiate(enemies[1].enemyGameObject, tempPosition, Quaternion.identity);
        }
        else if (randomFloat >= normalizedRarities[1] && randomFloat < normalizedRarities[2])
        {
            enemy = Instantiate(enemies[2].enemyGameObject, tempPosition, Quaternion.identity);
        }
        else if (randomFloat >= normalizedRarities[2] && randomFloat < normalizedRarities[3])
        {
            enemy = Instantiate(enemies[3].enemyGameObject, tempPosition, Quaternion.identity);
        }
        else if (randomFloat >= normalizedRarities[3] && randomFloat < normalizedRarities[4])
        {
            enemy = Instantiate(enemies[4].enemyGameObject, tempPosition, Quaternion.identity);
        }
        else
        {
            enemy = Instantiate(enemies[5].enemyGameObject, tempPosition, Quaternion.identity);
        }
        enemy.transform.parent = this.transform;
        enemy.transform.position += this.transform.position;
        Debug.Log(enemy.name);
        SpriteRenderer sprite = enemy.GetComponent<SpriteRenderer>();
        Transform healthBarTransform = enemy.transform.GetChild(0);
        Transform indicatorTransform = enemy.transform.GetChild(1);
        SkillMeter enemyHealthBar = enemy.transform.GetChild(0).gameObject.GetComponent<SkillMeter>();
        healthBarTransform.position = new Vector2(enemy.transform.position.x, enemy.transform.position.y - sprite.bounds.extents.y);
        healthBarTransform.localScale = new Vector3(0.005f, 0.002f, 0.005f);
        indicatorTransform.position = new Vector2(enemy.transform.position.x, enemy.transform.position.y + sprite.bounds.extents.y + 0.2f);
        indicatorTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        if (i == remainingMoves % width)
        {
            indicatorTransform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            indicatorTransform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        allEnemies[i] = enemy;
        allIndicators[i] = indicatorTransform;
    }

    public void DestroyAllEnemies()
    {
        for (int i = 0; i < width; i++)
        {
            if (allEnemies[i].GetComponent<Enemy>().currentHealth == 0)
            {
                Vector2 tempPosition;
                if (i == 0)
                {
                    tempPosition = new Vector2((i - 1) * 2f, height);
                }
                else
                {
                    tempPosition = new Vector2(((width - i) - 1) * 2f, height);
                }
                for (int j = 0; j < enemies.Length; j++)
                {
                    if (enemies[j].enemyGameObject.tag == allEnemies[i].gameObject.tag)
                    {
                        Debug.Log("Playing Enemy Defeat Sound: " + enemies[j].enemyDefeatSound);
                        soundManager.PlaySound(enemies[j].enemyDefeatSound);
                    }
                }
                Destroy(allEnemies[i]);
                enemiesDefeated += 1;
                float randomFloat = Random.Range(0.0f, 1.0f);
                Debug.Log("Enemies have been destroyed. Preparing to Spawn a New Enemy");
                StartCoroutine(SpawnEnemy(tempPosition, randomFloat, i));
                TurnAction();
            }
        }
    }
}