using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset.Editor;

public class PackSingle : IPackRule
{
    //期望把依赖的其他资源也打包进来
    public PackRuleResult GetPackRuleResult(PackRuleData data)
    {
        string bundleName = Path.GetDirectoryName(data.AssetPath);
        PackRuleResult result = new PackRuleResult(bundleName, "SingleByte");
        return result;
    }
}
