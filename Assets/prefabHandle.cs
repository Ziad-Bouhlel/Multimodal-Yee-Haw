using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class prefabHandle : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        // Récupérer le composant Button sur cet objet
        button = GetComponent<Button>();


        if (button != null)
        {
            GetComponent<Image>().color = Color.blue;

            button.onClick.AddListener(OnPrefabClicked);
        }
    }

    private void OnPrefabClicked()
    {
        GetComponent<Image>().color = Color.red;



        TimeSelector.Instance.OnSelect(name);
    }
}
