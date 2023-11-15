namespace TorunLive.Application.Interfaces.Adapters
{
    public interface IAdapter<T> where T : class
    {
        public T Adapt(string data);
    }
}
