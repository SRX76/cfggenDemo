using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using YooAsset;
using YooAsset.Editor;
using System;
using System.Text;

public class ResCollectorEditor
{
    [MenuItem("Tools/统计可加载的资源")]
    public static void CollectAll()
    {
        var settting = AssetBundleCollectorSettingData.Setting;

        Dictionary<Type, List<string>> dic = new();
        foreach (var package in settting.Packages)
        {
            var collectResult = settting.BeginCollect(package.PackageName, true, true);
            foreach (var item in collectResult.CollectAssets)
            {
                if (!dic.TryGetValue(item.AssetInfo.AssetType, out var list))
                {
                    list = new List<string>();
                    dic[item.AssetInfo.AssetType] = list;
                }
                list.Add(item.Address);
            }
        }
        StringBuilder sb = new StringBuilder(4096);
        Debug.LogError($"{dic.Count}");
        foreach (var kvp in dic)
        {
            sb.Append(kvp.Key.ToString());

            foreach (var item in kvp.Value)
            {
                sb.AppendLine();
                sb.Append(item);
            }
        }
        Debug.LogError(sb.ToString());
        sb.Clear();
        sb = null;
        dic.Clear();
        dic = null;
        Debug.Log("资源收集完成");
    }
}
