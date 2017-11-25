using UnityEngine;

public class AssetRequestTask{
    public System.Action<Object> onAssetLoaded;

    private string _assetbundleName;

    public void SetAssetBundleName(string assetBundleName, string assetBundleVariant){
        _assetbundleName = string.Format("{0}.{1}", assetBundleName.ToLower(), assetBundleVariant.ToLower());
    }

    public string GetABName(){
        return _assetbundleName;
    }
}
