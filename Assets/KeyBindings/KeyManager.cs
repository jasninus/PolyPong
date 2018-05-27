using System;
using UnityEngine;

internal sealed class KeyManager : MonoBehaviour
{
    private Key listeningKey;

    public event Action StoppedListening;

    private bool DoListen
    {
        get { return this.listeningKey != null; }
    }

    public void StartListening(Key key)
    {
        if (this.DoListen)
        {
            this.StopListening();
        }

        this.listeningKey = key;
    }

    public void StopListening()
    {
        this.listeningKey = null;
        if (this.StoppedListening != null)
        {
            this.StoppedListening();
        }
    }

    private void Update()
    {
        if (this.DoListen)
        {
            if (this.listeningKey.Listen())
            {
                this.StopListening();
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
            return Input.GetKey(this.Code);
        }

        public bool GetKeyDown()
        {
            return Input.GetKeyDown(this.Code);
        }

        public bool GetKeyUp()
        {
            return Input.GetKeyUp(this.Code);
        }

        public bool Listen()
        {
            if (Input.anyKeyDown)
            {
                for (int i = 0; i < IrregularKeys.Length; i++)
                {
                    if (Input.GetKeyDown(IrregularKeys[i]))
                    {
                        this.Code = IrregularKeys[i];
                        return true;
                    }
                }

                var input = Input.inputString;
                if (!string.IsNullOrEmpty(input))
                {
                    string keyCodeName = input.Substring(0, 1).ToUpper();

                    if (Enum.IsDefined(typeof(KeyCode), keyCodeName))
                    {
                        this.Code = this.Parse(keyCodeName);
                        return true;
                    }

                    // Assume length of 1
                    if (char.IsDigit(keyCodeName[0]))
                    {
                        this.Code = this.Parse("Alpha" + keyCodeName);
                        return true;
                    }
                }
            }

            return false;
        }

        public void Load(string name)
        {
            if (PlayerPrefs.HasKey(name))
            {
                this.Code = this.Parse(PlayerPrefs.GetString(name));
            }
            else
            {
                this.Code = this.defaultCode;
            }
        }

        public void Save(string name)
        {
            PlayerPrefs.SetString(name, this.Code.ToString());
        }

        private KeyCode Parse(string keyCodeName)
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), keyCodeName);
        }
    }
}