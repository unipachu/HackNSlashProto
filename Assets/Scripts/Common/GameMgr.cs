// NOTE: Make sure singleton execution order is so that the singletons are Awoken before
// NOTE C: a dependent singleton manager is awoken.
using UnityEngine;

public class GameMgr : Singleton<GameMgr>{
    override protected void Awake(){
        base.Awake();
        CpMgr.inst.Init();
        BtMgr.inst.Init();
    }

    void FixedUpdate() {
        CpMgr.inst.FixedTick();
    }
    
    void Update() {
        // TODO: Tick CapsuleCharMgr here too.
        BtMgr.inst.Tick();
        CpMgr.inst.Tick(Time.deltaTime);
    }

    void LateUpdate() {
        CpMgr.inst.LateTick();
    }
}
