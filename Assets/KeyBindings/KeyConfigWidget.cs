using UnityEngine;
using UnityEngine.UI;

internal sealed class KeyConfigWidget : MonoBehaviour
{
    private Button button;

    private Text buttonText;

    private bool isListening;

    [SerializeField]
    private KeyManager.Key key;

    [SerializeField]
    private KeyManager keyManager;

    public bool IsListening
    {
        get
        {
            return isListening;
        }

        set
        {
            isListening = value;
            UpdateButtonText();
        }
    }

    public string Name => name;

    private void Awake()
    {
        key.Load(Name);

        button = GetComponentInChildren<Button>();
        buttonText = button.GetComponentInChildren<Text>();
        button.onClick.AddListener(ToggleListening);
        UpdateButtonText();
    }

    private void OnDestroy()
    {
        key.Save(Name);
    }

    private void OnStoppedListening()
    {
        keyManager.StoppedListening -= OnStoppedListening;
        IsListening = false;
    }

    private void ToggleListening()
    {
        if (IsListening)
        {
            keyManager.StopListening();
        }
        else
        {
            IsListening = true;
        }
    }

    private void Update()
    {
        if (key.GetKeyDown())
        {
            Debug.Log(Name + " pressed!");
        }
    }

    private void UpdateButtonText()
    {
        if (IsListening)
        {
            buttonText.text = "Press any key...";
            keyManager.StartListening(key);
            keyManager.StoppedListening += OnStoppedListening;
        }
        else
        {
            buttonText.text = key.Code.ToString();
        }
    }
}