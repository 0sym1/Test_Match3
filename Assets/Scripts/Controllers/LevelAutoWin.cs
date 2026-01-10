using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelAutoWin : LevelCondition
{
    private BoardController m_board;
    private Cell[,] m_cells;

    public override void Setup(float value, Text txt, BoardController board)
    {
        base.Setup(value, txt);

        m_board = board;
        m_cells = m_board.GetCells();

        UpdateText();

        StartCoroutine(StartAutoWin());
    }

    private IEnumerator StartAutoWin()
    {
        yield return new WaitForSeconds(0.5f);
        for(int i=0 ; i< m_board.GetBoardSizeX(); i++)
        {
            for(int j=0; j< m_board.GetBoardSizeY(); j++)
            {
                if(m_cells[i,j].Item == null) continue;

                Item item = m_cells[i,j].Item;
                EvenManager.InvokeItemCollected(m_cells[i,j].Item);
                m_cells[i,j].Free();
                yield return new WaitForSeconds(0.5f);
                FindItem(item);
                yield return new WaitForSeconds(0.5f);
                FindItem(item);
                yield return new WaitForSeconds(0.5f);

            }
        }
        EvenManager.InvokeCheckGameWin();
    }

    private void FindItem(Item item)
    {
        for(int i=0 ; i< m_board.GetBoardSizeX(); i++)
        {
            for(int j=0; j< m_board.GetBoardSizeY(); j++)
            {
                if(m_cells[i,j].Item == null) continue;

                if(m_cells[i,j].Item.IsSameType(item))
                {
                    EvenManager.InvokeItemCollected(m_cells[i,j].Item);
                    m_cells[i,j].Free();
                    return;
                }
            }
        }
    }

    protected override void UpdateText()
    {
        m_txt.text = string.Format("AUTO WIN");
    }
}
