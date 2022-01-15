using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace InitBootSystem
{
    public class PreGameState : GameState
    {
        private readonly string mainMenuSceneName = "MainMenu";
        //to do:
        //загрузка сцены "Главное меню"
        //переход к состоянию игры
        //переход к состоянию выхода

        public override void LoadState()
        {
            //to do:
            //асинхронно загрузить главное меню
            //гм должен остаться в своей сцене, но активной будет загруженная

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
