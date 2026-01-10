using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelAutoLose : LevelCondition
{
    private BoardController m_board;
    private Cell[,] m_cells;
    private List<Item> m_itemsCollected = new List<Item>();

    public override void Setup(float value, Text txt, BoardController board)
    {
        base.Setup(value, txt);

        m_board = board;
        m_cells = m_board.GetCells();

        UpdateText();

        StartCoroutine(StartAutoLose());
    }

    private IEnumerator StartAutoLose()
    {
        yield return new WaitForSeconds(0.5f);

        int cnt = 5;
        for(int i=0 ; i< m_board.GetBoardSizeX(); i++)
        {
            for(int j=0; j< m_board.GetBoardSizeY(); j++)
            {
                if(m_cells[i,j].Item == null) continue;
                if(CheckItemCollected(m_cells[i,j].Item)) continue;

                EvenManager.InvokeItemCollected(m_cells[i,j].Item);
                m_itemsCollected.Add(m_cells[i,j].Item);
                m_cells[i,j].Free();
                cnt--;
                yield return new WaitForSeconds(0.5f);
                if(cnt <= 0) break;
            }
            if(cnt <= 0) break;
        }

        OnConditionComplete();
    }

    private bool CheckItemCollected(Item item)
    {
        foreach(var it in m_itemsCollected)
        {
            if(it.IsSameType(item))
            {
                return true;
            }
        }
        return false;
    }

    protected override void UpdateText()
    {
        m_txt.text = string.Format("AUTO LOSE");
    }
}
