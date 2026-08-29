using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Behavior tree using native arrays.
/// </summary>
public class BtMgr : Singleton<BtMgr>{
    [Tooltip("How deep a tree branch can be at max.")]
    [SerializeField] int maxNodesPerTree = 10;

    /// <summary>
    /// All nodes of all capsule character trees. Nodes of one tree are put here
    /// sequentially, trees ordered in the order of capsule character ids.
    /// </summary>
    public NativeArray<BtNodeData> nodes;
    /// <summary>
    /// Node currently running for a behavior tree. Defaults to -1.
    /// </summary>
    public NativeArray<int> curRunningNode;

    NativeArray<bool> occupied;

    public void Init() {
        if (nodes.IsCreated || curRunningNode.IsCreated) {
            Debug.LogError("Data already created before Init!", this);
            return;
        }
        AllocateNodeStorage();
        AllocateRuntimeData();
    }

    void OnDestroy() {
        nodes.Dispose();
        curRunningNode.Dispose();
        occupied.Dispose();
    }

    // ------------------------------------------------------------------------------
    // Public Methods
    // ------------------------------------------------------------------------------

    /// <summary>
    /// NOTE: Uses capsule character id as index.
    /// </summary>
    public void Register(int cpId, So_BtRootNode bt) {
        //Debug.Log($"Registered character {capsuleCharId}", this);
        AddTree(cpId, bt);
        curRunningNode[cpId] = -1;
        occupied[cpId] = true;
    }

    public void Unregister(int cpId) {
        occupied[cpId] = false;
    }

    public void Tick() {
        for (int i = 0; i < occupied.Length; i++) {
            if (!occupied[i])
                continue;
            TickBt(i);
        }
    }

    // ------------------------------------------------------------------------------
    // Private Methods
    // ------------------------------------------------------------------------------

    void AddTree(int cpId, So_BtRootNode tree) {
        Debug.Log($"Adding bt for char {cpId}: {tree.name}", this);
        List<BtNodeData> nodeList = new List<BtNodeData>();
        CompileNode(tree.root, nodeList, -1);
        if (nodeList.Count > maxNodesPerTree) {
            Debug.LogError($"Tree had {nodeList.Count} nodes but max node count "
                + $"per tree is {maxNodesPerTree}", this);
            return;
        }
        for (int i = 0; i < nodeList.Count; i++)
            nodes[maxNodesPerTree * cpId + i] = nodeList[i];
    }

    void AllocateNodeStorage() {
        nodes = new NativeArray<BtNodeData>(
            CpMgr.inst.maxCps * maxNodesPerTree,
            Allocator.Persistent
        );
    }

    void AllocateRuntimeData() {
        curRunningNode = new NativeArray<int>(
            CpMgr.inst.maxCps,
            Allocator.Persistent
        );
        occupied = new NativeArray<bool>(
            CpMgr.inst.maxCps,
            Allocator.Persistent
        );
    }

    /// <summary>
    /// We compile nodes to a flat list. Each nodes subtree is contiguous.
    /// </summary>
    void CompileNode(So_BtNode node, List<BtNodeData> nodeList, int parent) {
        Debug.Assert(node != null, $"Scriptable object bt node ref was null!", this);
        int i = nodeList.Count;
        nodeList.Add(default);
        int firstChild = node.children.Length > 0 ? nodeList.Count : -1;
        int prevChild = -1;
        for (int j = 0; j < node.children.Length; j++) {
            int childI = nodeList.Count;
            CompileNode(node.children[j], nodeList, i);
            if(prevChild != -1) {
                BtNodeData prevData = nodeList[prevChild];
                prevData.nextSibling = childI;
                nodeList[prevChild] = prevData;
            }
            prevChild = childI;
        }
        nodeList[i] = new BtNodeData {
            childCount = node.children.Length,
            dataId = -1,
            firstChild = firstChild,
            nextSibling = -1,
            nodeName = node.name,
            parent = parent,
            t = node.t
        };
    }

