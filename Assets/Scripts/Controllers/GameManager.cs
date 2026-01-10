using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };

    public enum eLevelMode
    {
        TIME_ATTACK,
        MOVES,
        AUTO_WIN,
        AUTO_LOSE,
    }

    public enum eStateGame
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        GAME_OVER,
        GAME_WIN,
    }

    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;

            StateChangedAction(m_state);
        }
    }

    public eLevelMode LevelMode { get; private set; }


    private GameSettings m_gameSettings;


    private BoardController m_boardController;
    private CellCollectedController m_cellCollectedController;

    private UIMainManager m_uiMenu;

    private LevelCondition m_levelCondition;

    private void Awake()
    {
        State = eStateGame.SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);
    }

    void Start()
    {
        State = eStateGame.MAIN_MENU;
    }

    internal void SetState(eStateGame state)
    {
        State = state;

        if(State == eStateGame.PAUSE)
        {
            DOTween.PauseAll();
        }
        else
        {
            DOTween.PlayAll();
        }
    }

    public void LoadLevel(eLevelMode mode)
    {
        LevelMode = mode;

        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings);

        m_cellCollectedController = new GameObject("CellCollectedController").AddComponent<CellCollectedController>();
        m_cellCollectedController.StartGame(this);

        switch (mode)
        {
            case eLevelMode.MOVES:
                m_levelCondition = this.gameObject.AddComponent<LevelMoves>();
                m_levelCondition.Setup(m_gameSettings.LevelMoves, m_uiMenu.GetLevelConditionView(), m_boardController, m_cellCollectedController);
                break;
            case eLevelMode.TIME_ATTACK:
                m_levelCondition = this.gameObject.AddComponent<LevelTime>();
                m_levelCondition.Setup(m_gameSettings.LevelTime, m_uiMenu.GetLevelConditionView(), this);
                break;

            case eLevelMode.AUTO_WIN:
                m_levelCondition = this.gameObject.AddComponent<LevelAutoWin>();
                m_levelCondition.Setup(0, m_uiMenu.GetLevelConditionView(), m_boardController);
                break;
            case eLevelMode.AUTO_LOSE:
                m_levelCondition = this.gameObject.AddComponent<LevelAutoLose>();
                m_levelCondition.Setup(0, m_uiMenu.GetLevelConditionView(), m_boardController);
                break;
        }

        m_levelCondition.ConditionCompleteEvent += GameOver;

        State = eStateGame.GAME_STARTED;
    }

    public void GameOver()
    {
        StartCoroutine(WaitBoardController());
    }

    internal void ClearLevel()
    {
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }
        if (m_cellCollectedController)
        {
            m_cellCollectedController.Clear();
            Destroy(m_cellCollectedController.gameObject);
            m_cellCollectedController = null;
        }
    }

    private IEnumerator WaitBoardController()
    {
        while (m_boardController.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        State = eStateGame.GAME_OVER;
        yield return new WaitForSeconds(1f);


        if (m_levelCondition != null)
        {
            m_levelCondition.ConditionCompleteEvent -= GameOver;

            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }
}
