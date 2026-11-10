using NUnit.Framework;
using TL6.PlayerCore;

public class PlayerHealthTests
{
    [Test]
    public void Damage_UsesArmourAndUpdatesTheProvidedStats()
    {
        var stats = new PlayerStats { Health = 100, MaxHealth = 100, Armor = 3 };
        var health = new PlayerHealth(stats);

        Assert.That(health.ApplyDamage(10), Is.EqualTo(7));
        Assert.That(stats.Health, Is.EqualTo(93));
        Assert.That(health.CurrentHP, Is.EqualTo(stats.Health));
    }

    [Test]
    public void Protection_BlocksRepeatedHitsUntilEnoughTimePasses()
    {
        var stats = new PlayerStats { Health = 100 };
        var health = new PlayerHealth(stats, 0.5f);

        health.ApplyDamage(10);
        Assert.That(health.ApplyDamage(10), Is.Zero);
        health.Tick(0f);
        health.Tick(-1f);
        Assert.That(health.ApplyDamage(10), Is.Zero);
        health.Tick(0.25f);
        Assert.That(health.ApplyDamage(10), Is.Zero);
        health.Tick(0.25f);
        Assert.That(health.ApplyDamage(10), Is.EqualTo(10));
        Assert.That(stats.Health, Is.EqualTo(80));
    }

    [Test]
    public void ArmourFullyBlocksHit_WithoutConsumingProtection()
    {
        var stats = new PlayerStats { Health = 100, Armor = 10 };
        var health = new PlayerHealth(stats);

        Assert.That(health.ApplyDamage(5), Is.Zero);
        Assert.That(health.IsInvulnerable, Is.False);
        Assert.That(health.ApplyDamage(15), Is.EqualTo(5));
    }

    [Test]
    public void NonPositiveDamageAndHealing_DoNotChangeHealth()
    {
        var stats = new PlayerStats { Health = 50 };
        var health = new PlayerHealth(stats);

        Assert.That(health.ApplyDamage(-20), Is.Zero);
        Assert.That(health.ApplyDamage(0), Is.Zero);
        Assert.That(health.Heal(-20), Is.Zero);
        Assert.That(health.Heal(0), Is.Zero);
        Assert.That(stats.Health, Is.EqualTo(50));
    }

    [Test]
    public void Healing_ClampsToTheLiveMaximumAndHandlesLargeAmounts()
    {
        var stats = new PlayerStats { Health = 90, MaxHealth = 100 };
        var health = new PlayerHealth(stats);

        Assert.That(health.Heal(int.MaxValue), Is.EqualTo(10));
        stats.MaxHealth = 120;
        Assert.That(health.Heal(20), Is.EqualTo(20));
        Assert.That(stats.Health, Is.EqualTo(120));
    }

    [Test]
    public void Death_ClampsHealthAndFiresOnceUntilANewRun()
    {
        var stats = new PlayerStats { Health = 10, MaxHealth = 100 };
        var health = new PlayerHealth(stats, 0f);
        int deaths = 0;
        health.PlayerDied += () => deaths++;

        Assert.That(health.ApplyDamage(100), Is.EqualTo(10));
        Assert.That(stats.Health, Is.Zero);
        health.ApplyDamage(100);
        Assert.That(health.Heal(20), Is.Zero);
        Assert.That(deaths, Is.EqualTo(1));

        health.Initialise();
        Assert.That(stats.Health, Is.EqualTo(100));
        health.ApplyDamage(100);
        Assert.That(deaths, Is.EqualTo(2));
    }

    [Test]
    public void ArmourUpgrades_AreReadOnTheNextHit()
    {
        var stats = new PlayerStats { Health = 100, Armor = 0 };
        var health = new PlayerHealth(stats, 0f);

        health.ApplyDamage(10);
        stats.Armor = 3;
        Assert.That(health.ApplyDamage(10), Is.EqualTo(7));
        Assert.That(stats.Health, Is.EqualTo(83));
    }
}
