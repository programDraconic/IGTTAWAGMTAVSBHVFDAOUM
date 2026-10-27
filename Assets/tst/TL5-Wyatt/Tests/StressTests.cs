using System.Diagnostics;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

public class StressTests
{
    private const double FrameBudgetMs = 16.67;
    private const int StartingBatch = 256;
    private const int MaxBatch = 1 << 21;

    [Test]
    public void Stress_GiftThroughput_ReportsBreakingPoint()
    {
        DatingController controller = new DatingController();
        controller.EnterDatingPhase();
        Suitor[] suitors = controller.GetSuitors();

        int[] given = new int[suitors.Length];
        int batch = StartingBatch;
        int largestWithinBudget = 0;
        double elapsedAtBreak = 0;
        int breakingBatch = 0;

        while (batch <= MaxBatch)
        {
            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < batch; i++)
            {
                int target = i % suitors.Length;
                controller.SelectSuitor(target);
                controller.OfferGift(Gift.ShrimpCracker);
                given[target]++;
            }
            watch.Stop();

            double ms = watch.Elapsed.TotalMilliseconds;
            if (ms > FrameBudgetMs)
            {
                breakingBatch = batch;
                elapsedAtBreak = ms;
                break;
            }

            largestWithinBudget = batch;
            batch *= 2;
        }

        Debug.Log("[TL5 stress] largest gift batch inside a 16.67 ms frame: " + largestWithinBudget +
                  "  |  broke at " + breakingBatch + " gifts in " + elapsedAtBreak.ToString("F2") + " ms" +
                  "  |  total gifts processed: " + Total(given));

        for (int i = 0; i < suitors.Length; i++)
        {
            Assert.AreEqual(given[i], suitors[i].State.GiftHistory.Count);
            Assert.AreEqual(true, suitors[i].State.Affection >= 0);
        }

        Assert.AreEqual(true, largestWithinBudget >= StartingBatch);
    }

    private static int Total(int[] counts)
    {
        int sum = 0;
        for (int i = 0; i < counts.Length; i++) sum += counts[i];
        return sum;
    }
}
