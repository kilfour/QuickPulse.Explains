namespace QuickPulse.Explains.Monastery.Fragments.Tables;

public record RowFragment(FirstCellFragment FirstCell, IEnumerable<CellFragment> Cells) : Fragment;

