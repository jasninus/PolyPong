using UnityEngine;
using UnityEngine.UI;

internal sealed class KeyConfigWidget : MonoBehaviour
{
    private Button button;

    private Text buttonText;

    private bool isListening = false;

    [SerializeField]
    private KeyManager.Key key;

    [SerializeField]
    private KeyManager keyManager;

    public bool IsListening
    {
        get
        {
            return this.isListening;
        }

        set
        {
            this.isListening = value;
            this.UpdateButtonText();
        }
    }

    public string Name
    {
        get { return this.name; }
    }

    private void Awake()
    {
        this.key.Load(this.Name);

        this.button = this.GetComponentInChildren<Button>();
        this.buttonText = this.button.GetComponentInChildren<Text>();
        this.button.onClick.AddListener(this.ToggleListening);
        this.UpdateButtonText();
    }

    private void OnDestroy()
    {
        this.key.Save(this.Name);
    }

    private void OnStoppedListening()
    {
        this.keyManager.StoppedListening -= this.OnStoppedListening;
        this.IsListening = false;
    }

    private void ToggleListening()
    {
        if (this.IsListening)
        {
            this.keyManager.StopListening();
        }
        else
        {
            this.IsListening = true;
        }
    }

    private void Update()
    {
        if (this.key.GetKeyDown())
        {
            Debug.Log(this.Name + " pressed!");
        }
    }

    private void UpdateButtonText()
    {
        if (this.IsListening)
        {
            this.buttonText.text = "Press any key...";
            this.keyManager.StartListening(this.key);
            this.keyManager.StoppedListening += this.OnStoppedListening;
        }
        else
        {
            this.buttonText.text = this.key.Code.ToString();
        }
    }
}