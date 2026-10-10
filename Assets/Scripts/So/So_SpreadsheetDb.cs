using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpreadsheetDb_", menuName = "Scriptable Object Data/SpreadsheetDb")]
public class So_SpreadsheetDb : SpreadsheetContainerBase {
    [Sheet("AiCpConfig")]
    [SerializeField] List<DbRow_AiCpConfig> aiCpConfig;
    [Sheet("CpCommonConfig")]
    [SerializeField] List<DbRow_CpCommonConfig> cpCommonConfig;
    [Sheet("CpHumdConfig")]
    [SerializeField] List<DbRow_CpHumdConfig> cpHumdConfig;

    SheetLookup<DbRow_AiCpConfig> aiCpConfigLookup;
    SheetLookup<DbRow_CpCommonConfig> cpCommonConfigLookup;
    SheetLookup<DbRow_CpHumdConfig> cpHumdConfigLookup;

    public override void RebuildLookups() {
        aiCpConfigLookup = new SheetLookup<DbRow_AiCpConfig>(aiCpConfig);
        cpCommonConfigLookup = new SheetLookup<DbRow_CpCommonConfig>(cpCommonConfig);
        cpHumdConfigLookup = new SheetLookup<DbRow_CpHumdConfig> (cpHumdConfig);
    }

    public DbRow_AiCpConfig GetAiCpConfig(string id) {
        return aiCpConfigLookup.Get(id);
    }

    public DbRow_CpCommonConfig GetCpCommonConfig(string id) {
        return cpCommonConfigLookup.Get(id);
    }

    public DbRow_CpHumdConfig GetCpHumdConfig(string id) {
        return cpHumdConfigLookup.Get(id);
    }
}
