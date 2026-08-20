using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// Behavior tree using native arrays.
/// </summary>
public class BtMgr : Singleton<BtMgr>{
    [Tooltip("How deep a tree branch can be at max.")]
    [SerializeField] int maxBtDepth = 50;

    [Header("Refs")]
    CapsuleCharMgr capsuleCharMgr;
    
    So_BtRootNode[] bts;
    public NativeArray<BtDef> defs;
    public NativeArray<BtNodeData> nodes;
    public NativeArray<BtSt> btSts;
    public NativeArray<int> exeStack;
    //// NOTE: Basically only used for debugging.
    //public NativeArray<bool> occupied;

    override protected void Awake() {
        base.Awake();
        CompileTrees();
        AllocateRuntimeData();
    }

    void OnDestroy() {
        defs.Dispose();
        nodes.Dispose();
        btSts.Dispose();
        exeStack.Dispose();
        //occupied.Dispose();
    }

    void AllocateRuntimeData() {
        btSts = new NativeArray<BtSt>(capsuleCharMgr.maxCapsuleChars, Allocator.Persistent);
        exeStack = new NativeArray<int>(
            capsuleCharMgr.maxCapsuleChars * maxBtDepth,
            Allocator.Persistent
        );
        //occupied = new NativeArray<bool>(capsuleCharMgr.maxCapsuleChars, Allocator.Persistent);
    }
    
    void CompileNode(So_BtNode node, List<BtNodeData> nodeList) {
        int i = nodeList.Count;
        nodeList.Add(default);
        BtNodeData data = new BtNodeData {
            t = node.t,
            firstChild = -1,
            childCount = 0,
            dataId = -1
        };
    }
    
    void CompileTrees() {
        List<BtNodeData> nodeList = new List<BtNodeData>();
        List<BtDef> defList = new List<BtDef>();
        foreach (So_BtRootNode tree in bts) {
            int start = nodeList.Count;
            CompileNode(tree.root, nodeList);
            defList.Add(new BtDef {
                nodeStart = start,
                nodeCount = nodeList.Count - start
            });
        }
        nodes = new NativeArray<BtNodeData>(nodeList.Count, Allocator.Persistent);
        defs = new NativeArray<BtDef>(defList.Count, Allocator.Persistent);
        for (int i = 0; i < nodeList.Count; i++)
            nodes[i] = nodeList[i];
        for (int i = 0; i < defList.Count; i++)
            defs[i] = defList[i];
    }

    // ------------------------------------------------------------------------------
    // Public Methods
    // ------------------------------------------------------------------------------

    /// <summary>
    /// NOTE: Uses capsule character id as index.
    /// </summary>
    public void Register(int capsuleCharId) {
        //if (occupied[capsuleCharId]) {
        //    Debug.LogError($"Id {capsuleCharId} already registered!");
        //    return;
        //}
        //occupied[capsuleCharId] = true;
        btSts[capsuleCharId] = new BtSt {
            curNode = 0,
            stackStart = capsuleCharId * maxBtDepth,
            stackCount = 0
        };
    }

    public void Tick() {
        var capsuleCharDatas = capsuleCharMgr.capsuleCharDatas;
        var occupied = capsuleCharMgr.occupied;
        for (int i = 0; i < occupied.Length; i++) {
            if (occupied[i]) {
                // TODO: Loop over behavior trees
            }
        }
    }

    //public void Unregister(int capsuleCharId) {
    //    Debug.Assert(occupied[capsuleCharId], $"Id has not been registered: {capsuleCharId}!", this);
    //    occupied[capsuleCharId] = false;
    //}
}
