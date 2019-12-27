using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class InputManager : MonoBehaviour {

    public struct KeyBindingData
    {
        public List<KeyCode> keyCodes;
        public ButtonPressType pressType;
        public CallBack callback;

        public bool isExclusive;
    }


    public enum ButtonPressType
    {
        PRESSED,
        DOWN,
        UP,
    }

    public GameManager gameManager;
    public GameSceneManager gameSceneManager;

    public delegate void CallBack(List<KeyCode> triggeredKeys);
    private Dictionary<List<KeyCode>, KeyBindingData> keyBindings;

    private Dictionary<KeyCode,bool> currPressedKeys = new Dictionary<KeyCode, bool>();
    private List<KeyCode> bufferMod;

    void Awake() //before any start init stuff
    {
        keyBindings = new Dictionary<List<KeyCode>, KeyBindingData>();

        AddKeyBinding(new List<KeyCode>() { KeyCode.Z, KeyCode.X, KeyCode.C }, InputManager.ButtonPressType.DOWN, 
        delegate(List<KeyCode> triggeredKey) {
            Debug.Log("rfegyregfyreugfryeufr");
        }, false);
    }


    public bool ContainsSequence<KeyCode>(List<KeyCode> source, List<KeyCode> other)
    {
        int count = other.Count();
        while (source.Any())
        {
            if (source.Take(count).SequenceEqual(other))
                return true;
            source = source.Skip(1).ToList();
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        bufferMod = new List<KeyCode>();
        Dictionary<KeyCode, bool> oldPressedKeys = currPressedKeys;
        currPressedKeys = new Dictionary<KeyCode, bool>();
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(key) && !currPressedKeys.ContainsKey(key))
            {
                if (oldPressedKeys.ContainsKey(key))
                {
                    currPressedKeys.Add( key, oldPressedKeys[key] );
                }
                else
                {
                    currPressedKeys.Add(key, false);
                }
            }
        }

        bool keysDown = !currPressedKeys.Keys.ToList().TrueForAll(oldPressedKeys.Keys.ToList().Contains);
        bool keysUp = !oldPressedKeys.Keys.ToList().TrueForAll(currPressedKeys.Keys.ToList().Contains) || currPressedKeys.Count == 0;
        if (currPressedKeys.Count > 0 && (keysDown || keysUp))
        {
            foreach(KeyCode key in currPressedKeys.Keys)
            {
                if (!currPressedKeys[key])
                {
                    bufferMod.Add(key);
                }
            }
            
        }

        foreach (List<KeyCode> simultaneouskeysList in keyBindings.Keys)
        {
            var pair = keyBindings[simultaneouskeysList];
            if (pair.isExclusive && (bufferMod.Count > simultaneouskeysList.Count || currPressedKeys.Count > simultaneouskeysList.Count))
            {
                continue;
            }
            switch (pair.pressType)
            {
                case ButtonPressType.DOWN:
                    if (bufferMod.Count < simultaneouskeysList.Count)
                    {
                        continue;
                    }

                    if (!(simultaneouskeysList.TrueForAll(delegate(KeyCode key) {
                        return (bufferMod.Contains(key) || key == (KeyCode)(-1));
                    }) && keysDown))
                    {
                        continue;
                    }
                    
                    break;
                case ButtonPressType.UP:
                    if (bufferMod.Count < simultaneouskeysList.Count)
                    {
                        continue;
                    }

                    if (!(simultaneouskeysList.TrueForAll(delegate (KeyCode key) {
                        return (bufferMod.Contains(key) || key == (KeyCode)(-1));
                    }) && keysUp))
                    {
                        continue;
                    }
                    
                    break;
                case ButtonPressType.PRESSED:
                    if (currPressedKeys.Count < simultaneouskeysList.Count)
                    {
                        continue;
                    }
                    if (!(simultaneouskeysList.TrueForAll(delegate (KeyCode key) {
                        return (currPressedKeys.Keys.ToList().Contains(key) || key == (KeyCode)(-1));
                    })))
                    {
                        continue;
                    }

                    break;
            }
            pair.callback(bufferMod);
            foreach(KeyCode key in simultaneouskeysList)
            {
                currPressedKeys[key] = true;
            }

            string print = "";
            foreach (KeyCode key in bufferMod)
            {
                print += key.ToString() + ",";
            }
            Debug.Log("Pressed Key: [" + print + "]");
            //currPressedKeys = currPressedKeys.Except(simultaneouskeysList).ToList();
        }


    }

    public void AddKeyBinding(List<KeyCode> keys, ButtonPressType pressType, CallBack callback, bool isExlusive)
    {
        keyBindings.Add(keys, new KeyBindingData() { pressType = pressType, callback = callback, isExclusive = isExlusive});
    }

    public void ChangeKeyBinding(List<KeyCode> keys, ButtonPressType pressType, CallBack callback, bool isExlusive)
    {
        if (keyBindings.ContainsKey(keys))
        {
            keyBindings[keys] = new KeyBindingData() { pressType = pressType, callback = callback, isExclusive = isExlusive };
        }
            
    }
    
    public void RemoveKeyBinding(List<KeyCode> keys)
    {
        if (keyBindings.ContainsKey(keys))
        {
            keyBindings.Remove(keys);
        }

    }

    public void RemoveAllKeyBindings()
    {
        keyBindings.Clear();
    }
    
    internal bool ContainsKeyBinding(List<KeyCode> generatedKeyCombo)
    {
        foreach(List<KeyCode> combo in keyBindings.Keys)
        {
            bool comboContains = true;
            foreach(KeyCode key in generatedKeyCombo)
            {
                if (!combo.Contains(key))
                {
                    comboContains = false;
                }
            }
            if (comboContains)
            {
                return true;
            }
        }
        return false;
    }
}


