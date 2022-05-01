using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public Text full_name, email;
    public string json;
    string url = "http://kilometretakip.site/PlumbAR/dbOperations.php";

    // Start is called before the first frame update
    void Start()
    {
        full_name.text = Login.name + " " + Login.surname;
        email.text = Login.mail;
        StartCoroutine(getAreas());
        SphereCollider sc = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator getAreas()
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "selectOwnership");
        form.AddField("user_id", Login.user_id);

        UnityWebRequest conn = UnityWebRequest.Post(url, form);
        yield return conn.SendWebRequest();

        json = conn.downloadHandler.text;
        if (json.Length != 0)
        {
            Debug.Log(json);
        }
    }
}