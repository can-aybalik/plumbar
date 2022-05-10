using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public InputField email, password;
    public Button login;
    public string json;
    public Text info_text;
    public bool try_login;
    public static string user_id = "";
    public static string name = "";
    public static string surname = "";
    public static string mail = "";

    string url = "http://kilometretakip.site/PlumbAR/dbOperations.php";

    public void login_button()
    {
        if (email.text.Length < 1 && password.text.Length < 1){
            info_text.text = "Please fill all the fields to login!";
            return;
        }
        try_login = true;
    }

    void Start()
    {
        try_login = false;
    }

    void Update()
    {
        if(try_login == true)
        {
            StartCoroutine(HandleLogin());
            try_login = false;
        }
    }

    IEnumerator HandleLogin()
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "selectUser");
        form.AddField("email", email.text);
        form.AddField("password", password.text);

        UnityWebRequest conn = UnityWebRequest.Post(url, form);
        yield return conn.SendWebRequest();

        json = conn.downloadHandler.text;

        foreach (char c in json)
        {
            if (c == ':')
            {
                continue;
            }
            if (Char.IsNumber(c))
            {
                user_id += c;
            }
            if (c == ',')
            {
                break;
            }
        }

        

        Newtonsoft.Json.Linq.JObject my_json = Newtonsoft.Json.Linq.JObject.Parse(json);
        //Newtonsoft.Json.Linq.JToken myvar = my_json.GetValue("response");
        //Debug.Log(myvar.Value<string>("id"));
        Debug.Log(my_json["response"]["id"]);

        name = (String)my_json["response"]["name"];
        surname = (String)my_json["response"]["surname"];
        mail = (String)my_json["response"]["email"];

        if (json.Length != 0){
            //Debug.Log(json);
            if (!json.Contains("null"))
            {
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
            }
            else
            {
                info_text.text = "There is no user with given credentials!";
            }
        }
    }
}
