namespace Utils
{
    public delegate void CallbackAction(ICallbacker callbacker);
    public interface ICallbacker
    {
        public event CallbackAction Callback;
    }
}
