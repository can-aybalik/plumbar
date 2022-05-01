using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Register : MonoBehaviour{
    public InputField user_name, surname, email, password;
    public Button register;
    public string Json;
    public Text info_text;
    public bool try_register;

    string url = "http://kilometretakip.site/PlumbAR/dbOperations.php";

    public void register_button(){
        if (user_name.text.Length < 1 || surname.text.Length < 1 || email.text.Length < 1 || password.text.Length < 1){
            info_text.text = "Please fill all of the fields!";
            return;
        }

        try_register = true;
    }

    void Start(){
        try_register = false;
    }

    void Update(){
        if(try_register == true){
            StartCoroutine(HandleRegister());
            try_register = false;
        }
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
    }
}
