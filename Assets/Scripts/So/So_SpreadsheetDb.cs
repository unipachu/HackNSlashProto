using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpreadsheetDb_", menuName = "Scriptable Object Data/SpreadsheetDb")]
public class So_SpreadsheetDb : SpreadsheetContainerBase {
    [Sheet("AiCpConfig")]
    [SerializeField] List<DbRow_AiCpConfig> aiCpConfig;
    [Sheet("ComboAnimDurConfig")]
    [SerializeField] List<DbRow_ComboAnimDurConfig> comboAnimDurConfig;
    [Sheet("CpCommonConfig")]
    [SerializeField] List<DbRow_CpCommonConfig> cpCommonConfig;
    [Sheet("CpFlyingHeadConfig")]
    [SerializeField] List<DbRow_CpFlyingHeadConfig> cpFlyingHeadConfig;
    [Sheet("CpHumdConfig")]
    [SerializeField] List<DbRow_CpHumdConfig> cpHumdConfig;
    // TODO MINOR: Some data only recuires one row. Now all sheets are made into lists, but there could be
    // C: support of individual row fields too.
    [Sheet("PlrCpConfig")]
    [SerializeField] List<DbRow_PlrCpConfig> plrCpConfig;

    public SheetLookup<DbRow_AiCpConfig> aiCpConfigLookup;
    public SheetLookup<DbRow_ComboAnimDurConfig> comboAnimDurConfigLookup;
    public SheetLookup<DbRow_CpCommonConfig> cpCommonConfigLookup;
    public SheetLookup<DbRow_CpFlyingHeadConfig> cpFlyingHeadConfigLookup;
    public SheetLookup<DbRow_CpHumdConfig> cpHumdConfigLookup;
    public SheetLookup<DbRow_PlrCpConfig> plrCpConfigLookup;

    public override void RebuildLookups() {
        aiCpConfigLookup = new SheetLookup<DbRow_AiCpConfig>(aiCpConfig);
        comboAnimDurConfigLookup = new SheetLookup<DbRow_ComboAnimDurConfig>(comboAnimDurConfig);
        cpCommonConfigLookup = new SheetLookup<DbRow_CpCommonConfig>(cpCommonConfig);
        cpFlyingHeadConfigLookup = new SheetLookup<DbRow_CpFlyingHeadConfig>(cpFlyingHeadConfig);
        cpHumdConfigLookup = new SheetLookup<DbRow_CpHumdConfig> (cpHumdConfig);
        plrCpConfigLookup = new SheetLookup<DbRow_PlrCpConfig> (plrCpConfig);
    }
}
