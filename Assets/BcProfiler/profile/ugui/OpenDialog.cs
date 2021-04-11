using UnityEngine;
using System.Collections;

public class OpenDialog : MonoBehaviour 
{
    public GameObject dialogGo = null;
    public GameObject closedDialogGo = null;

    public void onClick()
    {
        if (null != dialogGo)
            dialogGo.SetActive(true);
    }

    public void onClickWithCloseDlg()
    {
        if (null != dialogGo)
            dialogGo.SetActive(true);

        if (null != closedDialogGo)
            closedDialogGo.SetActive(false);
    }
}
