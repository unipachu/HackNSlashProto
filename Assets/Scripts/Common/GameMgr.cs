// NOTE: Make sure singleton execution order is so that the singletons are Awoken before
// NOTE C: a dependent singleton manager is awoken.
public class GameMgr : Singleton<GameMgr>{
    override protected void Awake(){
        base.Awake();
        CapsuleCharMgr.inst.Init();
        BtMgr.inst.Init();
    }

    void Update() {
        // TODO: Tick CapsuleCharMgr here too.
        BtMgr.inst.Tick();
    }
}
