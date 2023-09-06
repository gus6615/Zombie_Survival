using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject CtrlUI;
    [SerializeField] private GameObject PrintUI;
    [SerializeField] private GameObject AI;
    [SerializeField] private GameObject info;
    private Image backgroundImage;
    private Text dialog;

    private int sceneIndex;
    private bool isButton;
    public bool isTutorialDone;

    // Start is called before the first frame update
    void Start()
    {
        if (!SaveScript.saveData.isTutorial)
        {
            info.SetActive(false);
            Destroy(this.gameObject);
        }
        else
        {
            backgroundImage = info.GetComponentInChildren<Image>();
            dialog = info.GetComponentInChildren<Text>();
            isTutorialDone = false;
            CtrlUI.SetActive(false);
            PrintUI.SetActive(false);
            AI.SetActive(false);
            info.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isButton && !isTutorialDone)
        {
            switch (sceneIndex)
            {
                case 1:
                    SetDialog("아래에 있는 케릭터가 플레이어입니다.");
                    break;
                case 2:
                    SetDialog("지금 표시된 아이콘은 조준과 사격입니다.");
                    CtrlUI.SetActive(true);
                    break;
                case 3:
                    SetDialog("좀비는 앞, 뒤에서 나오므로 정확한 조준을 통해 처리하면 됩니다.");
                    break;
                case 4:
                    SetDialog("지금 표시된 아이콘은 플레이어의 정보를 알려줍니다.");
                    PrintUI.SetActive(true);
                    break;
                case 5:
                    SetDialog("최상단에 표시된 아이콘은 플레이어의 체력, 탄창 수, 골드, 점수를 나타냅니다.");
                    break;
                case 6:
                    SetDialog("그리고 체력 밑에 회색 바와 회색 점은 플레이어의 위치를 나타냅니다.");
                    break;
                case 7:
                    SetDialog("회색 점이 회색 바의 끝에 도달하게 된다면 게임을 클리어하게 됩니다.");
                    break;
                case 8:
                    SetDialog("방금 플레이어 옆에 추가된 케릭터는 용병입니다.");
                    AI.SetActive(true);
                    break;
                case 9:
                    SetDialog("용병은 좀비를 스스로 감지하여 공격합니다. 플레이어에게 큰 도움을 줍니다.");
                    break;
                case 10:
                    SetDialog("플레이어의 총기나 용병은 상점에서 구매가 가능합니다.");
                    break;
                case 11:
                    SetDialog("플레이어의 기본 능력치 및 총기 성능 업그레이드가 가능하며 용병 또한 업그레이드가 가능합니다.");
                    break;
                case 12:
                    SetDialog("지금부터 간단한 테스트가 시작될 예정입니다.");
                    break;
                case 13:
                    SetDialog("준비가 되셨다면 다음을 눌러 테스트를 통과하십시오.");
                    break;
                case 14:
                    isTutorialDone = true;
                    info.SetActive(false);
                    break;
            }

            isButton = false;
        }
    }

    private void SetDialog(string data)
    {
        info.SetActive(true);
        dialog.text = data;
    }

    private void SetUnvisible()
    {
        info.SetActive(false);
    }

    public void ButtonOn()
    {
        sceneIndex++;
        isButton = true;
    }
}
