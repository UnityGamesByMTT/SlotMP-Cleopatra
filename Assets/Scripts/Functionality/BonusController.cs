using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BonusController : MonoBehaviour
{
    [SerializeField]
    private Button Spin_Button;
    [SerializeField]
    private RectTransform Wheel_Transform;
    [SerializeField]
    private BoxCollider2D[] point_colliders;
    [SerializeField]
    private TMP_Text[] Bonus_Text;
    [SerializeField]
    private GameObject Bonus_Object;
    [SerializeField]
    private SlotBehaviour slotManager;
    [SerializeField]
    private AudioController _audioManager;
    [SerializeField]
    private GameObject PopupPanel;
    [SerializeField]
    private Transform Win_Transform;
    [SerializeField]
    private Transform Loose_Transform;

    internal bool isCollision = false;

    private Tween wheelRoutine;

    private float elasticIntensity = 5f;

    private int stopIndex = 0;

    [SerializeField] private ImageAnimation BonusOpen_ImageAnimation;
    [SerializeField] private ImageAnimation BonusClose_ImageAnimation;
    //[SerializeField] private GameObject BonusGameButtonPanel_Object;
    [SerializeField] private GameObject BonusGame_Panel;
    [SerializeField] private GameObject BonusClosingUI;

    private Coroutine BonusRoutine;


    private void Start()
    {
        if (Spin_Button) Spin_Button.onClick.RemoveAllListeners();
        if (Spin_Button) Spin_Button.onClick.AddListener(Spinbutton);
    }

    internal void StartBonus()
    {
        //ResetColliders();
        //if (PopupPanel) PopupPanel.SetActive(false);
        //if (Win_Transform) Win_Transform.gameObject.SetActive(false);
        //if (Loose_Transform) Loose_Transform.gameObject.SetActive(false);
        //if (_audioManager) _audioManager.SwitchBGSound(true);
        //if (Spin_Button) Spin_Button.interactable = true;
        //stopIndex = stop;
        //if (Bonus_Object) Bonus_Object.SetActive(true);

        if (BonusGame_Panel) BonusGame_Panel.SetActive(true);
        if (BonusOpen_ImageAnimation) BonusOpen_ImageAnimation.StartAnimation();

        if (BonusRoutine != null)
        {
            StopCoroutine(BonusRoutine);
            BonusRoutine = null;
        }
        BonusRoutine = StartCoroutine(BonusGameRoutine());
    }

    private IEnumerator BonusGameRoutine()
    {
        Debug.Log("Started Routine");

        yield return new WaitUntil(() => BonusOpen_ImageAnimation.textureArray[BonusOpen_ImageAnimation.textureArray.Count-1] == BonusOpen_ImageAnimation.rendererDelegate.sprite);

        Debug.Log("Here");

        yield return new WaitForSeconds(2f);

        slotManager.FreeSpin(1);

        Debug.Log("Started");

        //make all the lines below a function to end the bonus game

        yield return new WaitUntil(() => !slotManager.IsFreeSpin);

        yield return new WaitForSeconds(2f);

        BonusClose_ImageAnimation.StartAnimation();

        yield return new WaitUntil(() => BonusClose_ImageAnimation.textureArray[15] == BonusOpen_ImageAnimation.rendererDelegate.sprite);
        BonusClose_ImageAnimation.PauseAnimation();
        BonusClosingUI.SetActive(true);

        yield return new WaitForSeconds(2f);
        BonusClosingUI.SetActive(false);
    } 

    private void Spinbutton()
    {
        isCollision = false;
        if (Spin_Button) Spin_Button.interactable = false;
        RotateWheel();
        DOVirtual.DelayedCall(2f, () =>
        {
            TurnCollider(stopIndex);
        });
    }

    internal void PopulateWheel(List<string> bonusdata)
    {
        for (int i = 0; i < bonusdata.Count; i++)
        {
            if (bonusdata[i] == "-1") 
            {
                if (Bonus_Text[i]) Bonus_Text[i].text = "NO \nBONUS";
            }
            else
            {
                if (Bonus_Text[i]) Bonus_Text[i].text = bonusdata[i];
            }
        }
    }

    private void RotateWheel()
    {
        if (Wheel_Transform) Wheel_Transform.localEulerAngles = new Vector3(0, 0, 359);
        if (Wheel_Transform) wheelRoutine =  Wheel_Transform.DORotate(new Vector3(0, 0, 0), 1, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        _audioManager.PlayBonusAudio("cycleSpin");
    }

    private void ResetColliders()
    {
        foreach(BoxCollider2D col in point_colliders)
        {
            col.enabled = false;
        }
    }

    private void TurnCollider(int point)
    {
        if (point_colliders[point]) point_colliders[point].enabled = true;
    }

    internal void StopWheel()
    {
        if (wheelRoutine != null)
        {
            wheelRoutine.Pause(); // Pause the rotation

            // Apply an elastic effect to the paused rotation
            Wheel_Transform.DORotate(Wheel_Transform.eulerAngles + Vector3.forward * Random.Range(-elasticIntensity, elasticIntensity), 1f)
                .SetEase(Ease.OutElastic);
        }
        if (Bonus_Text[stopIndex].text.Equals("NO \nBONUS")) 
        {
            if (Loose_Transform) Loose_Transform.gameObject.SetActive(true);
            if (Loose_Transform) Loose_Transform.localScale = Vector3.zero;
            if (PopupPanel) PopupPanel.SetActive(true);
            if (Loose_Transform) Loose_Transform.DOScale(Vector3.one, 1f);
            PlayWinLooseSound(false);
        }
        else
        {
            if (Win_Transform) Win_Transform.gameObject.SetActive(true);
            if (Win_Transform) Win_Transform.localScale = Vector3.zero;
            if (PopupPanel) PopupPanel.SetActive(true);
            if (Win_Transform) Win_Transform.DOScale(Vector3.one, 1f);
            PlayWinLooseSound(true);
        }
        DOVirtual.DelayedCall(3f, () =>
        {
            ResetColliders();
            if (_audioManager) _audioManager.SwitchBGSound(false);
            if (Bonus_Object) Bonus_Object.SetActive(false);
            slotManager.CheckWinPopups();
        });
    }

    internal void PlayWinLooseSound(bool isWin)
    {
        if (isWin)
        {
            _audioManager.PlayBonusAudio("win");
        }
        else
        {
            _audioManager.PlayBonusAudio("lose");
        }
    }
}
