using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BonusController : MonoBehaviour
{
    [SerializeField] private SlotBehaviour slotManager;
    [SerializeField] private SocketIOManager SocketManager;
    [SerializeField] private AudioController _audioManager;
    [SerializeField] private ImageAnimation BonusOpen_ImageAnimation;
    [SerializeField] private ImageAnimation BonusClose_ImageAnimation;
    [SerializeField] private ImageAnimation BonusInBonus_ImageAnimation;
    [SerializeField] private GameObject BonusGame_Panel;
    [SerializeField] private GameObject BonusOpeningUI;
    [SerializeField] private GameObject BonusClosingUI;
    [SerializeField] private GameObject BonusInBonusUI;
    [SerializeField] private TMP_Text FSnum_Text;
    [SerializeField] private TMP_Text BonusOpeningText;
    [SerializeField] private TMP_Text BonusClosingText;
    [SerializeField] private TMP_Text BonusInBonusText;

    private Coroutine BonusRoutine;

    internal void StartBonus(int freespins)
    {
        if (FSnum_Text) FSnum_Text.text = freespins.ToString();
        if (BonusOpeningText) BonusOpeningText.text = freespins.ToString() + " FREE SPINS";
        if (BonusGame_Panel) BonusGame_Panel.SetActive(true);
        BonusRoutine = StartCoroutine(BonusGameStartRoutine(freespins));
    }

    private IEnumerator BonusGameStartRoutine(int spins)
    {
        if (BonusOpen_ImageAnimation) BonusOpen_ImageAnimation.StartAnimation();
        //yield return new WaitForSecondsRealtime(1.1f); //waiting for animation
        yield return new WaitUntil(() => BonusOpen_ImageAnimation.rendererDelegate.sprite == BonusOpen_ImageAnimation.textureArray[16]);

        Debug.Log("Here");

        BonusOpen_ImageAnimation.PauseAnimation();
        BonusOpeningUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        BonusOpeningUI.SetActive(false);
        BonusOpen_ImageAnimation.ResumeAnimation();

        //yield return new WaitForSecondsRealtime(.4f); //waiting for animation to finish.
        yield return new WaitUntil(() => BonusOpen_ImageAnimation.rendererDelegate.sprite == BonusOpen_ImageAnimation.textureArray[BonusOpen_ImageAnimation.textureArray.Count-1]);

        yield return new WaitForSeconds(1f);

        slotManager.FreeSpin(spins);
    }

    internal IEnumerator BonusInBonus()
    {
        BonusInBonus_ImageAnimation.StartAnimation();

        yield return new WaitUntil(() => BonusInBonus_ImageAnimation.rendererDelegate.sprite == BonusInBonus_ImageAnimation.textureArray[5]);

        BonusInBonus_ImageAnimation.PauseAnimation();
        BonusInBonusText.text = SocketManager.resultData.freeSpins.count.ToString() + " FREE SPINS";
        BonusInBonusUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        BonusInBonusUI.SetActive(false);
        BonusInBonus_ImageAnimation.ResumeAnimation();

        yield return new WaitUntil(() => BonusInBonus_ImageAnimation.rendererDelegate.sprite == BonusInBonus_ImageAnimation.textureArray[BonusInBonus_ImageAnimation.textureArray.Count-1]);

        yield return new WaitForSeconds(1f);

        slotManager.FreeSpin(SocketManager.resultData.freeSpins.count);
    }

    internal IEnumerator BonusGameEndRoutine()
    {
        BonusClose_ImageAnimation.StartAnimation();

        //yield return new WaitForSecondsRealtime(.45f); //waiting for animation to finish.
        yield return new WaitUntil(() => BonusClose_ImageAnimation.rendererDelegate.sprite == BonusClose_ImageAnimation.textureArray[6]);

        BonusClose_ImageAnimation.PauseAnimation();
        BonusClosingUI.SetActive(true);

        yield return new WaitForSeconds(3f);
        BonusClosingUI.SetActive(false);
        BonusClose_ImageAnimation.ResumeAnimation();

        yield return new WaitUntil(()=> BonusClose_ImageAnimation.rendererDelegate.sprite == BonusClose_ImageAnimation.textureArray[BonusClose_ImageAnimation.textureArray.Count-1]);

        if (BonusGame_Panel) BonusGame_Panel.SetActive(false);

    }
}
