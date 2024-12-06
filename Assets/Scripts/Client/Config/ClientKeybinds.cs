using UnityEngine;
using Util;

namespace Client.Config
{

    public enum Keybind
    {
        MoveUp,
        MoveDown,
        MoveRight,
        MoveLeft,
        AbilityQ,
        AbilityE,
        AbilityR,
        AbilityDash
    }
    
    public class ClientKeybinds
    {

        public static JsonPlayerPrefs prefs = new(Application.persistentDataPath + "/keybinds.json");
        
        public static KeyCode GetKeybind(Keybind keybind)
        {
            return (KeyCode) prefs.GetInt(keybind.ToString());
        }
        
        public static void SetKeybind(Keybind keybind, KeyCode keyCode)
        {
            prefs.SetInt(keybind.ToString(), (int) keyCode);
            prefs.Save();
        }

    }
}