using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int currentHealth;
    public Transform bar;
    public SpriteRenderer border;
    public int health;
    public Color indicatorGreen;
    private Vector3 scale;
    public GameObject gameOverOverlay;
    public Text enemiesDefeated;
    private EnemyManager enemyManager;
    private Board board;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        enemyManager = FindObjectOfType<EnemyManager>();
        bar = this.gameObject.transform.GetChild(0).GetChild(2);
        border = this.gameObject.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
        bar.GetChild(0).GetComponent<SpriteRenderer>().color = indicatorGreen;
        scale = new Vector3(1f, 1f, 1f);
        bar.localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        bar.localScale = scale;
        if (scale.x == 0f)
        {
            board.currentState = GameState.GameOver;
            enemiesDefeated.text = "Enemies Defeated: " + enemyManager.enemiesDefeated;
            gameOverOverlay.SetActive(true);
        }
    }

    public void SetScale(int damageDealt)
    {
        float damageAsRatio = (float)damageDealt / (float)health;
        scale -= new Vector3(damageAsRatio, 0f, 0f);
        if (scale.x < 0f)
        {
            scale = new Vector3(0f, 1f, 1f);
        }
        else if (scale.x >= 1f)
        {
            scale = new Vector3(1f, 1f, 1f);
        }
        currentHealth -= damageDealt;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        else if (currentHealth >= health)
        {
            currentHealth = health;
        }
    }
}
