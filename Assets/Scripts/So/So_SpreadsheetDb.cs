using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpreadsheetDb_",
    menuName = "Scriptable Object Data/SpreadsheetDb")]
public class So_SpreadsheetDb : SpreadsheetContainerBase {
    [Sheet("CpCommonConfig")]
    [SerializeField] List<DbRow_CpCommonConfig> cpCommonConfig;
    [Sheet("CpHumdConfig")]
    [SerializeField] List<DbRow_CpHumdConfig> cpHumdConfig;

    SheetLookup<DbRow_CpCommonConfig> cpCommonConfigLookup;
    SheetLookup<DbRow_CpHumdConfig> cpHumdConfigLookup;

    public override void RebuildLookups() {
        cpCommonConfigLookup = new SheetLookup<DbRow_CpCommonConfig>(cpCommonConfig);
        cpHumdConfigLookup = new SheetLookup<DbRow_CpHumdConfig> (cpHumdConfig);
    }

    public DbRow_CpCommonConfig GetCpCommonConfig(string id) {
        return cpCommonConfigLookup.Get(id);
    }

    public DbRow_CpHumdConfig GetCpHumdConfig(string id) {
        return cpHumdConfigLookup.Get(id);
    }
}
