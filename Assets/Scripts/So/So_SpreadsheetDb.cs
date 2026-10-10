using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpreadsheetDb_",
    menuName = "Scriptable Object Data/SpreadsheetDb")]
public class So_SpreadsheetDb : SpreadsheetContainerBase {
    [Sheet("CpCommonConfig")]
    [SerializeField]
    List<DbRow_CpCommonConfig> cpCommonConfig;

    SheetLookup<DbRow_CpCommonConfig> cpCommonConfigLookup;

    public override void RebuildLookups() {
        cpCommonConfigLookup = new SheetLookup<DbRow_CpCommonConfig>(cpCommonConfig);
    }


    public DbRow_CpCommonConfig GetCpCommonConfig(string id) {
        return cpCommonConfigLookup.Get(id);
    }
}
