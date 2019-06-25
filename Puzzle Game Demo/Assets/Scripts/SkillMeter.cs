using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMeter : MonoBehaviour
{
    public bool skillReady = false;
    public Transform bar;
    public SpriteRenderer border;
    public float skillLimit = 100f;

    private DamageManager damageManager;
    private EnemyManager enemyManager;
    private Player player;
    private Vector3 scale;
    private Board board;
    private SoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        damageManager = FindObjectOfType<DamageManager>();
        enemyManager = FindObjectOfType<EnemyManager>();
        player = FindObjectOfType<Player>();
        board = FindObjectOfType<Board>();
        bar = this.gameObject.transform.GetChild(0).GetChild(2);
        border = this.gameObject.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
        bar.GetChild(0).GetComponent<SpriteRenderer>().color = this.gameObject.GetComponent<Text>().color;
        scale = new Vector3(0f, 1f, 1f);
        bar.localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            CastRay();
        }
        bar.localScale = scale;
        if (scale.x == 1f)
        {
            border.color = new Color(1f, 1f, 1f, 1f);
            skillReady = true;
        }
    }

    public void SetScale(float counterValue)
    {
        scale = new Vector3 (Mathf.Min(counterValue/skillLimit, 1f), 1f, 1f);
    }

    private void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.name == "ForegroundSprite")
            {
                if (hit.collider.gameObject.transform.parent.parent.parent.gameObject == this.gameObject)
                {
                    if (board.currentState == GameState.PlayerMove)
                    {
                        if (skillReady)
                        {
                            tag = this.gameObject.tag;
                            if (tag == "Red Dot")
                            {
                                soundManager.PlaySound("Comet_Storm");
                                for (int i = 0; i < board.width; i++)
                                {
                                    for (int j = 0; j < board.height; j++)
                                    {
                                        if (board.allDots[i, j].GetComponent<Dot>().gameObject.tag == tag)
                                        {
                                            board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                                        }
                                    }
                                }
                                board.currentState = GameState.PlayerWait;
                                board.DestroyMatches();
                            }
                            else if (tag == "White Dot")
                            {
                                soundManager.PlaySound("Restore");
                                player.SetScale(player.health/-5);
                            }
                            else if (tag == "Black Dot")
                            {
                                soundManager.PlaySound("Multiply_Wounds");
                                StartCoroutine(board.MultiplyWounds(1f, 1f));
                                for (int i = 0; i < enemyManager.width; i++)
                                {
                                    int healthlost = enemyManager.allEnemies[i].GetComponent<Enemy>().health - enemyManager.allEnemies[i].GetComponent<Enemy>().currentHealth;
                                    enemyManager.allEnemies[i].GetComponent<Enemy>().SetScale(healthlost);
                                }
                            }
                            else if (tag == "Blue Dot")
                            {
                                enemyManager.remainingMoves += enemyManager.movesPerTurn * 2;
                                StartCoroutine(board.FreezeEnemies(0.5f, 0.5f));
                                soundManager.PlaySound("Time_Freeze");
                            }
                            else if (tag == "Yellow Dot")
                            {
                                soundManager.PlaySound("Invigorate");
                                board.pieceValue *= 5;
                            }
                            skillReady = false;
                            scale = new Vector3(0f, 1f, 1f);
                            border.color = new Color(0f, 0f, 0f, 1f);
                            board.ResetCounter(this);
                            Debug.Log(hit.collider.gameObject.name);
                        }
                    }
                }
            }
        }
    }
}
