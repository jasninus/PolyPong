using System;
using UnityEngine;

internal sealed class KeyManager : MonoBehaviour
{
    private Key listeningKey;

    public event Action StoppedListening;

    private bool DoListen => listeningKey != null;

    public void StartListening(Key key)
    {
        if (DoListen)
        {
            StopListening();
        }

        listeningKey = key;
    }

    public void StopListening()
    {
        listeningKey = null;
        StoppedListening?.Invoke();
    }

    private void Update()
    {
        if (DoListen)
        {
            if (listeningKey.Listen())
            {
                StopListening();
            }
        }
    }

    [Serializable]
    public class Key
    {
        private static readonly KeyCode[] IrregularKeys =
        {
            KeyCode.Space,
            KeyCode.KeypadEnter,
            KeyCode.LeftShift,
            KeyCode.RightShift,
            KeyCode.LeftAlt,
            KeyCode.RightAlt,
            KeyCode.Return,
            KeyCode.Tab
        };

        [SerializeField]
        private KeyCode defaultCode;

        public KeyCode Code { get; private set; }

        public bool GetKey()
        {
            return Input.GetKey(Code);
        }

        public bool GetKeyDown()
        {
            return Input.GetKeyDown(Code);
        }

        public bool GetKeyUp()
        {
            return Input.GetKeyUp(Code);
        }

        public bool Listen()
        {
            if (Input.anyKeyDown)
            {
                for (int i = 0; i < IrregularKeys.Length; i++)
                {
                    if (Input.GetKeyDown(IrregularKeys[i]))
                    {
                        Code = IrregularKeys[i];
                        return true;
                    }
                }

                var input = Input.inputString;
                if (!string.IsNullOrEmpty(input))
                {
                    string keyCodeName = input.Substring(0, 1).ToUpper();

                    if (Enum.IsDefined(typeof(KeyCode), keyCodeName))
                    {
                        Code = Parse(keyCodeName);
                        return true;
                    }

                    // Assume length of 1
                    if (char.IsDigit(keyCodeName[0]))
                    {
                        Code = Parse("Alpha" + keyCodeName);
                        return true;
                    }
                }
            }

            return false;
        }

        public void Load(string name)
        {
            Code = PlayerPrefs.HasKey(name) ? Parse(PlayerPrefs.GetString(name)) : defaultCode;
        }

        public void Save(string name)
        {
            PlayerPrefs.SetString(name, Code.ToString());
        }

        private KeyCode Parse(string keyCodeName)
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), keyCodeName);
        }
    }
}