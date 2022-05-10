using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour{
    public InputField user_name, surname, email, password;
    public Button register;
    public string Json;
    public Text info_text;
    public bool try_register;
    public static string user_id_static;
    public static string name_static;
    public static string surname_static;
    public static string mail_static;

    public Newtonsoft.Json.Linq.JObject my_json;

    string url = "http://kilometretakip.site/PlumbAR/dbOperations.php";

    public void register_button(){
        if (user_name.text.Length < 1 || surname.text.Length < 1 || email.text.Length < 1 || password.text.Length < 1){
            info_text.text = "Please fill all of the fields!";
            return;
        }

        StartCoroutine(HandleRegister());
    }

    void Start(){
        try_register = false;
    }

    void Update(){
        
    }

    IEnumerator HandleRegister(){

        WWWForm form = new WWWForm();
        form.AddField("operation", "insertUser");
        form.AddField("name", user_name.text);
        form.AddField("surname", surname.text);
        form.AddField("email", email.text);
        form.AddField("password", password.text);

        UnityWebRequest conn = UnityWebRequest.Post(url, form);
        yield return conn.SendWebRequest();

        Json = conn.downloadHandler.text;

        if(Json.Length > 5)
           Debug.Log(Json);

        my_json = Newtonsoft.Json.Linq.JObject.Parse(Json);

        if ((String)my_json["response"] == "OK")
        {
            name_static = user_name.text;
            surname_static = surname.text;
            mail_static = email.text;

            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
    }

}
