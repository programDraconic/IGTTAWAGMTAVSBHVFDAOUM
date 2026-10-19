// Contract other teams implement (Weapons, Enemies, Summons...) so 1.0 can
// tick them without knowing what they are.
public interface IGameplayFeature
{
    bool Initialize(object context);   // context = the RunSession
    void Tick(FrameContext frame);
    void Shutdown();
}
