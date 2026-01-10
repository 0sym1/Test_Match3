using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottom
{
    private Transform m_root;
    private int m_Length = 5;
    private Vector2 m_originPosition = new Vector2(-1.8f, -3.6f);

    private List<Cell> m_cellCollecteds = new List<Cell>();

    public Bottom(Transform root)
    {
        m_root = root;
        CreatCellCollected();
    }
    
    private void CreatCellCollected()
    {
        Cell cell = Resources.Load<Cell>(Constants.PREFAB_CELL_COLLECTED); 
        for(int i=0 ; i< m_Length; i++)
        {
            Cell cellCollected = GameObject.Instantiate(cell);
            cellCollected.transform.SetParent(m_root);
            cellCollected.transform.localPosition = new Vector2(m_originPosition.x + i * 0.9f, m_originPosition.y);

            m_cellCollecteds.Add(cellCollected);
        }
    }

    public List<Cell> GetCellCollecteds()
    {
        return m_cellCollecteds;
    }
}
