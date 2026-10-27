using System.Collections.Generic;

public enum GiftKind { Trinket, Delicacy, Blood, Heirloom }

public class Gift
{
    public readonly string Id;
    public readonly string DisplayName;
    public readonly GiftKind Kind;
    public readonly int Value;

    public Gift(string id, string displayName, GiftKind kind, int value)
    {
        Id = id;
        DisplayName = displayName;
        Kind = kind;
        Value = value;
    }

    public static readonly Gift ShrimpCracker = new Gift("cracker", "Shrimp Cracker", GiftKind.Trinket, 1);
    public static readonly Gift Pearl         = new Gift("pearl",   "Pearl",          GiftKind.Delicacy, 3);
    public static readonly Gift BloodVial     = new Gift("blood",   "Blood Vial",     GiftKind.Blood,    4);
    public static readonly Gift Heirloom      = new Gift("heirloom","Family Heirloom",GiftKind.Heirloom, 6);

    public static IReadOnlyList<Gift> Catalogue
    {
        get { return new List<Gift> { ShrimpCracker, Pearl, BloodVial, Heirloom }; }
    }

    public override string ToString() { return DisplayName; }
}
