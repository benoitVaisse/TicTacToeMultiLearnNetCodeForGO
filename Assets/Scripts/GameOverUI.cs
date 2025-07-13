using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _resultTextMesh;
        [SerializeField]
        private Color _winColor;
        [SerializeField]
        private Color _loseColor;
        [SerializeField]
        private Button _rematchButton;

        private void Start()
        {
            Hide();

            GameManager.Instance.OnGameWinnerEvent += GameManager_OnGameWinnerEvent;
            GameManager.Instance.OnGameRematchEvent += GameManager_OnGameRematchEvent; ;

            _rematchButton.onClick.AddListener(() => GameManager.Instance.RematchRpc());
        }

        private void GameManager_OnGameRematchEvent(object sender, System.EventArgs e)
        {
            Hide();
        }

        private void GameManager_OnGameWinnerEvent(object sender, GameManager.OnGameWinnerEventArguments e)
        {
            if (e.WinnerPlayerType == GameManager.Instance.PlayerType)
            {
                _resultTextMesh.color = _winColor;
                _resultTextMesh.text = "YOU WIN !!";
            }
            else
            {
                _resultTextMesh.color = _loseColor;
                _resultTextMesh.text = "YOU LOOSE !!";

            }
            Show();
        }

        private void Hide()
        {
            this.gameObject.SetActive(false);
        }
        private void Show()
        {
            this.gameObject.SetActive(true);
        }
    }
}
