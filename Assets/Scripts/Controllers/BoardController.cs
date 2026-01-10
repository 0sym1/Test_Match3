using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };

    public bool IsBusy { get; private set; }

    private Board m_board;

    private GameManager m_gameManager;

    private Camera m_cam;

    private Collider2D m_hitCollider;

    private GameSettings m_gameSettings;

    private List<Cell> m_potentialMatch;

    private float m_timeAfterFill;

    private bool m_hintIsShown;

    private bool m_gameOver;

    private void OnEnable()
    {
        EvenManager.OnCheckGameWin += CheckGameWin;
    }
    private void OnDisable()
    {
        EvenManager.OnCheckGameWin -= CheckGameWin;
    }

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);

        Fill();
    }

    private void Fill()
    {
        m_board.Fill();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                break;
        }
    }


    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;

        if (!m_hintIsShown)
        {
            m_timeAfterFill += Time.deltaTime;
            if (m_timeAfterFill > m_gameSettings.TimeForHint)
            {
                m_timeAfterFill = 0f;
            }
        }

        if(m_gameManager.LevelMode == GameManager.eLevelMode.AUTO_WIN || m_gameManager.LevelMode == GameManager.eLevelMode.AUTO_LOSE)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                Item item = cell.Item;

                if(item != null)
                {
                    if(!item.IsSelected)
                    {
                        cell.Free();
                        EvenManager.InvokeItemCollected(item);
                        item.IsSelected = true;
                        OnMoveEvent();
                    }
                    else if(item.IsSelected && m_gameManager.LevelMode == GameManager.eLevelMode.TIME_ATTACK)
                    {
                        item.Cell.Free();
                        item.OriginalCell.Assign(item);
                        item.AnimationMoveToPosition();
                        EvenManager.InvokeItemCollected(item);
                        item.IsSelected = false;
                        OnMoveEvent();
                    }
                }

            }
        }
    }

    public void CheckGameWin()
    {
        if (m_board.CheckGameWin())
        {
            m_gameManager.SetState(GameManager.eStateGame.GAME_WIN);
        }
    }

    public Cell[,] GetCells()
    {
        return m_board.GetCells();
    }
    public int GetBoardSizeX()
    {
        return m_gameSettings.BoardSizeX;
    }
    public int GetBoardSizeY()
    {
        return m_gameSettings.BoardSizeY;
    }

    internal void Clear()
    {
        m_board.Clear();
    }
}
