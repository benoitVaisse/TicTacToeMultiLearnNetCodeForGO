using TMPro;
using UnityEngine;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject _crossArrowGameObject;
        [SerializeField]
        private GameObject _circleArrowGameObject;
        [SerializeField]
        private GameObject _crossYouTextMesh;
        [SerializeField]
        private GameObject _circleYouTextMesh;

        [SerializeField]
        private TextMeshProUGUI _scoreCrossPlayer;
        [SerializeField]
        private TextMeshProUGUI _scoreCirclePlayer;

        private void Awake()
        {
            _crossArrowGameObject.SetActive(false);
            _circleArrowGameObject.SetActive(false);
            _crossYouTextMesh.SetActive(false);
            _circleYouTextMesh.SetActive(false);
            _scoreCrossPlayer.text = "";
            _scoreCirclePlayer.text = "";
        }

        private void Start()
        {
            GameManager.Instance.OnGameStartedEvent += GameManager_OnGameStarted;
            GameManager.Instance.OnSwitchCurrentPlayablePlayerEvent += GameManager_OnSwitchCurrentPlayablePlayerEvent;
            GameManager.Instance.OnScorePLayerChangeEvent += GameManager_OnScorePLayerChangeEvent;
        }

        private void GameManager_OnScorePLayerChangeEvent(object sender, System.EventArgs e)
        {
            PlayerScores scores = GameManager.Instance.Scoreplayer;
            _scoreCrossPlayer.text = scores.CrossPlayerScore.ToString();
            _scoreCirclePlayer.text = scores.CirclePlayerScore.ToString();
        }

        private void GameManager_OnSwitchCurrentPlayablePlayerEvent(object sender, System.EventArgs e)
        {
            UpdateUserPlayableArrow();
        }

        private void GameManager_OnGameStarted(object sender, System.EventArgs e)
        {
            if (GameManager.Instance.PlayerType == PlayerType.Cross)
            {
                _crossYouTextMesh.SetActive(true);
            }
            else
            {
                _circleYouTextMesh.SetActive(true);
            }
            _scoreCrossPlayer.text = "0";
            _scoreCirclePlayer.text = "0";
            UpdateUserPlayableArrow();
        }

        private void UpdateUserPlayableArrow()
        {
            Debug.Log($"UpdateUserPlayableArrow playertype {GameManager.Instance.PlayerType} {GameManager.Instance.CurrentPlayanlePlayerType}");
            if (GameManager.Instance.CurrentPlayanlePlayerType == PlayerType.Cross)
            {
                _crossArrowGameObject.SetActive(true);
                _circleArrowGameObject.SetActive(false);
            }
            else
            {
                _crossArrowGameObject.SetActive(false);
                _circleArrowGameObject.SetActive(true);
            }
        }
    }
}
