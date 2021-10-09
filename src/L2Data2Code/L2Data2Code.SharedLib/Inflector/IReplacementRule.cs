namespace L2Data2Code.SharedLib.Inflector
{
    public interface IReplacementRule : IRuleApplier
    {
        string Replacement { get; }
        string Pattern { get; }
    }
}