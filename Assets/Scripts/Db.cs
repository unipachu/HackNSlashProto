using UnityEngine;

/// <summary>
/// Database for spreadsheets data.
/// </summary>
public class Db : Singleton<Db>{
    [SerializeField] So_SpreadsheetDb db;

    public static DbRow_AiCpConfig GetAiCpConfig(string id) {
        return inst.db.aiCpConfigLookup.Get(id);
    }

    public static DbRow_ComboAnimDurConfig GetComboAnimDurConfig(string id) {
        return inst.db.comboAnimDurConfigLookup.Get(id);
    }

    public static DbRow_CpCommonConfig GetCpCommonConfig(string id) {
        return inst.db.cpCommonConfigLookup.Get(id);
    }

    public static DbRow_CpFlyingHeadConfig GetCpFlyingHeadConfig(string id) {
        return inst.db.cpFlyingHeadConfigLookup.Get(id);
    }

    public static DbRow_CpHumdConfig GetCpHumdConfig(string id) {
        return inst.db.cpHumdConfigLookup.Get(id);
    }

    public static DbRow_PlrCpConfig GetPlrCpConfig(string id) {
        return inst.db.plrCpConfigLookup.Get(id);
    }

    public static DbRow_HitEffectsConfig GetHitEffectsConfig(string id) {
        return inst.db.hitEffectsConfigLookup.Get(id);
    }
}
