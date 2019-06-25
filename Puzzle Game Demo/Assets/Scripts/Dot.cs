using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private FindMatches findMatches;
    private Board board;
    private GameObject otherDot;
    private Vector2 initialTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    private SpriteRenderer mySprite;
    private EnemyManager enemyManager;
    private SoundManager soundManager;
    public float swipeAngle = 0;
    public float swipeResist = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        board = FindObjectOfType<Board>();
        enemyManager = FindObjectOfType<EnemyManager>();
        findMatches = FindObjectOfType<FindMatches>();
        mySprite = GetComponent<SpriteRenderer>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }

    // Update is called once per frame
    void Update()
    {
        //FindMatches();
        findMatches.FindAllMatches();
        if (isMatched)
        {
            mySprite.color = new Color(1f, 1f, 1f, .2f);
            //board.DestroyMatches();
        }
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > 0.1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if(board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }
        if (Mathf.Abs(targetY - transform.position.y) > 0.1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    private IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(.5f);
        if(otherDot != null)
        {
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                //otherDot.GetComponent<Dot>().row = row;
                //otherDot.GetComponent<Dot>().column = column;
                //row = previousRow;
                //column = previousColumn;
                yield return new WaitForSeconds(.5f);
                if (enemyManager.remainingMoves == 4)
                {
                    board.UnfreezeEnemies();
                }
                enemyManager.remainingMoves -= 1;
                if (enemyManager.remainingMoves > 0)
                {
                    board.currentState = GameState.PlayerMove;
                }
                else
                {
                    board.currentState = GameState.Enemy;
                }
                enemyManager.TurnAction();
            }
            else
            {
                board.DestroyMatches();
            }
            otherDot = null;
        } else
        {
            board.currentState = GameState.PlayerMove;
        }
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.PlayerMove)
        {
            initialTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mySprite.color = new Color(.7f, .7f, .7f, 1f);
        }
    }

    private void OnMouseDrag()
    {
        if (board.currentState == GameState.PlayerMove)
        {
            Vector2 tempPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float changeInX = Mathf.Abs(tempPosition.x - initialTouchPosition.x);
            float changeInY = Mathf.Abs(tempPosition.y - initialTouchPosition.y);
            if (changeInX < 2 && changeInX > changeInY)
            {
                if (column == 0)
                {
                    transform.position = new Vector2(Mathf.Max(tempPosition.x,transform.position.x), transform.position.y);
                } else if (column == board.width -1)
                {
                    transform.position = new Vector2(Mathf.Min(tempPosition.x, transform.position.x), transform.position.y);
                }
                else
                {
                    transform.position = new Vector2(tempPosition.x, transform.position.y);
                }
            }
            if (changeInY < 2 && changeInY >= changeInX)
            {
                if (row == 0)
                {
                    transform.position = new Vector2(transform.position.x, Mathf.Max(tempPosition.y, transform.position.y));
                }
                else if (row == board.height - 1)
                {
                    transform.position = new Vector2(transform.position.x, Mathf.Min(tempPosition.y, transform.position.y));
                }
                else
                {
                    transform.position = new Vector2(transform.position.x, tempPosition.y);
                }
            }
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.PlayerMove)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
            mySprite.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - initialTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - initialTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - initialTouchPosition.y, finalTouchPosition.x - initialTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentState = GameState.PlayerWait;
        } else
        {
            board.currentState = GameState.PlayerMove;
        }
    }

    void MovePieces()
    {
        soundManager.PlaySound("Slide");
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width-1)
        {
            //Right Swipe
            otherDot = board.allDots[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height-1)
        {
            //Up Swipe
            otherDot = board.allDots[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left Swipe
            otherDot = board.allDots[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            otherDot = board.allDots[column, row - 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMove());
    }

    void FindMatches()
    {
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];
            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
}
