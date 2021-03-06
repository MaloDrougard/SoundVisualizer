﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

/// <summary>
/// This objest is used to set all sliders at once and to be able to create slider configuration;
/// Fot the moment all the sliders and toggles of the interfaces are set and save even if you add a new one :) 
/// Be care full, the values and the sliders are mapped by list!
/// </summary>
public class SliderManager : MonoBehaviour {



    List<Slider> sliders = new List<Slider>();
    List<Toggle> toggles = new List<Toggle>();

    public Dictionary<string, SlidersConfig> configs = new Dictionary<string, SlidersConfig>();

    InputField saveConfigNameInput;
    Dropdown loadConfigDropdown;

    string dataPath; 

    // Use this for initialization
    void Start() {

        sliders = new List<Slider>(FindObjectsOfType<Slider>());
        toggles = new List<Toggle>(FindObjectsOfType<Toggle>());

        saveConfigNameInput = GameObject.Find("InputFieldSaveSlidersConfig").GetComponent<InputField>();
        loadConfigDropdown = GameObject.Find("DropdownLoadSlidersConfig").GetComponent<Dropdown>();

        dataPath = Path.Combine(Application.persistentDataPath, "SlidersConfigs.txt");
        Debug.LogWarning(dataPath); 


        SlidersConfig initConfig = new SlidersConfig();
        initConfig.configName = "init";
        initConfig.slidersValue = new List<float>() { 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, };
        initConfig.togglesValue = new List<bool>() {true, true, true };
        configs.Add(initConfig.configName, initConfig);
        SetLoadDropDown();

        LoadConfig("init");
    }

    public void SetLoadDropDown()
    {
        loadConfigDropdown.ClearOptions();
        loadConfigDropdown.AddOptions(new List<string>(configs.Keys));
    }

    public void LoadConfig()
    {
        string name = loadConfigDropdown.options[loadConfigDropdown.value].text; ;
        LoadConfig(name);
    }

    public void LoadConfig(string name)
    {
        SlidersConfig config = configs[name];
        for (int i = 0; i < sliders.Count; i++)
        {
            sliders[i].value = config.slidersValue[i];
        }  
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].isOn = config.togglesValue[i];
        }
    }

    public void SaveConfig()
    {
        SaveConfig(saveConfigNameInput.text);    
    }

    public void SaveConfig(string name)
    {
        SlidersConfig newConfig = new SlidersConfig();
        newConfig.configName = name;
        for (int i = 0; i < sliders.Count; i++)
        {
            newConfig.slidersValue.Add(sliders[i].value);
        }
        for (int i = 0; i < toggles.Count; i++)
        {
            newConfig.togglesValue.Add(toggles[i].isOn);
        }
        configs.Add(newConfig.configName, newConfig);
        SetLoadDropDown(); // to update the display; 
    }

    public void SaveOnDisk()
    {
        
        SlidersConfigJsonWrapper configWrapper = new SlidersConfigJsonWrapper();
        configWrapper.configs = new SlidersConfig[configs.Count];
        int i = 0; 
        foreach (string configName in configs.Keys)
        {
            configWrapper.configs[i] = configs[configName];
            i++;     
        }
      
        string jsonString = JsonUtility.ToJson(configWrapper);
        Debug.LogWarning(jsonString);
        using (StreamWriter streamWriter = File.CreateText(dataPath))
        {
            streamWriter.Write(jsonString);
        }
    }

    public void LoadFromDisk()
    {

        SlidersConfigJsonWrapper configWrapper;
        using (StreamReader streamReader = File.OpenText(dataPath))
        {
            string jsonString = streamReader.ReadToEnd();
            configWrapper = JsonUtility.FromJson<SlidersConfigJsonWrapper>(jsonString);
        }

        configs.Clear(); 
        for(int i = 0; i < configWrapper.configs.Length; i++)
        {
            SlidersConfig config = configWrapper.configs[i];
            configs.Add(config.configName, config);
        }
        SetLoadDropDown();    
    }
   
}



[Serializable]

public class SlidersConfig
{
    public string configName = "notSet"; 
    public List<float> slidersValue = new List<float>();
    public List<bool> togglesValue = new List<bool>(); 
}


// Used to be able to save easly the data in json
[Serializable]
public class SlidersConfigJsonWrapper
{
    public SlidersConfig[] configs; 
}