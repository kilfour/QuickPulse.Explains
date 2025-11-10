namespace QuickPulse.Explains.Monastery.Fragments.Tables;

public record TableFragment(IEnumerable<string> Headers, IEnumerable<RowFragment> Body) : Fragment;

