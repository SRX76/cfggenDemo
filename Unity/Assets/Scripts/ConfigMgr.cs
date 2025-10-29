using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InitConfig();
        foreach (var item in Config.Testdemo.DataGlobal_num.All())
        {
            Debug.LogError($"{item.Id},{item.ColorType}");
        }
    }

    private static void InitConfig()
    {
        var filePath = "config.bytes";
        var bytes = File.ReadAllBytes(filePath);
        //XorBytesWithCipher(bytes, "password");
        Config.Loader.Processor = Config.Processor.Process;
        Config.Loader.LoadBytes(bytes);
    }

    //打包时设置了密码，需要使用这个方法解密
    public static void XorBytesWithCipher(byte[] target, string cipher)
    {
        byte[] cipherBytes = System.Text.Encoding.UTF8.GetBytes(cipher);
        for (int i = 0; i < target.Length; i++)
        {
            target[i] = (byte)(target[i] ^ cipherBytes[i % cipherBytes.Length]);
        }
    }
}
