using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

class SavedDatasManager
{
    #region Paths
    static readonly string podsSavedPath    = @"/Jsons/PodsSaved.json";
    static readonly string raceDatasPath    = @"/Jsons/RaceDatas.json";
    static readonly string ghostsPath       = @"/Jsons/Ghosts.json";
    static readonly string unlockedPath     = @"/Jsons/LockStates.json";
    static readonly string partsPrefabPath  = @"Parts"; // -> "Assets/Resources/Parts"
    static readonly string fracturedPartsPrefabPath  = @"FracturedParts";
    static readonly string podsPrefabPath   = @"Pods";
    static readonly string podsVignettePath = @"Vignettes";
    #endregion

    #region Champs
    public static bool bugMillestone = true;

    private static List<PodModel> podModels = null;

    private static List<Part> allParts = null;
    private static List<FracturedPart> fracturedParts = null;
    private static List<Part> lockedParts = null;
    private static List<Part> unlockParts = null;
    private static List<GameObject> defaultPods = null;
    private static List<RaceData> raceDatas = null;
    private static List<Ghost> ghosts = null;
    private static List<LockStatePart> lockStateParts = null;

    public static List<PodModel> PodModels
    {
        get
        {
            if (podModels == null)
                podModels = GetSavedPodModels() ?? new List<PodModel>();
            return podModels;
        }
    }
    public static List<Part> AllParts
    {
        get
        {
            if (allParts == null)
                allParts = GetParts() ?? new List<Part>();
            return allParts;
        }
    }
    public static List<FracturedPart> FracturedParts
    {
        get
        {
            if (fracturedParts == null)
                fracturedParts = GetFracturedParts() ?? new List<FracturedPart>();
            return fracturedParts;
        }
    }
    public static List<Part> LockedParts
    {
        get
        {
            if (lockedParts == null)
                lockedParts = GetLockedParts() ?? new List<Part>();
            return lockedParts;
        }
    }
    public static List<Part> UnlockedParts
    {
        get
        {
            if (unlockParts == null)
                unlockParts = GetUnlockedParts() ?? new List<Part>();
            return unlockParts;
        }
    }
    public static List<GameObject> DefaultPods
    {
        get
        {
            if (defaultPods == null)
                defaultPods = GetDefaultPods() ?? new List<GameObject>();
            return defaultPods;
        }
    }
    public static List<RaceData> RaceDatas
    {
        get
        {
            if (raceDatas == null)
            {
                raceDatas = GetSavedRaceDatas() ?? new List<RaceData>();
            }
            return raceDatas;
        }
    }
    public static List<Ghost> Ghosts
    {
        get
        {
            if (ghosts == null)
                ghosts = GetGhosts() ?? new List<Ghost>();
            return ghosts;
        }
    }
    private static List<LockStatePart> LockStateParts
    {
        get
        {
            if (lockStateParts == null)
                lockStateParts = GetLockStateFile();
            return lockStateParts;
        }
    }
    #endregion
    
