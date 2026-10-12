

public class SortDescending : SortAlgorithm
{
    public override int Compare(int left, int right)
    {
        return right - left;
    }
}
