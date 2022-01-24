using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace InitBootSystem
{
    public class PreGameState : GameState
    {
        private readonly string mainMenuSceneName = "MainMenu";

        public override void LoadState()
        {

            var mainSceneLoading = SceneManager.LoadSceneAsync(mainMenuSceneName, LoadSceneMode.Additive);
            mainSceneLoading.completed += OnSceneLoading;
        }

        private void OnSceneLoading(UnityEngine.AsyncOperation obj)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainMenuSceneName));
        }

        private void OnSceneLoaded(Scene scene)
        {
            SceneManager.SetActiveScene(scene);
            return;
        }

        public override void ChangeState(GameState state)
        {
            gameManager.ChangeGameState(state);
        }

    }
}
