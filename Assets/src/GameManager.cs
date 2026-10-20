using UnityEngine;

public class GameManager : MonoBehaviour
{
    private RunState runState;
    private RunSession session;
    private FrameContext frame;
    private PlayerStats player;

    private WeaponBase[] weapons;
    private Summon[] summons;
    private Pickup[] pickups;
    private StatModifier[] modifiers;
    private SortAlgorithm sorter;

    private int weaponIndex;
    private int summonIndex;
    private int pickupIndex;
    private int modifierIndex;

    private int wave = 1;
    private int enemyHealth = 40;
    private int score;
    private string runMessage;
    private string lastAction = "Defeat the enemy to clear the wave.";

    private void Start()
    {
        runState = new PlayingState();
        session = new RunSession();
        frame = new FrameContext();
        player = new PlayerStats();

        weapons = new WeaponBase[]
        {
            new PistolWeapon(
                new WeaponData { weaponName = "Pistol", magazineSize = 12 },
                0.25f),

            new RifleWeapon(
                new WeaponData { weaponName = "Rifle", magazineSize = 30 },
                0.5f),

            new ShotgunWeapon(
                new WeaponData { weaponName = "Shotgun", magazineSize = 8 },
                8),

            new LauncherWeapon(
                new WeaponData { weaponName = "Launcher", magazineSize = 1 },
                null)
        };

        summons = new Summon[]
        {
            new OrbitingSummon(),
            new HuntingSummon(),
            new TotemSummon(),
            new RevengeSummon()
        };

        pickups = new Pickup[]
        {
            new XPPickup(),
            new ShardPickup(),
            new HealthPickup(),
            new OtherPickup()
        };

        modifiers = new StatModifier[]
        {
            new PercentModifier(),
            new FlatModifier()
        };

        sorter = new SortAscending();
        UpdateRunMessage();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(
            new Rect(20, 20, 420, Screen.height - 40),
            GUI.skin.box);

        GUILayout.Label("BINDING ARENA");
        GUILayout.Label("Wave: " + wave + "    Score: " + score);
        GUILayout.Label("Enemy HP: " + enemyHealth);
        GUILayout.Label("Player HP: " + player.Health + "/" + player.MaxHealth);
        GUILayout.Label("XP: " + player.XP + "    Shards: " + player.Shards);
        GUILayout.Label("Run: " + runMessage);
        GUILayout.Space(10);
        GUILayout.Label(lastAction);

        if (GUILayout.Button("Fire: " + weapons[weaponIndex].GetType().Name))
            FireWeapon();

        if (GUILayout.Button("Swap Weapon"))
            weaponIndex = (weaponIndex + 1) % weapons.Length;

        if (GUILayout.Button("Use: " + summons[summonIndex].GetType().Name))
            UseSummon();

        if (GUILayout.Button("Collect: " + pickups[pickupIndex].GetType().Name))
            CollectPickup();

        if (GUILayout.Button("Next Pickup"))
            pickupIndex = (pickupIndex + 1) % pickups.Length;

        if (GUILayout.Button("Switch Modifier"))
        {
            modifierIndex = (modifierIndex + 1) % modifiers.Length;
            lastAction = "Selected " + modifiers[modifierIndex].GetType().Name;
        }

        if (GUILayout.Button("Sort"))
            lastAction = sorter.GetType().Name + " -> " + sorter.Sort();

        if (GUILayout.Button("Pause / Resume"))
            ToggleRunState();

        GUILayout.EndArea();
    }

    private void FireWeapon()
    {
        if (!(runState is PlayingState))
        {
            lastAction = "The run is paused.";
            return;
        }

        WeaponBase weapon = weapons[weaponIndex];
        weapon.Fire();

        int damage = Mathf.Max(
            1,
            Mathf.RoundToInt(modifiers[modifierIndex].Apply(10f)));

        DamageEnemy(damage);
        lastAction = weapon.GetType().Name + " hit for " + damage + " damage.";
        UpdateRunMessage();
    }

    private void UseSummon()
    {
        if (!(runState is PlayingState))
        {
            lastAction = "The run is paused.";
            return;
        }

        Summon summon = summons[summonIndex];
        summon.Activate();

        SummonContext context = new SummonContext(
            Vector2.zero,
            "Enemy-" + wave,
            true);

        summon.UpdateBehavior(context);
        DamageEnemy(10);

        lastAction = summon.LastBehavior;
        summonIndex = (summonIndex + 1) % summons.Length;
        UpdateRunMessage();
    }

    private void CollectPickup()
    {
        lastAction = pickups[pickupIndex].Collect(player);
        pickupIndex = (pickupIndex + 1) % pickups.Length;
    }

    private void DamageEnemy(int damage)
    {
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            score += 100;
            wave++;
            enemyHealth = 40 + (wave - 1) * 10;
            lastAction += " Wave cleared! Next enemy incoming.";
        }
    }

    private void ToggleRunState()
    {
        runState = runState is PlayingState
            ? (RunState)new BaseOnlyState()
            : new PlayingState();

        UpdateRunMessage();
    }

    private void UpdateRunMessage()
    {
        runMessage = runState.Tick(session, frame);
    }
}