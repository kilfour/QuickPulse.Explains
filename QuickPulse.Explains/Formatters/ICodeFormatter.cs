
namespace QuickPulse.Explains.Formatters;

public interface ICodeFormatter
{
    IEnumerable<string> Format(IEnumerable<string> lines);
}