    BtResult EvalLeaf(BtNodeT t, int cpId) {
        float2 horDesiredVel;
        Cp_BaseData ccMgr = CpMgr.inst.data;
        CpMgr caMgr = CpMgr.inst;
        Cp_BrainData brainData = CpMgr.inst.brainData;
        switch (t) {
            case BtNodeT.Cmd_Idle:
                ccMgr.input_atk_Heavy[cpId] = false;
                ccMgr.input_atk_Light[cpId] = false;
                ccMgr.input_atk_Ult[cpId] = false;
                ccMgr.input_dodge[cpId] = false;
                if(math.lengthsq(ccMgr.input_mov[cpId]) > caMgr.movInputSqrDeadzone)
                    ccMgr.input_mov_LastNonZero[cpId] = ccMgr.input_mov[cpId];
                ccMgr.input_mov[cpId] = float2.zero;
                return BtResult.Success;
            case BtNodeT.Cmd_Atk1:
                ccMgr.input_atk_Heavy[cpId] = false;
                ccMgr.input_atk_Light[cpId] = true;
                ccMgr.input_atk_Ult[cpId] = false;
                ccMgr.input_dodge[cpId] = false;
                if (math.lengthsq(ccMgr.input_mov[cpId]) > caMgr.movInputSqrDeadzone)
                    ccMgr.input_mov_LastNonZero[cpId] = ccMgr.input_mov[cpId];
                horDesiredVel = new float2(
                        brainData.agentDesiredVel[cpId].x,
                        brainData.agentDesiredVel[cpId].z
                );
                //Debug.Log($"{cpId} agent desired hor vel: {horDesiredVel}", this);
                // Agent can have 0 desired velocity, thus to avoid NaNs:
                if (math.lengthsq(horDesiredVel) > 0.0001f)
                    // Movement input should always be max 1 length.
                    ccMgr.input_mov[cpId] = math.normalize(horDesiredVel);
                return BtResult.Success;
            case BtNodeT.Cmd_MovToTgt:
                ccMgr.input_atk_Heavy[cpId] = false;
                ccMgr.input_atk_Light[cpId] = false;
                ccMgr.input_atk_Ult[cpId] = false;
                ccMgr.input_dodge[cpId] = false;
                if (math.lengthsq(ccMgr.input_mov[cpId]) > caMgr.movInputSqrDeadzone)
                    ccMgr.input_mov_LastNonZero[cpId] = ccMgr.input_mov[cpId];
                horDesiredVel = new float2(
                        brainData.agentDesiredVel[cpId].x,
                        brainData.agentDesiredVel[cpId].z
                );
                // Agent can have 0 desired velocity, thus to avoid NaNs:
                if (math.lengthsq(horDesiredVel) > 0.0001f)
                    // Movement input should always be max 1 length.
                    ccMgr.input_mov[cpId] = math.normalize(horDesiredVel);
                else
                    ccMgr.input_mov[cpId] = float2.zero;
                    //Debug.Log($"{cpId} BtNodeT.Cmd_MovToTgt movement input: {ccMgr.input_mov[cpId]}", this);
                return BtResult.Success;
            case BtNodeT.Cond_InAggroRange:
                // TODO: aggro and atk ranges should be calculated using navmesh path finding. Or maybe not.
                return brainData.inAggroRange[cpId] ? BtResult.Success : BtResult.Failure;
            case BtNodeT.Cond_InAtkRange:
                return brainData.inAtkRange[cpId] ? BtResult.Success : BtResult.Failure;
            case BtNodeT.Selector:
                Debug.LogError("Selector is a composite not a leaf!", this);
                return BtResult.Running;
            case BtNodeT.Sequence:
                Debug.LogError("Sequence is a composite not a leaf!", this);
                return BtResult.Running;
            default:
                Debug.LogError("Switch defaulted!", this);
                return BtResult.Running;
        }
    }

    /// <summary>
    /// Evaluate each node of a bt.
    /// </summary>
    // TODO: Create some optional debugging system that writes data about what nodes were
    // TODO C: visited and what they returned.
    void TickBt(int capsuleCharId) {
        int outerIterations = 0;
        int innerIterations = 0;
        int treeStart = capsuleCharId * maxNodesPerTree;
        int nodeI = curRunningNode[capsuleCharId] == -1
            ? treeStart
            : treeStart + curRunningNode[capsuleCharId];
        BtResult result = BtResult.Success;
        while (true) {
            if (++outerIterations > 100) {
                Debug.LogError($"{capsuleCharId} bt outer loop looped too long.", this);
                return;
            }
            BtNodeData node = nodes[nodeI];
            //Debug.Log($"Went to node {node.nodeName}");
            if (node.t == BtNodeT.Sequence || node.t == BtNodeT.Selector) {
                nodeI = treeStart + node.firstChild;
                continue;
            }
            result = EvalLeaf(node.t, capsuleCharId);
            if (result == BtResult.Running) {
                curRunningNode[capsuleCharId] = nodeI - treeStart;
                return;
            }
            while (true) {
                if (++innerIterations > 100) {
                    Debug.LogError($"{capsuleCharId} bt inner loop looped too long.", this);
                    return;
                }
                if (node.parent == -1) {
                    curRunningNode[capsuleCharId] = -1;
                    return;
                }
                BtNodeData parent = nodes[treeStart + node.parent];
                bool parentFinished
                    = parent.t == BtNodeT.Sequence
                    && result == BtResult.Failure
                    || parent.t == BtNodeT.Selector
                    && result == BtResult.Success;
                if (parentFinished) {
                    nodeI = treeStart + node.parent;
                    node = parent;
                    continue;
                }
                if (node.nextSibling != -1) {
                    nodeI = treeStart + node.nextSibling;
                    break;
                }
                result = parent.t == BtNodeT.Sequence ? BtResult.Success : BtResult.Failure;
                nodeI = treeStart + node.parent;
                node = parent;
            }
        }
    }
}
