using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCollectedController : MonoBehaviour
{
    private GameManager m_gameManager;
    private Bottom m_bottom;
    List<Cell> m_cellCollecteds = new List<Cell>();

    private void OnEnable()
    {
        EvenManager.OnItemCollected += HandleItemCollected;
    }

    private void OnDisable()
    {
        EvenManager.OnItemCollected -= HandleItemCollected;
    }

    public void StartGame(GameManager gameManager)
    {
        m_gameManager = gameManager;

        m_bottom = new Bottom(transform);
        m_cellCollecteds = m_bottom.GetCellCollecteds();
    }

    private void HandleItemCollected(Item item)
    {
        if(item.IsSelected){
            FixCells();
            return;
        }
        if(CheckCellsFull()) return;

        Cell cell = null;
        int index_insert = -1;
        for (int i = m_cellCollecteds.Count-1 ; i >= 0; i--)
        {
            if (m_cellCollecteds[i].Item == null)
            {
                cell = m_cellCollecteds[i];
            }
            else if (m_cellCollecteds[i].Item.IsSameType(item))
            {
                cell = m_cellCollecteds[i+1];
                index_insert = i + 1;
                break;
            }
        } 

        if(cell == null) return;

        if(index_insert != -1 && m_cellCollecteds[index_insert].Item != null)
        {
            for(int i = m_cellCollecteds.Count -1; i > index_insert; i--)
            {
                if (m_cellCollecteds[i - 1].Item != null)
                {
                    Item itemMove = m_cellCollecteds[i - 1].Item;
                    m_cellCollecteds[i].Assign(itemMove);
                    itemMove.AnimationMoveToPosition();
                }
            }
        }

        cell.Assign(item);
        item.AnimationMoveToPosition();

        if (CheckMatch())
        {
            for(int i=0 ; i< m_cellCollecteds.Count; i++)
            {
                if(m_cellCollecteds[i].Item == null)
                {
                    for(int j = i+1; j< m_cellCollecteds.Count; j++)
                    {
                        if (m_cellCollecteds[j].Item != null)
                        {
                            Item itemMove = m_cellCollecteds[j].Item;
                            m_cellCollecteds[i].Assign(itemMove);
                            itemMove.AnimationMoveToPosition();
                            m_cellCollecteds[j].Free();
                            break;
                        }
                    }
                }
            }
        }
    }

    private bool CheckMatch()
    {
        for(int i=0; i< m_cellCollecteds.Count -2; i++)
        {
            if (m_cellCollecteds[i].Item != null && m_cellCollecteds[i+1].Item != null && m_cellCollecteds[i+2].Item != null)
            {
                if (m_cellCollecteds[i].Item.IsSameType(m_cellCollecteds[i+1].Item) && m_cellCollecteds[i].Item.IsSameType(m_cellCollecteds[i+2].Item))
                {
                    m_cellCollecteds[i].ExplodeItem();
                    m_cellCollecteds[i+1].ExplodeItem();
                    m_cellCollecteds[i+2].ExplodeItem();

                    return true;
                }
            }
        }

        return false;
    }

    private void FixCells()
    {
        for(int i=0 ; i< m_cellCollecteds.Count - 1; i++)
        {
            if(m_cellCollecteds[i].Item == null)
            {
                if (m_cellCollecteds[i+1].Item != null)
                {
                    Debug.Log("FixCells Move " + i);
                    Item itemMove = m_cellCollecteds[i+1].Item;
                    m_cellCollecteds[i+1].Free();
                    m_cellCollecteds[i].Assign(itemMove);
                    itemMove.AnimationMoveToPosition();
                }
            }
        }
    }

    public bool CheckGameOver()
    {
        if(m_gameManager.LevelMode == GameManager.eLevelMode.TIME_ATTACK)
        {
            return false;
        }

        return CheckCellsFull();
    }

    private bool CheckCellsFull()
    {
        for(int i=0; i< m_cellCollecteds.Count; i++)
        {
            if (m_cellCollecteds[i].Item == null)
            {
                return false;
            }
        }

        return true;
    }

    public void Clear()
    {
        for(int i=0; i< m_cellCollecteds.Count; i++)
        {
            m_cellCollecteds[i].Free();
        }
    }
}
