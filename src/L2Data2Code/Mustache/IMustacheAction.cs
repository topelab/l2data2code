namespace Mustache
{
    internal interface IMustacheAction
    {
        void Initialize(MustacheOptions options);
        void Run();
    }
}