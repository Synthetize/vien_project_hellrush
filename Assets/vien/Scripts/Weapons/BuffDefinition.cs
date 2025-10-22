using UnityEngine;

[CreateAssetMenu(menuName="Weapons/Simple/Buff")]
public class BuffDefinition : ScriptableObject {
    public string buffId = "overclock";
    public float duration = 10f;
    public StatMod[] modifiers;
    // Politica ultra-semplice: se il buff è già attivo, si rinnova la durata.
}