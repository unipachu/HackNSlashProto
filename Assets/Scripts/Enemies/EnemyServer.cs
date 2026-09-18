using System.Collections.Generic;
using UnityEngine;

// TODO: Enemy wave manager basically. 
public class EnemyServer : Singleton<EnemyServer>{
    [SerializeField] So_NpcSetup hammerEnemyConfig;
    [SerializeField] So_NpcSetup pistolEnemyConfig;
    [SerializeField] Transform[] spawnPoints;

    public void SpawnEnemies() {
        for (int i = 0; i < spawnPoints.Length; i++) {
            AiCtrl aiCtrl = new AiCtrl();
            CpRegisterer cp;
            BtNode bt;
            if (i%2 == 0) {
                cp = Instantiate(hammerEnemyConfig.cpPrefab);
                CpMgr.inst.Register(aiCtrl, cp);
                bt = CreateNode(hammerEnemyConfig.btNodeConfig, cp, aiCtrl);
            }
            else {
                cp = Instantiate(pistolEnemyConfig.cpPrefab);
                CpMgr.inst.Register(aiCtrl, cp);
                bt = CreateNode(pistolEnemyConfig.btNodeConfig, cp, aiCtrl);
            }
            AiCtrlMgr.inst.Register(aiCtrl, bt);
            cp.transform.SetPositionAndRotation(spawnPoints[i].position, spawnPoints[i].rotation);
            //PlayerSpawner.SpawnAiCpAtSpawnPt()
        }
    }

    public BtNode CreateNode(BtNodeConfig nodeConfig, CpRegisterer cp, AiCtrl aiCtrl) {
        List<BtNode> children = new();
        switch (nodeConfig.t) {
            case BtNodeT.Cmd_Atk1:
                return new BtNode_Cmd_Atk1(cp, aiCtrl);
            case BtNodeT.Cmd_Idle:
                return new BtNode_Cmd_Idle(cp, aiCtrl);
            case BtNodeT.Cmd_MovToTgt:
                return new BtNode_MovToTgt(cp, aiCtrl);
            case BtNodeT.Cond_InAggroRange:
                return new BtNode_InAggroRange(cp);
            case BtNodeT.Cond_InAtkRange:
                return new BtNode_Cond_InAtkRange(cp);
            case BtNodeT.Selector:
                for (int i = 0; i < nodeConfig.children.Count; i++)
                    children.Add(CreateNode(nodeConfig.children[i], cp, aiCtrl));
                return new BtNode_Selector(nodeConfig.dbgName, children);
            case BtNodeT.Sequence:
                for (int i = 0; i < nodeConfig.children.Count; i++) 
                    children.Add(CreateNode(nodeConfig.children[i], cp, aiCtrl));
                return new BtNode_Sequence(nodeConfig.dbgName, children);
            default:
                Debug.LogError($"Switch defaulted with: {nodeConfig.t}.", this);
                return null;
        }
    }
}
