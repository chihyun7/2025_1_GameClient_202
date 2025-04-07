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

    [SerializeField] private Image portraitImage;          //캐릭터 초상화 이미지  UI 요소 추가

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
    private Coroutine typingCoroutine;   // 코루틴 선언

    private DialogSO currentDIalog;

    private void Awake()
    {
        if (instance == null)  //싱글톤 패턴구현
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
            dialogDatabase.Initailize();  //초기화
        }
        else
        {
            Debug.LogError("Dialog Database is not assigned to dialog Manager");
        }
        if (NextButton != null)
        {
            NextButton.onClick.AddListener(NextDialog);          // 버튼 리스너 등록
        }
        else
        {
            Debug.LogError("Next Button is not assigned!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // UI초기화 후 대화시작 (Id 1)
        CloseDialog();
        StartDialog(1); // 자동으로 첫 대화시작
    }

    // Update is called once per frame
    void Update()
    {

    }

    // id로 대화시작
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

    // diloggSO로 대화시작
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
        characterNameText.text = currentDIalog.chracterName;      //캐릭터 이름설정

        //대화 텍스트 설정 부분 수정
        if(useTypewriterEffect)
        {
            StartTypingEffect(currentDIalog.text);
        }
        else
        {
            dialogText.text = currentDIalog.text;
        }


                                 //대화 텍스트 설정

        //초상화 설정 (새로추가)
        if (currentDIalog.portrait != null)
        {
            portraitImage.sprite = currentDIalog.portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else if (string.IsNullOrEmpty(currentDIalog.portraitPath))
        {
            // resource 폴더에서 이미지롣
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
            portraitImage.gameObject.SetActive(false);   // 초상화가 없으면 이미지 비활성화
        }

        // 선택지 표시
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

    public void NextDialog()  // 다음대화로 진행
    { 

        if(isTyping)       // 타이핑중이면 타이핑완료쳐리
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

    //텍스트 타이밍 효과 코루틴
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

    //타이핑 효과 중지

    private void StopTypingEffext()
        {
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    //타이핑 효과 시작
    private void StartTypingEffect(string text)
    {
        isTyping = true;
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    public void CloseDialog()                                      //대화종료
    {
        dialogPanel.SetActive(false);
        currentDIalog = null;
        StopTypingEffext();                                 // 타이핑 효과 중지 추가
    }

    // 선택지 초기화
    private void ClearChoices()
    {
        foreach(Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }
        choicesPanel.SetActive(false);
    }

    // 선택지 처리
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

    // 선택지 표시

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
                DialogChoiceSO choiceSO = choice;            //람다식에서 사용하기 위해서 지역 변수에 할당
                button.onClick.AddListener(() => SelectChoice(choiceSO));
            }
        }
    }
}
