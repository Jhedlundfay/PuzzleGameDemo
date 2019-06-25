using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    PlayerWait, PlayerMove, Enemy
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.PlayerMove;
    public int height;
    public int width;
    public int offset;
    public GameObject[] dots;
    public GameObject destroyEffect;
    public GameObject[,] allDots;
    public int defaultPieceValue = 10;
    public int defaultMultiplier = 1;
    private DamageManager damageManager;
    public SkillMeter[] counters;

    public int pieceValue;
    public int multiplier;
    private float[] counterValues;
    private EnemyManager enemyManager;
    private Coroutine boardFiller = null;
    private SoundManager soundManager;
    public SpriteRenderer timeFreeze;
    public SpriteRenderer multiplyWounds;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        pieceValue = defaultPieceValue;
        multiplier = defaultMultiplier;
        enemyManager = FindObjectOfType<EnemyManager>();
        counterValues = new float[counters.Length];
        damageManager = FindObjectOfType<DamageManager>();
        allDots = new GameObject[width, height];
        Setup();
    }


    private void Setup()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j+offset);
                int randDot = Random.Range(0, dots.Length);

                while (MatchesAt(i, j, dots[randDot]))
                {
                    randDot = Random.Range(0, dots.Length);
                }

                GameObject dot = Instantiate(dots[randDot], tempPosition, Quaternion.identity);

                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;

                dot.transform.parent = this.transform;
                dot.name = "(" + i + "," + j + ")";
                allDots[i, j] = dot;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject dot)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row].tag == dot.tag && allDots[column - 2, row].tag == dot.tag)
            {
                return true;
            }
            if (allDots[column, row - 1].tag == dot.tag && allDots[column, row - 2].tag == dot.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1].tag == dot.tag && allDots[column, row - 2].tag == dot.tag)
                {
                    return true;
                }
            } else if (column > 1)
            {
                if (allDots[column - 1, row].tag == dot.tag && allDots[column - 2, row].tag == dot.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row] != null)
        {
            if (allDots[column, row].GetComponent<Dot>().isMatched)
            {
                GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
                Destroy(particle, 0.5f);
                Destroy(allDots[column, row]);
                damageManager.InflictDamage(pieceValue * multiplier);
                for (int i = 0; i < counters.Length; i++)
                {
                    if (allDots[column,row].GetComponent<Dot>().tag == counters[i].tag)
                    {
                        counterValues[i]++;
                        counters[i].SetScale(counterValues[i]);
                    }
                }
                allDots[column, row] = null;

            }
        }
    }

    public void DestroyMatches()
    {
        soundManager.PlaySound("Match");
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        if(boardFiller != null)
        {
            StopCoroutine(boardFiller);
        }
        boardFiller = StartCoroutine(FillBoard());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j+offset);
                    int randDot = Random.Range(0, dots.Length);
                    GameObject dot = Instantiate(dots[randDot], tempPosition, Quaternion.identity);
                    allDots[i, j] = dot;
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoard()
    {
        
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while(MatchesOnBoard())
        {
            multiplier += 1;
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        if (enemyManager.remainingMoves == 4)
        {
            UnfreezeEnemies();
        }
        enemyManager.remainingMoves -= 1;
        if (enemyManager.remainingMoves > 0)
        {
            currentState = GameState.PlayerMove;
        }
        else
        {
            currentState = GameState.Enemy;
            pieceValue = defaultPieceValue;
        }
        enemyManager.TurnAction();
        multiplier = defaultMultiplier;
    }

    public void ResetCounter(SkillMeter counter)
    {
        for (int i = 0; i < counters.Length; i++)
        {
            if (counters[i] == counter)
            {
                counterValues[i] = 0;
            }
        }
    }
    
    public IEnumerator FreezeEnemies(float aValue, float aTime)
    {
        timeFreeze.enabled = true;
        float alpha = timeFreeze.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            timeFreeze.color = newColor;
            yield return null;
        }
    }

    public IEnumerator MultiplyWounds(float value, float time)
    {
        multiplyWounds.enabled = true;
        float alpha = multiplyWounds.color.a;
        for (float t = 0.0f; t <= 1.0f; t += Time.deltaTime / time)
        {
            if (t <= 0.5f)
            {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, value, t));
                float skew = Mathf.Lerp(-0.4f, 0f, t);
                multiplyWounds.color = newColor;
                multiplyWounds.sharedMaterial.SetFloat("_HorizontalSkew", skew);
            }
            else
            {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(value, alpha, t));
                float skew = Mathf.Lerp(0f, 0.4f, t);
                multiplyWounds.color = newColor;
                multiplyWounds.sharedMaterial.SetFloat("_HorizontalSkew", skew);
            }
            yield return null;
        }
    }

    public void UnfreezeEnemies()
    {
        soundManager.PlaySound("Ice_Shatter");
        timeFreeze.enabled = false;
    }
}
