using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BonusController : MonoBehaviour
{
    [SerializeField] private SlotBehaviour slotManager;
    [SerializeField] private AudioController _audioManager;
    [SerializeField] private ImageAnimation BonusOpen_ImageAnimation;
    [SerializeField] private ImageAnimation BonusClose_ImageAnimation;
    [SerializeField] private GameObject BonusGame_Panel;
    [SerializeField] private GameObject BonusOpeningUI;
    [SerializeField] private GameObject BonusClosingUI;
    [SerializeField] private TMP_Text FSnum_Text;
    [SerializeField] private TMP_Text BonusOpeningText;
    [SerializeField] private TMP_Text BonusClosingText;

    private Coroutine BonusRoutine;

    internal void StartBonus(int freespins)
    {
        if (FSnum_Text) FSnum_Text.text = freespins.ToString();
        if (BonusOpeningText) BonusOpeningText.text = freespins.ToString() + " FREE SPINS";
        if (BonusGame_Panel) BonusGame_Panel.SetActive(true);
        if (BonusOpen_ImageAnimation) BonusOpen_ImageAnimation.StartAnimation();
        BonusRoutine = StartCoroutine(BonusGameRoutine(freespins));
    }

    private IEnumerator BonusGameRoutine(int spins)
    {
        yield return new WaitForSecondsRealtime(1f); //waiting for animation

        BonusOpen_ImageAnimation.PauseAnimation();
        BonusOpeningUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        BonusOpeningUI.SetActive(false);
        BonusOpen_ImageAnimation.ResumeAnimation();

        yield return new WaitForSecondsRealtime(.4f); //waiting for animation to finish.

        yield return new WaitForSeconds(2f);

        slotManager.FreeSpin(spins);

        //make all the lines below a function to end the bonus game

        yield return new WaitUntil(() => !slotManager.IsFreeSpin);

        //yield return new WaitForSeconds(2f);

        BonusClose_ImageAnimation.StartAnimation();

        yield return new WaitForSecondsRealtime(.45f); //waiting for animation to finish.

        BonusClose_ImageAnimation.PauseAnimation();
        BonusClosingUI.SetActive(true);

        yield return new WaitForSeconds(3f);
        BonusClosingUI.SetActive(false);
        BonusClose_ImageAnimation.ResumeAnimation();

        yield return new WaitForSecondsRealtime(1.3f);

        EndBonus();
    }

    private void EndBonus()
    {
        if (BonusGame_Panel) BonusGame_Panel.SetActive(false);

        if (BonusRoutine != null)
        {
            StopCoroutine(BonusRoutine);
            BonusRoutine = null;
        }
    }
}
