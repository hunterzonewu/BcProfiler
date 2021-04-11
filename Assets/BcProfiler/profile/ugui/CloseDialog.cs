using UnityEngine;
using System.Collections;

public class CloseDialog : MonoBehaviour 
{
    public GameObject dialogGo = null;
    public void onClick()
    {
        if (null != dialogGo)
            dialogGo.SetActive(false);
    }
}
