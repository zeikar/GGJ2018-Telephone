using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : MonoBehaviour {

    public void onNormalClick()
    {
        Title.Instance.onClick();
    }

    public void onInfiClick()
    {
        Title.Instance.onInfiClick();
    }

    public void onHelpClick()
    {
        Title.Instance.onHelpClick();
    }
}
