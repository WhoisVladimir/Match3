namespace InitBootSystem
{
    public abstract class GameState
    {
        protected readonly GameManager gameManager = GameManager.Instance;

        public abstract void LoadState();
        public abstract void ChangeState(GameState state);
    }
}
