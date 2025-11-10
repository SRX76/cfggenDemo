using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using YooAsset;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class ResMgr : MonoBehaviour
{
    [SerializeField]
    private EPlayMode playMode;
    public EPlayMode PlayMode
    {
        get
        {
            return playMode;
        }
        set
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                return;
            }
#endif
            playMode = value;
        }
    }
    public bool InitSuccess;

    public ResourcePackage package;

    public class AssetInfo<T>
    {
        public string file { get; private set; }
        public AssetHandle handle;
        public int count;
        public HashSet<T> hashSet;

        private Action action;
        public AssetInfo(AssetHandle _handle)
        {
            handle = _handle;
            handle.Completed += Handle_Completed;
        }

        private void Handle_Completed(AssetHandle obj)
        {

        }

        public void Load(Action act)
        {

        }

        public void Free(T obj)
        {
            hashSet.Remove(obj);
        }
    }
    private Dictionary<string, AssetHandle> assetHandleDic = new Dictionary<string, AssetHandle>();
    public Transform parent;
    private int index;
    void Awake()
    {
        StartCoroutine(InitPackage());
    }

    private async void Update()
    {
        if (!InitSuccess)
        {
            return;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            for (int i = 0; i < 100; i++)
            {
                int curindex = index++;
                await LoadGOAsync("engines_enemy", (goPfb) => CreateGO(goPfb, curindex));
            }
        }
    }

    private void OnDisable()
    {
        Debug.LogError($"OnDisable");
        int count = parent.childCount;
        while (count > 0)
        {
            GameObject.Destroy(parent.GetChild(count - 1));
            count--;
        }
    }
    private void OnDestroy()
    {
        Debug.LogError($"OnDestroy");
        StartCoroutine(ForceUnloadAllAssets());
    }

    private void ReleaseHandle(string file)
    {
        if (assetHandleDic.TryGetValue(file, out var handle))
        {
            assetHandleDic.Remove(file);
            if (handle != null)
            {
                handle.Release();
            }
        }
    }

    private IEnumerator ForceUnloadAllAssets()
    {
        var operation = package.UnloadAllAssetsAsync();
        yield return operation;
        yield return package.DestroyAsync();
        package = null;
    }

    void CreateGO(GameObject goPfb, int index)
    {
        var go = GameObject.Instantiate(goPfb, parent);
        float angle = index * 3 / Mathf.PI;
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);
        go.transform.position = new Vector3(x, 0, y) * index * 0.3f;
    }

    async Task LoadGOAsync(string path, Action<GameObject> act)
    {
        if (!assetHandleDic.TryGetValue(path, out var handle) || handle == null)
        {
            handle = package.LoadAssetAsync<GameObject>(path);
            assetHandleDic[path] = handle;
        }
        //if (assetHandle.Contains(handle))
        //{
        //    Debug.LogError($"handle已经添加到列表中");
        //}
        //assetHandle.Add(handle);
        if (handle.Status != EOperationStatus.Succeed)
        {
            await handle.Task;
        }
        if (handle.Status == EOperationStatus.Succeed)
        {
            act?.Invoke(handle.AssetObject as GameObject);
        }
    }

    async void LoadGameObject(string path, Action<GameObject> act)
    {
        if (!assetHandleDic.TryGetValue(path, out var handle) || handle == null)
        {
            handle = package.LoadAssetAsync<GameObject>(path);
            assetHandleDic[path] = handle;
            await handle.Task;
        }
        if (handle.Status == EOperationStatus.Succeed)
        {
            act?.Invoke(handle.AssetObject as GameObject);
        }
        else if (handle.IsDone)
        {

        }

    }


    private IEnumerator InitPackage_1()
    {
        YooAssets.Initialize();
        var packageName = "DefaultPackage";
        package = YooAssets.TryGetPackage(packageName);
        if (package == null)
        {
            package = YooAssets.CreatePackage(packageName);
        }

        InitializeParameters createParameters = CreateInitParameters(playMode);
        if (createParameters == null)
        {
            Debug.LogError($"获取初始化参数失败！！！！！");
            yield break;
        }
        var initOperation = package.InitializeAsync(createParameters);
        yield return initOperation;
        if (initOperation.Status == EOperationStatus.Succeed)
        {
            package = YooAssets.GetPackage("DefaultPackage");
            var operation = package.RequestPackageVersionAsync();
            yield return operation;
            if (operation.Status == EOperationStatus.Succeed)
            {
                Debug.LogError($"资源版本号：{operation.PackageVersion}");
                var operation_updateManifest = package.UpdatePackageManifestAsync(operation.PackageVersion);
                yield return operation_updateManifest;
                if (operation_updateManifest.Status == EOperationStatus.Succeed)
                {
                    Debug.LogError($"更新资源清单成功！");
                    var downloader = package.CreateResourceDownloader(10, 3);
                    Debug.LogError($"需要下载的资源数量：{downloader.TotalDownloadCount}");
                    if (downloader.TotalDownloadCount == 0)
                    {
                        YooAssets.SetDefaultPackage(package);
                        InitSuccess = true;

                        Debug.Log($"资源包初始化成功,{package.GetPackageVersion()}");
                    }
                }
                else
                {
                    Debug.LogError($"更新资源清单失败：{operation_updateManifest.Error}");
                    yield break;
                }
            }
            else
            {
                Debug.LogError($"请求资源版本号失败：{operation.Error}");
                yield break;
            }



        }
        else
        {
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
            InitSuccess = false;
        }
    }

    private IEnumerator InitPackage()
    {
        YooAssets.Initialize();
        var packageName = "DefaultPackage";
        package = YooAssets.TryGetPackage(packageName);
        if (package == null)
        {
            package = YooAssets.CreatePackage(packageName);
        }
        //根据选择的模式创建对应平台的初始化参数对象
        InitializeParameters createParameters = CreateInitParameters(playMode);
        if (createParameters == null)
        {
            Debug.LogError($"获取初始化参数失败！！！！！");
            yield break;
        }
        var initOperation = package.InitializeAsync(createParameters);
        yield return initOperation;
        if (initOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
            InitSuccess = false;
            yield break;
        }
        //获取远程的资源版本号
        var operation = package.RequestPackageVersionAsync();
        yield return operation;
        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"请求资源版本号失败：{operation.Error}");
            yield break;
        }
        //更新资源清单
        var operation_updateManifest = package.UpdatePackageManifestAsync(operation.PackageVersion);
        yield return operation_updateManifest;
        if (operation_updateManifest.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"更新资源清单失败：{operation_updateManifest.Error}");
            yield break;
        }
        //创建资源下载器
        var downloader = package.CreateResourceDownloader(10, 3);
        while (downloader.TotalDownloadCount > 0)
        {
            yield return downloader.Task;
        }
        if (downloader.TotalDownloadCount == 0)
        {
            YooAssets.SetDefaultPackage(package);
            InitSuccess = true;

            Debug.Log($"资源包初始化成功,{package.GetPackageVersion()}");
        }
    }

    #region 根据选择的模式创建对应平台的初始化参数对象

    private InitializeParameters CreateInitParameters(EPlayMode mode)
    {
        if (mode == EPlayMode.EditorSimulateMode)
        {
            return CreateEditorSimulateModeParameters();
        }
        if (mode == EPlayMode.OfflinePlayMode)
        {
            return CreateOfflinePlayModeParameters();
        }
        if (mode == EPlayMode.HostPlayMode)
        {
            return CreateHostPlayModeParameters();
        }
        if (mode == EPlayMode.WebPlayMode)
        {
            return CreateWebPlayModeParameters();
        }
        return null;
    }

    private InitializeParameters CreateEditorSimulateModeParameters()
    {
        var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
        var packageRoot = buildResult.PackageRootDirectory;
        var createParameters = new EditorSimulateModeParameters();
        createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
        return createParameters;
    }

    private InitializeParameters CreateOfflinePlayModeParameters()
    {
        var fileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
        var createParameters = new OfflinePlayModeParameters();
        createParameters.BuildinFileSystemParameters = fileSystemParams;
        return createParameters;
    }
    private InitializeParameters CreateHostPlayModeParameters()
    {
        var defaultHostServer = "http://127.0.0.1/CDN/Android/v1.0";
        var fallbackHostServer = "http://127.0.0.1/CDN/Android/v1.0";
        IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
        var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
        var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

        var createParameters = new HostPlayModeParameters();
        createParameters.CacheFileSystemParameters = cacheFileSystemParams;
        createParameters.BuildinFileSystemParameters = buildinFileSystemParams;
        return createParameters;
    }
    private InitializeParameters CreateWebPlayModeParameters()
    {
        var defaultHostServer = "http://127.0.0.1/CDN/Android/v1.0";
        var fallbackHostServer = "http://127.0.0.1/CDN/Android/v1.0";
        IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
        var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
        var webRemoteFileSystemParams = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices);

        var createParameters = new WebPlayModeParameters();
        createParameters.WebServerFileSystemParameters = webServerFileSystemParams;
        createParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;
        return createParameters;
    }

    #endregion

    #region Loader资源加载器

    #region GameObjectLoader


    #endregion

    #endregion

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }

        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }
}
