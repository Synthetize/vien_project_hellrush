public enum StatOp { Add, Mul }

[System.Serializable]
public struct StatMod {
    public string stat;  // "damage", "fireRate", "reloadTime", "magazineSize"
    public StatOp op;    // Add o Mul
    public float value;  // es: +5 (Add) or 1.2 (Mul)
}