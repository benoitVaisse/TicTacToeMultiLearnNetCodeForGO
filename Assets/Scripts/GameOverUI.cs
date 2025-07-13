using TMPro;
using UnityEngine;

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

        private void Start()
        {
            Hide();

            GameManager.Instance.OnGameWinnerEvent += GameManager_OnGameWinnerEvent;
        }

        private void GameManager_OnGameWinnerEvent(object sender, GameManager.OnGameWinnerEventArguments e)
        {
            Debug.Log($"TestWinner {e.WinnerPlayerType}");
            Debug.Log($"TestWinner {GameManager.Instance.PlayerType}");
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
