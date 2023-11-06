namespace TorunLive.Application.Interfaces.Parsers
{
    public interface IParser<T> where T : class
    {
        public T Parse(string data);
    }
}
