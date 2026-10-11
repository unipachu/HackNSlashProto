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
    // TODO MINOR: Some data only recuires one row. Now all sheets are made into lists, but there could be
    // C: support of individual row fields too.
    [Sheet("PlrCpConfig")]
    [SerializeField] List<DbRow_PlrCpConfig> plrCpConfig;

    public SheetLookup<DbRow_AiCpConfig> aiCpConfigLookup;
    public SheetLookup<DbRow_CpCommonConfig> cpCommonConfigLookup;
    public SheetLookup<DbRow_CpHumdConfig> cpHumdConfigLookup;
    public SheetLookup<DbRow_PlrCpConfig> plrCpConfigLookup;

    public override void RebuildLookups() {
        aiCpConfigLookup = new SheetLookup<DbRow_AiCpConfig>(aiCpConfig);
        cpCommonConfigLookup = new SheetLookup<DbRow_CpCommonConfig>(cpCommonConfig);
        cpHumdConfigLookup = new SheetLookup<DbRow_CpHumdConfig> (cpHumdConfig);
        plrCpConfigLookup = new SheetLookup<DbRow_PlrCpConfig> (plrCpConfig);
    }
}
