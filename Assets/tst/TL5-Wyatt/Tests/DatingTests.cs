// Assets/tst/TL5-Wyatt/Tests/DatingTests.cs
// Tests for F6 - Dating System & Relationship Metagame.
// Covers the TL5 class diagram: the Suitor hierarchy, the ReceiveGift template
// method, RelationshipState thresholds, and DatingController's public surface.
//
// Naming follows Method_Condition_Result so a red test in CI names the failure.

using System.Collections.Generic;
using NUnit.Framework;

public class DatingTests
{
    private DatingController controller;

    [SetUp]
    public void SetUp()
    {
        controller = new DatingController();
        controller.EnterDatingPhase();
    }

    // =====================================================================
    // HAPPY PATH
    // =====================================================================

    // The binding test. Suitor is abstract, so instead of base-vs-subclass we
    // hold two different subclasses in base-typed variables: if the runtime type
    // decides, the same gift produces different results.
    [Test]
    public void BaseRef_ToDifferentSubclasses_UsesOverride()
    {
        Suitor matriarch = new Matriarch();
        Suitor friend    = new Friend();

        matriarch.ReceiveGift(Gift.ShrimpCracker);
        friend.ReceiveGift(Gift.ShrimpCracker);

        Assert.AreNotEqual(friend.State.Affection, matriarch.State.Affection);
    }

    // The second half of the pair: SelectReaction is overridden too, so the
    // reaction text differs even when the gift is identical.
    [Test]
    public void SelectReaction_DifferentSubclasses_ReturnsDifferentLines()
    {
        Suitor matriarch = new Matriarch();
        Suitor friend    = new Friend();

        string a = matriarch.ReceiveGift(Gift.Pearl).Line;
        string b = friend.ReceiveGift(Gift.Pearl).Line;

        Assert.AreNotEqual(a, b);
    }

    [Test]
    public void EnterDatingPhase_AfterCall_RosterHasFourSuitors()
    {
        Assert.AreEqual(4, controller.GetSuitors().Length);
    }

    // The template method must run every step, including the fixed Record step,
    // no matter which subclass supplied the overridable ones.
    [Test]
    public void ReceiveGift_AnySuitor_RecordsGiftInHistory()
    {
        Suitor husband = new Husband();

        husband.ReceiveGift(Gift.Pearl);
        husband.ReceiveGift(Gift.BloodVial);

        Assert.AreEqual(2, husband.State.GiftHistory.Count);
    }

    [Test]
    public void Advance_WhenThresholdMet_PublishesUnlockEvent()
    {
        List<RelationshipUnlock> published = new List<RelationshipUnlock>();
        controller.UnlockEarned += u => published.Add(u);

        controller.SelectSuitor(1);                  // the Matriarch
        controller.OfferGift(Gift.Heirloom);         // +10 affection
        controller.Advance();

        Assert.AreEqual(1, published.Count);
        Assert.AreEqual(RelationshipStage.Tolerating, published[0].Stage);
    }

    [Test]
    public void RestoreState_FromCapturedSnapshot_RoundTripsAffectionAndStage()
    {
        controller.SelectSuitor(1);
        controller.OfferGift(Gift.Heirloom);
        controller.Advance();

        Dictionary<string, RelationshipSnapshot> snapshot = controller.CaptureState();

        DatingController restored = new DatingController();
        restored.RestoreState(snapshot);

        Suitor before = controller.GetSuitors()[1];
        Suitor after  = restored.GetSuitors()[1];

        Assert.AreEqual(before.State.Affection, after.State.Affection);
        Assert.AreEqual(before.State.Stage, after.State.Stage);
    }

    // =====================================================================
    // FAILURE PATH
    // =====================================================================

    // Mirror of the BindingTests fail test: two instances of the SAME type have
    // nothing to distinguish them, so the same gift must give the same result.
    [Test]
    public void BaseRef_ToSameSubclass_NoBindingDifference()
    {
        Suitor one = new Friend();
        Suitor two = new Friend();

        one.ReceiveGift(Gift.Pearl);
        two.ReceiveGift(Gift.Pearl);

        Assert.AreEqual(one.State.Affection, two.State.Affection);
    }

    [Test]
    public void OfferGift_NullGift_ReturnsNullAndDoesNotThrow()
    {
        Assert.AreEqual(null, controller.OfferGift(null));
    }

    [Test]
    public void SelectSuitor_IndexOutOfRange_LeavesCurrentSuitorUnchanged()
    {
        controller.SelectSuitor(0);
        Suitor expected = controller.CurrentSuitor;

        controller.SelectSuitor(99);
        controller.SelectSuitor(-1);

        Assert.AreEqual(expected, controller.CurrentSuitor);
    }

    [Test]
    public void Advance_BelowThreshold_ReturnsFalseAndDoesNotChangeStage()
    {
        controller.SelectSuitor(2);                  // the Potential
        controller.OfferGift(Gift.Pearl);            // +2, threshold is 4

        Assert.AreEqual(false, controller.Advance());
        Assert.AreEqual(RelationshipStage.Hostile, controller.CurrentSuitor.State.Stage);
    }

    // =====================================================================
    // BOUNDARY
    // =====================================================================

    // One below the threshold must not advance; exactly on it must.
    [Test]
    public void Advance_ExactlyAtThreshold_AdvancesStage()
    {
        controller.SelectSuitor(2);                  // the Potential

        controller.OfferGift(Gift.Pearl);            // affection 2 - one short
        Assert.AreEqual(false, controller.Advance());

        controller.OfferGift(Gift.Pearl);            // affection 4 - exactly on it
        Assert.AreEqual(true, controller.Advance());
        Assert.AreEqual(RelationshipStage.Tolerating, controller.CurrentSuitor.State.Stage);
    }

    // An insulting gift gives a negative delta, but affection floors at zero.
    [Test]
    public void ReceiveGift_InsultingGift_ClampsAffectionAtZero()
    {
        Suitor matriarch = new Matriarch();

        matriarch.ReceiveGift(Gift.ShrimpCracker);   // -3
        matriarch.ReceiveGift(Gift.ShrimpCracker);   // -3

        Assert.AreEqual(0, matriarch.State.Affection);
    }

    [Test]
    public void Advance_AtBetrothed_CannotAdvanceFurther()
    {
        controller.SelectSuitor(3);                  // the Friend, +8 per heirloom
        for (int i = 0; i < 4; i++) controller.OfferGift(Gift.Heirloom);   // affection 32

        for (int i = 0; i < 4; i++) Assert.AreEqual(true, controller.Advance());

        Assert.AreEqual(RelationshipStage.Betrothed, controller.CurrentSuitor.State.Stage);
        Assert.AreEqual(false, controller.Advance());
    }
}
