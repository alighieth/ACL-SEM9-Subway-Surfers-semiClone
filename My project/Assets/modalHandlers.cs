using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modalHandlers : MonoBehaviour
{
    public GameObject HowPlayPanel;
    public GameObject CreditsPanel;
    //public GameObject HowPlayPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void openHowPlayModalPanel()
    {
        HowPlayPanel.SetActive(true);
    }

    public void closeHowPlayModalPanel()
    {
        HowPlayPanel.SetActive(true);
    }

    public void openCreditsModalPanel()
    {
        CreditsPanel.SetActive(true);
    }

    public void closeCreditsModalPanel()
    {
        CreditsPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
