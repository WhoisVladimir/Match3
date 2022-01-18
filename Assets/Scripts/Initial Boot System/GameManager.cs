namespace InitBootSystem
{
    public class GameManager : Singleton<GameManager>
    {
        public GameState PreGame { get; private set; }
        public GameState Gameplay { get; private set; }
        public GameState Paused { get; private set; }
        public GameState PreExit { get; private set; }

        public GameState CurrentState { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            PreGame = new PreGameState();
            Gameplay = new GameplayState();
            Paused = new PausedState();
            PreExit = new PreExitState();
        }

        private void Start()
        {
            PreGame.LoadState();
        }
        public void ChangeGameState(GameState gameState)
        {
            gameState.LoadState();
            CurrentState = gameState;
        }
    }
}
