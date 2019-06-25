using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageManager : MonoBehaviour
{

    public Text damageText;
    public int damage;
    public Transform bar;

    private Board board;
    private EnemyManager enemyManager;
    private Player player;
    private SoundManager soundManager;

    private float shakeTime = 0.5f;
    private float shakeAmount = 2f;
    private bool shaking = false;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        player = FindObjectOfType<Player>();
        board = FindObjectOfType<Board>();
        enemyManager = FindObjectOfType<EnemyManager>();
        damage = 0;
    }

    private void Update()
    {
        if (shaking)
        {
            enemyManager.allEnemies[enemyManager.remainingMoves % enemyManager.width].transform.position += new Vector3 ((Random.insideUnitSphere * (Time.deltaTime * shakeAmount)).x, 0f, 0f);
        }
    }

    public void InflictDamage(int amountDealt)
    {
        damage = amountDealt;
        if (board.currentState != GameState.Enemy)
        {
            if (damage > 0)
            {
                enemyManager.allEnemies[enemyManager.remainingMoves % enemyManager.width].GetComponent<Enemy>().SetScale(damage);
                StartCoroutine(Shake(enemyManager.allEnemies[enemyManager.remainingMoves % enemyManager.width]));
            }
        }
        else
        {
            soundManager.PlaySound("Attack_Damage");
            if (damage > 0)
            {
                player.GetComponent<Player>().SetScale(damage);
            }
        }
        damage = 0;
    }

    public void SetSize (float normal)
    {
        bar.localScale = new Vector2(normal, 1f);
    }

    public IEnumerator Shake (GameObject gameObject)
    {
        Vector2 originalPosition = gameObject.transform.position;

        if (!shaking)
        {
            shaking = true;
        }

        yield return new WaitForSeconds(shakeTime);

        shaking = false;

        gameObject.transform.position = originalPosition;
    }
}
