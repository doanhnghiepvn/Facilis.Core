namespace Facilis.Core.Abstractions
{
    public interface IOperators
    {
        string GetSystemOperatorName();

        string GetCurrentOperatorName();
    }

    public class Operator : IOperators
    {
        public string SystemOperatorName { get; set; }
        public string CurrentOperatorName { get; set; }

        public string GetCurrentOperatorName() => this.CurrentOperatorName;

        public string GetSystemOperatorName() => this.SystemOperatorName;
    }
}