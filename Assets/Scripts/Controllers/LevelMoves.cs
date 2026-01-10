using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMoves : LevelCondition
{
    private int m_moves;

    private BoardController m_board;

    private CellCollectedController m_cellCollectedController;

    public override void Setup(float value, Text txt, BoardController board, CellCollectedController cellCollectedController)
    {
        base.Setup(value, txt);
 
        m_moves = 0;

        m_board = board;

        m_cellCollectedController = cellCollectedController;

        m_board.OnMoveEvent += OnMove;

        UpdateText();
    }

    private void OnMove()
    {
        if (m_conditionCompleted) return;

        m_moves++;

        UpdateText();

        if(m_cellCollectedController.CheckGameOver())
        {
            OnConditionComplete();
        }
        else
        {
            EvenManager.InvokeCheckGameWin();
        }
    }

    protected override void UpdateText()
    {
        m_txt.text = string.Format("MOVES:\n{0}", m_moves);
    }

    protected override void OnDestroy()
    {
        if (m_board != null) m_board.OnMoveEvent -= OnMove;

        base.OnDestroy();
    }
}
