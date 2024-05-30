using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TileManager _tileManager; 
    
    public GameState State;

    private int currentLevelID = 1; // Level number
    private void Awake()
    {
        Instance = this;
    }

    public void UpdateGameState(GameState gameState)
    {
        State = gameState;

        switch(State)
        {
            case GameState.Start:
                _tileManager.GenerateTile(1);
                break;
            case GameState.Lose:
                _tileManager.ClearSlot();
                _tileManager.GenerateTile(currentLevelID);
                break;
            case GameState.NextGame:
                currentLevelID++;
                if (currentLevelID > 5)
                {
                    UpdateGameState(GameState.End);
                }
                _tileManager.ClearSlot();
                _tileManager.GenerateTile(currentLevelID);
                break;
            case GameState.End:
                _tileManager.ClearSlot();
                _tileManager.GenerateTile(1);
                break;
        }
    }
    public enum GameState
    {
        Start,
        Lose,
        NextGame,
        End
    };
}
