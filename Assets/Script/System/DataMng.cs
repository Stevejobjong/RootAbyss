using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class DataMng : Singleton<DataMng>
{
    public string path;
    public Data settingValue;
    void Awake(){
        path = Application.persistentDataPath + "/SettingData.json";
        if (File.Exists(path))
            LoadSettingData();
        else
            InitSettingData();
    }

    public void InitSettingData()
    {
        settingValue = new Data(200f, 0.3f);
        var result = JsonConvert.SerializeObject(settingValue);
        print(result);
        File.WriteAllText(path, result);
    }
    public void LoadSettingData()
    {
        string JsonFile;
        if (File.Exists(path))
        {
            JsonFile = File.ReadAllText(path);
            print(JsonFile);
            settingValue = JsonConvert.DeserializeObject<Data>(JsonFile);
        }
    }
    public void SaveSettingData()
    {
        var result = JsonConvert.SerializeObject(settingValue);
        print(result);
        File.WriteAllText(path, result);
    }
}
[System.Serializable]
public class Data
{
    public float MouseSens;
    public float Volume;
    public Data(float m, float v)
    {
        this.MouseSens = m;
        this.Volume = v;
    }
}