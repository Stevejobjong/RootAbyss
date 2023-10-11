using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// 파일 입출력을 관리하는 매니저
/// </summary>
public class FileMng : Singleton<FileMng>
{
    protected FileMng() { }

    public void LoadFile<T>(ref Dictionary<string, T> a, string path) where T : Object
    {
        string log = path + "에서 불러온 파일 \n";
        a = new Dictionary<string, T>();
        T[] File = Resources.LoadAll<T>(path);
        foreach (var f in File)
        {
            log += f.name + "\n";
            a.Add(f.name, f);
        }
        Debug.Log(log);
    }

    public void LoadFile<T>(ref List<T> a, string path) where T : Object
    {
        string log = path + "에서 불러온 파일 \n";
        a = new List<T>();
        T[] File = Resources.LoadAll<T>(path);
        foreach (var f in File)
        {
            log += f.name + "\n";
            a.Add(f);
        }
        Debug.Log(log);
    }
}