    #region PodModel
    private static List<PodModel> GetSavedPodModels()
    {
        List<PodModel> result = new List<PodModel>();
        string path = Application.streamingAssetsPath + podsSavedPath;

        if (!File.Exists(path))
        {
            using (File.Create(path)) ;
            if (ScriptDefaultDatas.DefaultPodJson != "")
                SavePodModels(JsonConvert.DeserializeObject<List<PodModel>>(ScriptDefaultDatas.DefaultPodJson).OrderBy(x => x.ID).ToList(), false);
        }

        using (StreamReader jsonR = new StreamReader(path))
        {
            try
            {
                result = JsonConvert.DeserializeObject<List<PodModel>>(jsonR.ReadToEnd()).OrderBy(x => x.ID).ToList();
            }
            catch (Exception e)
            {
                result = null;
            }
        }

        if (result == null || bugMillestone)
        {
            //Debug.Log("Get Bug Millestone");
            bugMillestone = true;
            if (TempPlayerPref.PodModels.Count == 0)
                TempPlayerPref.PodModels = JsonConvert.DeserializeObject<List<PodModel>>(ScriptDefaultDatas.DefaultPodJson).OrderBy(x => x.ID).ToList();
            result = TempPlayerPref.PodModels;
        }
        return result ?? (result = new List<PodModel>());
    }
    private static void SavePodModels(List<PodModel> pms, bool _refresh = true)
    {
        if (!bugMillestone)
        {
            using (StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + podsSavedPath))
            {
                writer.Write(JsonConvert.SerializeObject(pms));
            }
        }
        else
        {
            Debug.Log("Bug Millestone");
            TempPlayerPref.PodModels = pms;
            TempPlayerPref.PodModels.ForEach(x => Debug.Log(x.ID));
        }
        if (_refresh) podModels = GetSavedPodModels();
    }
    public static void SavePodModels(ref PodModel _pm)
    {
        string tmpName = _pm.Name;
        int tmpID = _pm.ID;

        if (_pm.ID == -999) // Nouveau pod
        {
            _pm.ID = (GetSavedPodModels().Count == 0) ? 0 : GetSavedPodModels().Max(x => x.ID) + 1;
            int i = 1;
            while (GetPodModelByName(_pm.Name) != null)
            {
                _pm.Name = tmpName + i;
                i++;
            }
        }

        if (GetPodModelById(_pm.ID) == null) podModels.Add(_pm);
        else
        {
            DeletePodModel(_pm.ID);
            podModels.Add(_pm);
        }

        SavePodModels(PodModels);
    }
    public static PodModel GetPodModelByName(string _name)
    {
        List<PodModel> pms = PodModels.Where(x => x.Name == _name).ToList();
        return (pms.Count > 0) ? pms[0] : null;
    }
    public static PodModel GetPodModelById(int _id)
    {
        List<PodModel> pms = PodModels.Where(x => x.ID == _id).ToList();
        return (pms.Count > 0) ? pms[0] : null;
    }
    public static void DeletePodModel(int id)
    {
        podModels.RemoveAll(x => x.ID == id);
        SavePodModels(podModels);
        podModels = GetSavedPodModels();
    }
    #endregion
    
    #region PodParts
    private class LockStatePart
    {
        public int id;
        public bool isUnlock;
    }

    private static List<Part> GetParts()
    {
        List<Part> parts = new List<Part>();
        parts = Resources.LoadAll<Part>(partsPrefabPath).ToList();
        return parts;
    }
    private static List<FracturedPart> GetFracturedParts()
    {
        List<FracturedPart> parts = new List<FracturedPart>();
        parts = Resources.LoadAll<FracturedPart>(fracturedPartsPrefabPath).ToList();
        return parts;
    }
    private static List<Part> GetUnlockedParts()
    {
        List<Part> tmp = new List<Part>();
        foreach (Part part in AllParts)
        {
            if (LockStateParts.Find(x => x.id == part.ID) != null)
                if (LockStateParts.Find(x => x.id == part.ID).isUnlock)
                    tmp.Add(part);
        }
        return tmp;
    }
    private static List<Part> GetLockedParts()
    {
        List<Part> tmp = new List<Part>();
        foreach (Part part in AllParts)
        {
            if (LockStateParts.Find(x => x.id == part.ID) != null)
                if (!LockStateParts.Find(x => x.id == part.ID).isUnlock)
                {
                    tmp.Add(part);
                }
        }
        return tmp;
    }
    /// <param name="_lockState">true -> Unlock; false -> Lock</param>
    public static void SetLockState(bool _lockState, int _id = -1)
    {
        string path = Application.streamingAssetsPath + unlockedPath;
        LockStateParts.Find(x => x.id == _id).isUnlock = _lockState;
        using (StreamWriter jsonW = new StreamWriter(path))
        {
            jsonW.Write(JsonConvert.SerializeObject(LockStateParts));
        }
        RefreshLockStateParts();
    }
    public static Part GetPartByID(int _id)
    {
        List<Part> p = AllParts.Where(x => x.ID == _id).ToList();
        return (p.Count > 0) ? p[0] : null;
    }
    public static Part GetPartByName(string _name)
    {
        List<Part> p = AllParts.Where(x => x.Name == _name).ToList();
        return (p.Count > 0) ? p[0] : null;
    }
    public static Part GetPartByTypePart(TypePart _typePart)
    {
        List<Part> p = AllParts.Where(x => x.TypePart == _typePart).ToList();
        return (p.Count > 0) ? p[0] : null;
    }
    public static List<Part> GetPartsByTypePart(TypePart _typePart)
    {
        return AllParts.Where(x => x.TypePart == _typePart).ToList();
    }

    private static List<LockStatePart> GetLockStateFile()
    {
        List<LockStatePart> result = new List<LockStatePart>();
        string path = Application.streamingAssetsPath + unlockedPath;
        if (!File.Exists(path))
        {
            using (File.Create(path)) ;
            AllParts.ForEach(x => result.Add(new LockStatePart() { id = x.ID, isUnlock = false }));
            //result.ForEach(x => Debug.Log(x.id));
            //foreach(Part part in parts)
            //    lockStateParts.Add(new LockStatePart() { id = part.ID, isUnlock = false });
            using (StreamWriter jsonW = new StreamWriter(path))
            {
                jsonW.Write(JsonConvert.SerializeObject(result.OrderBy(x => x.id).ToList()));
            }
            SetLockState(true, 4);
            SetLockState(true, 104);
            SetLockState(true, 204);
            SetLockState(true, 304);
            SetLockState(true, 404);
        }
        else
        {
            result = Deserialize<List<LockStatePart>>(path);
            //using (StreamReader jsonR = new StreamReader(path))
            //{
            //    try
            //    {
            //        lockStateParts = JsonConvert.DeserializeObject<List<LockStatePart>>(jsonR.ReadToEnd()).ToList();
            //    }
            //    catch
            //    {
            //        lockStateParts = new List<LockStatePart>();
            //    }
            //}
        }
        return result;
    }
    private static void RefreshLockStateParts()
    {
        lockStateParts = GetLockStateFile();
    }
    #if UNITY_EDITOR
    [MenuItem("SavedDatasManager/Debug/Lock All Parts")]
    #endif
    public static void LockAll()
    {
        AllParts.ForEach(x => SetLockState(false, x.ID));
    }
    
    #if UNITY_EDITOR
    [MenuItem("SavedDatasManager/Debug/Unlock All Parts")]
    #endif
    public static void UnlockAll()
    {
        AllParts.ForEach(x => SetLockState(true, x.ID));
    }
    
    #if UNITY_EDITOR
    [MenuItem("SavedDatasManager/Debug/Unlock Beginner Parts")]
    #endif
    public static void UnlockBegginer()
    {
        LockAll();
        SetLockState(true, 4);
        SetLockState(true, 104);
        SetLockState(true, 204);
        SetLockState(true, 304);
        SetLockState(true, 404);
    }
    #endregion

    #region RaceData
    private static List<RaceData> GetSavedRaceDatas()
    {
        List<RaceData> result = new List<RaceData>();
        string path = Application.streamingAssetsPath + raceDatasPath;
        if (!File.Exists(path))
        {
            using (File.Create(path)) ;
        }

        using (StreamReader jsonR = new StreamReader(path))
        {
            try
            {
                result = JsonConvert.DeserializeObject<List<RaceData>>(jsonR.ReadToEnd()).ToArray().OrderBy(x => x.ID).ToList();
            }
            catch
            {
                result = null;
            }
        }
        return result ?? (result = new List<RaceData>());
    }
    public static List<RaceData> GetSavedRaceDatasByRaceType(RaceType _rt)
    {
        return RaceDatas.FindAll(x => x.RaceType == _rt);
    }
    private static void SaveRaceDatas(List<RaceData> rds, bool _refresh = true)
    {
        using (StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + raceDatasPath))
        {
            writer.Write(JsonConvert.SerializeObject(rds));
        }
        if (_refresh)
            raceDatas = GetSavedRaceDatas();
    }
    public static void SaveRaceDatas(RaceData rd)
    {
        List<RaceData> rds = RaceDatas;
        rds.Add(rd);
        SaveRaceDatas(rds);
    }
    #endregion
    
    #region Ghosts
    private static List<Ghost> GetGhosts()
    {
        List<Ghost> ghosts = new List<Ghost>();
        string path = Application.streamingAssetsPath + ghostsPath;
        if (!File.Exists(path))
        {
            using (File.Create(path)) ;
        }
        using (StreamReader jsonR = new StreamReader(path))
        {
            ghosts = JsonConvert.DeserializeObject<List<Ghost>>(jsonR.ReadToEnd());
        }
        return ghosts;
    }
    public static void SaveGhost(Ghost _ghost)
    {
        ghosts = Ghosts;
        ghosts.Add(_ghost);
        SaveGhosts(ghosts);
    }
    private static void SaveGhosts(List<Ghost> _ghosts, bool _refresh = true)
    {
        using (StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + ghostsPath))
        {
            writer.Write(JsonConvert.SerializeObject(ghosts));
        }
        if (_refresh)
            ghosts = GetGhosts();
    }
    #endregion

    #region DefaultPod
    private static List<GameObject> GetDefaultPods()
    {
        return new List<GameObject>();
        return Resources.LoadAll<GameObject>(podsPrefabPath).ToList();
    }
    #endregion

    private static T Deserialize<T>(string _path) where T : new()
    {
        T result;
        using (StreamReader jsonR = new StreamReader(_path))
        {
            try
            {
                result = JsonConvert.DeserializeObject<T>(jsonR.ReadToEnd());
            }
            catch
            {
                result = new T();
            }
        }
        return result;
    }
}
