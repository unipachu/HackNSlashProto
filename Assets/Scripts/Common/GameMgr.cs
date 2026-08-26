// NOTE: Make sure singleton execution order is so that the singletons are Awoken before
// NOTE C: a dependent singleton manager is awoken.
using UnityEngine;

public class GameMgr : Singleton<GameMgr>{
    override protected void Awake(){
        base.Awake();
        HandEquippableMgr.inst.Init();
        CapsuleCharMgr.inst.Init();
        BtMgr.inst.Init();
    }

    void Update() {
        // TODO: Tick CapsuleCharMgr here too.
        BtMgr.inst.Tick();
        CapsuleCharMgr.inst.Tick(Time.deltaTime);
    }
}
