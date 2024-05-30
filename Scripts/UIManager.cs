using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;


public class UIManager : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image _buttonImg;
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _text;

    private Sprite START, NEXT, REPEAT;

    private void Start()
    {
        START = Resources.Load<Sprite>("UI/play");
        NEXT = Resources.Load<Sprite>("UI/arrow_right");
        REPEAT = Resources.Load<Sprite>("UI/repeat");
        Show(GameManager.Instance.State);
    }
    public void Show(GameManager.GameState gameState)
    {
        background.enabled = true;
        _panel.SetActive(true);
        GameManager.Instance.State = gameState;

        switch (gameState)
        {
            case GameManager.GameState.Start:
                _buttonImg.sprite = START;
                _text.SetText("Press Icon to start");
                break;

            case GameManager.GameState.NextGame:
                _buttonImg.sprite = NEXT;
                _text.SetText("Stage complete");
                break;

            case GameManager.GameState.Lose:
                _buttonImg.sprite = REPEAT;
                _text.SetText("You lose");
                break;

            case GameManager.GameState.End:
                _buttonImg.sprite = REPEAT;
                _text.SetText("End of prototype");
                break;
        }

    }

    public void OnClicked()
    {
        background.enabled = false;
        _panel.SetActive(false);
        GameManager.Instance.UpdateGameState(GameManager.Instance.State);
    }
}
