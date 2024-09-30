using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;
using UnityEngine.Networking;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Button Terms_Button;
    [SerializeField]
    private Button Privacy_Button;

    [Header("Popus UI")]
    [SerializeField]
    private GameObject MainPopup_Object;

    [Header("Big Win Popup")]
    [SerializeField]
    private Image BigWin_Image;
    [SerializeField]
    private GameObject BigWinPopup_Object;
    [SerializeField]
    private TMP_Text BigWin_Text;

    [Header("Disconnection Popup")]
    [SerializeField]
    private Button CloseDisconnect_Button;
    [SerializeField]
    private GameObject DisconnectPopup_Object;

    [Header("AnotherDevice Popup")]
    [SerializeField]
    private Button CloseAD_Button;
    [SerializeField]
    private GameObject ADPopup_Object;

    [Header("Reconnection Popup")] //ask if this will be used
    [SerializeField]
    private TMP_Text reconnect_Text;
    [SerializeField]
    private GameObject ReconnectPopup_Object;

    [Header("LowBalance Popup")]
    [SerializeField]
    private Button LBExit_Button;
    [SerializeField]
    private GameObject LBPopup_Object;

    [Header("Audio Objects")]
    [SerializeField] private Button AudioON_Button;
    [SerializeField] private Button AudioOFF_Button;

    [Header("Paytable Objects")]
    [SerializeField] private GameObject PaytableMenuObject;
    [SerializeField] private Button Paytable_Button;
    [SerializeField] private Button PaytableClose_Button;

    [Header("Game Quit Objects")]
    [SerializeField] private Button Quit_Button;
    [SerializeField] private Button QuitYes_Button;
    [SerializeField] private Button QuitNo_Button;
    [SerializeField] private GameObject QuitMenuObject;

    [SerializeField]
    private AudioController audioController;

    [SerializeField] private SlotBehaviour slotManager;

    [SerializeField]
    private SocketIOManager socketManager;

    private bool isMusic = true;
    private bool isSound = true;
    private bool isExit = false;

    private int FreeSpins;

    private void Start()
    {
        if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
        if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); });

        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(CallOnExitFunction);

        if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
        if (CloseAD_Button) CloseAD_Button.onClick.AddListener(CallOnExitFunction);

        if (audioController) audioController.ToggleMute(false);

        isMusic = true;
        isSound = true;

        if (AudioON_Button) AudioON_Button.onClick.RemoveAllListeners();
        if (AudioON_Button) AudioON_Button.onClick.AddListener(delegate { AudioOnOFF(false); });

        if (AudioOFF_Button) AudioOFF_Button.onClick.RemoveAllListeners();
        if (AudioOFF_Button) AudioOFF_Button.onClick.AddListener(delegate { AudioOnOFF(true); });

        if (Quit_Button) Quit_Button.onClick.RemoveAllListeners();
        if (Quit_Button) Quit_Button.onClick.AddListener(OpenQuitPanel);

        if (QuitNo_Button) QuitNo_Button.onClick.RemoveAllListeners();
        if (QuitNo_Button) QuitNo_Button.onClick.AddListener(delegate { ClosePopup(QuitMenuObject); });

        if (QuitYes_Button) QuitYes_Button.onClick.RemoveAllListeners();
        if (QuitYes_Button) QuitYes_Button.onClick.AddListener(CallOnExitFunction);

        if (Paytable_Button) Paytable_Button.onClick.RemoveAllListeners();
        if (Paytable_Button) Paytable_Button.onClick.AddListener(OpenPaytablePanel);

        if (PaytableClose_Button) PaytableClose_Button.onClick.RemoveAllListeners();
        if (PaytableClose_Button) PaytableClose_Button.onClick.AddListener(delegate { ClosePopup(PaytableMenuObject); });

    }

    private void AudioOnOFF(bool state)
    {
        if (state) //turning audio on
        {
            AudioOFF_Button.gameObject.SetActive(false);
            AudioON_Button.gameObject.SetActive(true);
            audioController.ToggleMute(false);
        }
        else //turning audio off
        {
            AudioOFF_Button.gameObject.SetActive(true);
            AudioON_Button.gameObject.SetActive(false);
            audioController.ToggleMute(true);
        }
    }

    private void OpenQuitPanel()
    {
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        if (QuitMenuObject) QuitMenuObject.SetActive(true);
    }

    private void OpenPaytablePanel()
    {
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        if (PaytableMenuObject) PaytableMenuObject.SetActive(true);
    }

    internal void LowBalPopup()
    {
        OpenPopup(LBPopup_Object);
    }

    internal void DisconnectionPopup(bool isReconnection)
    {
        //if(isReconnection)
        //{
        //    OpenPopup(ReconnectPopup_Object);
        //}
        //else
        //{
        //    ClosePopup(ReconnectPopup_Object);
        //}
        if (!isExit)
        {
            OpenPopup(DisconnectPopup_Object);
        }
    }
        
    internal void StartBigWinPopupAnim()
    {
        if (BigWinPopup_Object) BigWinPopup_Object.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);

        BigWin_Image.rectTransform.DOScale(new Vector3(1, 1, 1), .5f);

        DOVirtual.DelayedCall(3f, () =>
        {
            BigWin_Image.rectTransform.DOScale(Vector3.zero, .5f).OnComplete(() => ClosePopup(BigWinPopup_Object));
            //ClosePopup(BigWinPopup_Object);
            slotManager.CheckPopups = false;
        });
    }

    internal void ADfunction()
    {
        OpenPopup(ADPopup_Object); 
    }

    internal void InitialiseUIData(string SupportUrl, string AbtImgUrl, string TermsUrl, string PrivacyUrl, Paylines symbolsText)
    {
        //if (Support_Button) Support_Button.onClick.RemoveAllListeners();
        //if (Support_Button) Support_Button.onClick.AddListener(delegate { UrlButtons(SupportUrl); });

        if (Terms_Button) Terms_Button.onClick.RemoveAllListeners();
        if (Terms_Button) Terms_Button.onClick.AddListener(delegate { UrlButtons(TermsUrl); });

        if (Privacy_Button) Privacy_Button.onClick.RemoveAllListeners();
        if (Privacy_Button) Privacy_Button.onClick.AddListener(delegate { UrlButtons(PrivacyUrl); });

        StartCoroutine(DownloadImage(AbtImgUrl));
        //PopulateSymbolsPayout(symbolsText);
    }

    //private void PopulateSymbolsPayout(Paylines paylines)
    //{
    //    for (int i = 0; i < SymbolsText.Length; i++)
    //    {
    //        string text = null;
    //        if (paylines.symbols[i].Multiplier[0][0] != 0)
    //        {
    //            text += "5x - " + paylines.symbols[i].Multiplier[0][0];
    //        }
    //        if (paylines.symbols[i].Multiplier[1][0] != 0)
    //        {
    //            text += "\n4x - " + paylines.symbols[i].Multiplier[1][0];
    //        }
    //        if (paylines.symbols[i].Multiplier[2][0] != 0)
    //        {
    //            text += "\n3x - " + paylines.symbols[i].Multiplier[2][0];
    //        }
    //        if (SymbolsText[i]) SymbolsText[i].text = text;
    //    }

    //    for (int i = 0; i < paylines.symbols.Count; i++)
    //    {
    //        if (paylines.symbols[i].Name.ToUpper() == "FREESPIN")
    //        {
    //            if (FreeSpin_Text) FreeSpin_Text.text = paylines.symbols[i].description.ToString();
    //        }
    //        if (paylines.symbols[i].Name.ToUpper() == "SCATTER")
    //        {
    //            if (Scatter_Text) Scatter_Text.text = paylines.symbols[i].description.ToString();
    //        }
    //        if (paylines.symbols[i].Name.ToUpper() == "JACKPOT")
    //        {
    //            if (Jackpot_Text) Jackpot_Text.text = paylines.symbols[i].description.ToString();
    //        }
    //        if (paylines.symbols[i].Name.ToUpper() == "BONUS")
    //        {
    //            if (Bonus_Text) Bonus_Text.text = paylines.symbols[i].description.ToString();
    //        }
    //        if (paylines.symbols[i].Name.ToUpper() == "WILD")
    //        {
    //            if (Wild_Text) Wild_Text.text = paylines.symbols[i].description.ToString();
    //        }
    //    }
    //}

    private void CallOnExitFunction()
    {
        isExit = true;
        audioController.PlayButtonAudio();
        slotManager.CallCloseSocket();
    }

    private void OpenPopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    private void ClosePopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(false);
        if (!DisconnectPopup_Object.activeSelf) 
        {
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
        }
    }

    private void UrlButtons(string url)
    {
        Application.OpenURL(url);
    }

    private IEnumerator DownloadImage(string url)
    {
        // Create a UnityWebRequest object to download the image
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        // Wait for the download to complete
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            // Apply the sprite to the target image
            //AboutLogo_Image.sprite = sprite;
        }
        else
        {
            Debug.LogError("Error downloading image: " + request.error);
        }
    }
}
