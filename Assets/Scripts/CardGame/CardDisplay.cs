using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;                   // ī�� ������
    public int cardIndex;   

    // 3d ī�� ���
    public MeshRenderer cardRender;          // ī�� ������
    public TextMeshPro nameText;             //�̸� �ؽ�Ʈ
    public TextMeshPro costText;             //��� �ؽ�Ʈ
    public TextMeshPro attackText;           //���ݷ� ȿ�� �ؽ�Ʈ
    public TextMeshPro descriptionText;      //���� �ؽ�Ʈ

    //ī�����
    private bool isDragging = false;
    private Vector3 originalPosition; // �巡�� �� ������ġ

    // ���̾� ����ũ
    public LayerMask enemyLayer;            // �� ���̾�
    public LayerMask playerLayer;           // �÷��̾� ���̾�
    // Start is called before the first frame update
    void Start()
    {
        // ���̾� ����ũ ����
        playerLayer = LayerMask.GetMask("Player");
        enemyLayer = LayerMask.GetMask("Enemy");


        SetupCard(cardData);
    }

    // ī�� ������ ����
    public void SetupCard(CardData data)
    {

        cardData = data;

        //3d �ؽ�Ʈ ������Ʈ
        if (nameText != null) nameText.text = data.cardName;
        if (costText != null) costText.text = data.manacost.ToString();
        if (attackText != null) attackText.text = data.effectAmount.ToString();
        if (descriptionText != null) descriptionText.text = data.description;

        //ī�� �ؽ��� ����
        if (cardRender != null && data.artwork != null)
        {
            Material cardMaterial = cardRender.material;
            cardMaterial.mainTexture = data.artwork.texture;
        }
    }

    private void OnMouseDown()
    {
        //�巹�� ���۽� ���� ��ġ ����
        originalPosition = transform.position;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            //���콺  ��ġ�� ī�� �̵�
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(originalPosition).z;  
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z); 
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        //����ĳ��Ʈ�� Ÿ�� ����
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // ī�� ��� ���� ������ ����
        bool cardUsed = false;

        //�� ���� �η� �ߴ��� �˻�
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
            // ������ ���� ȿ�� ����
            CaracterStats enemyStats = hit.collider.GetComponent<CaracterStats>();

            if (enemyStats != null)
            {
                if(cardData.cardType == CardData.CardType.Attack)
                {
                    //���� ī��� ������ �ֱ�
                    enemyStats.TakeDamage(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} ī��� ������ {cardData.effectAmount}�������� �ԷȽ��ϴ�. ");
                    cardUsed = true;
                }
                else
                {
                    Debug.Log("�� ī��� ������ ����� �� �����ϴ�.");
                }
            }
        }
        else if(Physics.Raycast(ray,out hit, Mathf.Infinity, playerLayer))
        {
            // �÷��̾�� �� ȿ�� ����
            CaracterStats playerStats = hit.collider.GetComponent<CaracterStats>();

            if (playerStats != null)
            {
                if (cardData.cardType == CardData.CardType.Heal)
                {
                    // ��ī��� ȸ���ϱ�
                    playerStats.Heal(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} īƮ�� �÷��̾��� ü����{cardData.effectAmount}ȸ���߽��ϴ�!");
                    cardUsed= true;
                }
                else
                {
                    Debug.Log("�� ī��� �÷��̾�� ����� �� �����ϴ�.");
                }
            }
        }

        //ī�带 ������� �ʾҴٸ� ���� ��ġ�� �µ�����
        if(!cardUsed)
        {
            transform.position = originalPosition;
        }
        else
        {
            Destroy(gameObject);    
        }
    }
}
