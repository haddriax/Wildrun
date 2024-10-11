using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class TempPlayerPref
{
    public static List<PodModel> PodModels = new List<PodModel>();

    public static List<Part> LockedParts = new List<Part>();
    public static List<Part> UnlockedParts = new List<Part>();

    private static Dictionary<string, string> tmpPlayerPref = new Dictionary<string, string>();

    public static bool HasKey(string _key)
    {
        return tmpPlayerPref.ContainsKey(_key);
    }

    public static void SetKey(string _key, string _value)
    {
        if (!HasKey(_key))
            tmpPlayerPref.Add(_key, _value);
        else
            tmpPlayerPref[_key] = _value;
    }

    public static string GetKey(string _key)
    {
        if (HasKey(_key))
            return tmpPlayerPref[_key];
        return "";
    }

    public static void DeleteKey(string _key)
    {
        if (HasKey(_key))
            tmpPlayerPref.Remove(_key);
    }
}
