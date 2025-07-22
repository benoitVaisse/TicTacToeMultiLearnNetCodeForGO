using UnityEngine;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _placeSFx;
        [SerializeField]
        private Transform _winSFx;
        [SerializeField]
        private Transform _loseSFx;

        private void Start()
        {
            GameManager.Instance.OnPlaceSoundFXEvent += GameManager_OnPlaceSoundFXEvent;
            GameManager.Instance.OnGameWinnerEvent += GameManager_OnGameWinnerEvent; ;
        }

        private void GameManager_OnGameWinnerEvent(object sender, GameManager.OnGameWinnerEventArguments e)
        {
            if (e.WinnerPlayerType == GameManager.Instance.PlayerType)
            {
                Transform sound = Instantiate(_winSFx);
                Destroy(sound.gameObject, 5f);
            }
            else
            {
                Transform sound = Instantiate(_loseSFx);
                Destroy(sound.gameObject, 5f);
            }
        }

        private void GameManager_OnPlaceSoundFXEvent(object sender, System.EventArgs e)
        {
            Transform sound = Instantiate(_placeSFx);
            Destroy(sound.gameObject, 5f);
        }
    }
}
