public class SortAlgorithm
{
    protected int[] data = { 5, 2, 9, 1, 7 };


    public string Sort()
    {
        ProcessArray();
        return ReturnArray();
    }


    public virtual int Compare(int left, int right)
    {
        return 0;
    }


    public void ProcessArray()
    {
        for (int i = 0; i < data.Length - 1; i++)
            for (int j = 0; j < data.Length - 1 - i; j++)
                if (Compare(data[j], data[j + 1]) > 0)
                {
                    int tmp = data[j];
                    data[j] = data[j + 1];
                    data[j + 1] = tmp;
                }
    }


    public string ReturnArray()
    {
        return string.Join(", ", data);
    }
}
