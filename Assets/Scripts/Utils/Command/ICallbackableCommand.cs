namespace Utils
{
    public interface ICallbackableCommand : ICommand
    {
        public void OnCallbackAction(ICallbacker callbacker);
    }
}
