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

        private void Awake()
        {
            _crossArrowGameObject.SetActive(false);
            _circleArrowGameObject.SetActive(false);
            _crossYouTextMesh.SetActive(false);
            _circleYouTextMesh.SetActive(false);
        }

        private void Start()
        {
            GameManager.Instance.OnGameStartedEvent += GameManager_OnGameStarted;
            GameManager.Instance.OnSwitchCurrentPlayablePlayerEvent += GameManager_OnSwitchCurrentPlayablePlayerEvent; ;
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
