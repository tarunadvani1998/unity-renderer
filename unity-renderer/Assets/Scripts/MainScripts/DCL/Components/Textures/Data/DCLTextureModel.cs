using DCL.Helpers;
using UnityEngine;

[System.Serializable]
public class DCLTextureModel : BaseModel
{

    public string src;
    public BabylonWrapMode wrap = BabylonWrapMode.CLAMP;
    public FilterMode samplingMode = FilterMode.Bilinear;
    public bool hasAlpha = false;

    public override BaseModel GetDataFromJSON(string json) { return Utils.SafeFromJson<DCLTextureModel>(json); }

    public enum BabylonWrapMode
    {
        CLAMP,
        WRAP,
        MIRROR
    }
}
