using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHide : MonoBehaviour
{
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private GameObject hideButton = null;
    [SerializeField] private GameObject showButton = null;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void whenButtonClicked()
    {
        if (hideButton.activeInHierarchy == true)
        {
            hideButton.SetActive(false);
            showButton.SetActive(true);
            chatUI.SetActive(false);
        }
        else
        {
            hideButton.SetActive(true);
            showButton.SetActive(false);
            chatUI.SetActive(true);
        }
    }
}
