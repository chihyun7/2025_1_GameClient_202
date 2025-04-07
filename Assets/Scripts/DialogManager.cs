using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance { get; private set; }

    [Header("Dialog References")]
    [SerializeField] private DialogDatabaseSO dialogDatabase;

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;

    [SerializeField] private Image portraitImage;          //ĳ���� �ʻ�ȭ �̹���  UI ��� �߰�

    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Button NextButton;

    [Header("Dialog Settings")]
    [SerializeField] private float typingspeed = 0.05f;
    [SerializeField] private bool useTypewriterEffect = true;

    [Header("DialogChoices")]
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private GameObject choiceButtonPrefab;

    private bool isTyping = false;
    private Coroutine typingCoroutine;   // �ڷ�ƾ ����

    private DialogSO currentDIalog;

    private void Awake()
    {
        if (instance == null)  //�̱��� ���ϱ���
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (dialogDatabase != null)
        {
            dialogDatabase.Initailize();  //�ʱ�ȭ
        }
        else
        {
            Debug.LogError("Dialog Database is not assigned to dialog Manager");
        }
        if (NextButton != null)
        {
            NextButton.onClick.AddListener(NextDialog);          // ��ư ������ ���
        }
        else
        {
            Debug.LogError("Next Button is not assigned!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // UI�ʱ�ȭ �� ��ȭ���� (Id 1)
        CloseDialog();
        StartDialog(1); // �ڵ����� ù ��ȭ����
    }

    // Update is called once per frame
    void Update()
    {

    }

    // id�� ��ȭ����
    public void StartDialog(int dialogId)
    {
        DialogSO dialog = dialogDatabase.GetDialogByld(dialogId);
        if (dialog != null)
        {
            StartDialog(dialog);
        }
        else
        {
            Debug.LogError($"Dialgo with ID {dialogId} not found!");
        }
    }

    // diloggSO�� ��ȭ����
    public void StartDialog(DialogSO dialog)
    {
        if (dialog == null) return;

        currentDIalog = dialog;
        ShowDialog();
        dialogPanel.SetActive(true);

    }

    public void ShowDialog()
    {
        if (currentDIalog == null) return;
        characterNameText.text = currentDIalog.chracterName;      //ĳ���� �̸�����

        //��ȭ �ؽ�Ʈ ���� �κ� ����
        if(useTypewriterEffect)
        {
            StartTypingEffect(currentDIalog.text);
        }
        else
        {
            dialogText.text = currentDIalog.text;
        }


                                 //��ȭ �ؽ�Ʈ ����

        //�ʻ�ȭ ���� (�����߰�)
        if (currentDIalog.portrait != null)
        {
            portraitImage.sprite = currentDIalog.portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else if (string.IsNullOrEmpty(currentDIalog.portraitPath))
        {
            // resource �������� �̹�����
            Sprite portrait = Resources.Load<Sprite>(currentDIalog.portraitPath);
            if (portrait != null)
            {
                portraitImage.sprite = portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Portrait not found at path : {currentDIalog.portraitPath}");
                portraitImage.gameObject.SetActive(false);
            }

        }
        else
        {
            portraitImage.gameObject.SetActive(false);   // �ʻ�ȭ�� ������ �̹��� ��Ȱ��ȭ
        }

        // ������ ǥ��
        ClearChoices();
        if (currentDIalog.choices != null && currentDIalog.choices.Count > 0)
        {
            ShowChoices();
            NextButton.gameObject.SetActive(false );
        }
        else
        {
            NextButton.gameObject.SetActive(true);
        }
    }

    public void NextDialog()  // ������ȭ�� ����
    { 

        if(isTyping)       // Ÿ�������̸� Ÿ���οϷ��ĸ�
        {
            StopTypingEffext();
            dialogText.text = currentDIalog.text;
            isTyping = false;
            return;
        }






        if (currentDIalog != null && currentDIalog.nextild > 0)
        {
            DialogSO nextDialog = dialogDatabase.GetDialogByld(++currentDIalog.nextild);
            if (nextDialog != null)
            {
                currentDIalog = nextDialog;
                ShowDialog();
            }
            else
            {
                CloseDialog();
            }
        }
        else
        {
            CloseDialog();
        }
    }

    //�ؽ�Ʈ Ÿ�̹� ȿ�� �ڷ�ƾ
    private IEnumerator TypeText(string text)
    {
        dialogText.text = "";
        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingspeed);
        }
        isTyping = false;
    }

    //Ÿ���� ȿ�� ����

    private void StopTypingEffext()
        {
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    //Ÿ���� ȿ�� ����
    private void StartTypingEffect(string text)
    {
        isTyping = true;
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    public void CloseDialog()                                      //��ȭ����
    {
        dialogPanel.SetActive(false);
        currentDIalog = null;
        StopTypingEffext();                                 // Ÿ���� ȿ�� ���� �߰�
    }

    // ������ �ʱ�ȭ
    private void ClearChoices()
    {
        foreach(Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }
        choicesPanel.SetActive(false);
    }

    // ������ ó��
    public void SelectChoice(DialogChoiceSO choice)
    {
        if (choice != null && choice.nextId > 0)
        {
            DialogSO nextdialog = dialogDatabase.GetDialogByld(choice.nextId);
            if (nextdialog != null)
            {
                currentDIalog = nextdialog;
                ShowDialog();
            }
            else
            {
                CloseDialog();
            }
        }
        else
        {
            CloseDialog();
        }
    }

    // ������ ǥ��

    private void ShowChoices()
    {
        choicesPanel.SetActive(true);

        foreach (var choice in currentDIalog.choices)
        {
            GameObject choiceGO = Instantiate(choiceButtonPrefab, choicesPanel.transform);
            TextMeshProUGUI buttonText = choiceGO.GetComponent<TextMeshProUGUI>();
            Button button = choiceGO.GetComponent<Button>();

            if (buttonText != null)
            {
                buttonText.text = choice.text;
            }
            if (button != null)
            {
                DialogChoiceSO choiceSO = choice;            //���ٽĿ��� ����ϱ� ���ؼ� ���� ������ �Ҵ�
                button.onClick.AddListener(() => SelectChoice(choiceSO));
            }
        }
    }
}
