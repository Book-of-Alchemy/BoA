
public abstract class Artifact 
{
    protected StatModifier modifier;
    public ArtifactData data;

    public Artifact(ArtifactData data)
    { this.data = data; }

    public virtual void Equip(PlayerStats player) { }
    public virtual void UnEquip(PlayerStats player) { }

}
