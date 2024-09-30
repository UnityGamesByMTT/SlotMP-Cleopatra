using UnityEngine;
using UnityEngine.UI;

public class BoxUIScript : MonoBehaviour
{
    internal bool isAnim = false;
    [SerializeField]
    private Image Box_Image;
    [SerializeField]
    private GameObject BoxImage_Object;

    internal void TurnOnBoxes(Sprite bg)
    {
        if (isAnim)
        {
            if (Box_Image) Box_Image.sprite = bg;
            if (BoxImage_Object) BoxImage_Object.SetActive(true);
        }
    }

    internal void TurnOffBoxes()
    {
        if (BoxImage_Object) BoxImage_Object.SetActive(false);
    }
}
