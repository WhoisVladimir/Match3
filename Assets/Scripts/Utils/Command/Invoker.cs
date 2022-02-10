namespace Utils
{
    public static class Invoker
    {
        private static bool isInProgress;
        public static void StartCommand(ICommand command)
        {
            if (isInProgress) return;
            isInProgress = true;
            command.Execute();
        }

        public static void AddCommand(ICommand command)
        {
            command.Execute();
        }

        public static void AddEndedCommand(ICommand command)
        {
            command.Execute();
            isInProgress = false;
        }
    }
}
