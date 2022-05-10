using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class Main : MonoBehaviour
{
    public Text full_name, email;
    public string json;
    string url = "http://kilometretakip.site/PlumbAR/dbOperations.php";

    public GameObject list_item;
    public GameObject area_list;

    public pipeController pipeController;

    public string creator = "";

    // Start is called before the first frame update
    void Start()
    {

        if(Login.name == "")
        {
            full_name.text = Register.name_static + " " + Register.surname_static;
            email.text = Register.mail_static;
        
        }
        else
        {
            full_name.text = Login.name + " " + Login.surname;
            email.text = Login.mail;
        }
        StartCoroutine(getAreas());

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator getAreas()
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "selectAreasOfAUser");
        form.AddField("user_id", Login.user_id);

        UnityWebRequest conn = UnityWebRequest.Post(url, form);
        yield return conn.SendWebRequest();

        json = conn.downloadHandler.text;
        var my_json = JObject.Parse(json).Children();

        List<JToken> tokens = my_json.Children().ToList();

        foreach (var x in tokens)
        { // if 'obj' is a JObject
            Debug.Log(x);
            GameObject newListItem = Instantiate(list_item);
            newListItem.transform.parent = area_list.transform;

            newListItem.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = (String)x["area_name"]; //Area Name
            yield return StartCoroutine(selectCreatorById((String)x["creator_id"]));
            newListItem.transform.GetChild(1).transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Text>().text = creator;

            newListItem.transform.GetChild(1).transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Text>().text = (String)x["id"];

            /*
            if((String)x["creator_id"] == Login.user_id)
            {
                Debug.Log("GÝRDÝÝÝÝÝ");
                newListItem.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(true);
            }
            */

        }

        list_item.SetActive(false);

    }




    IEnumerator selectCreatorById(string user_id)
    {
        WWWForm form = new WWWForm();
        form.AddField("operation", "selectUserById");
        form.AddField("user_id", user_id);

        UnityWebRequest conn = UnityWebRequest.Post(url, form);
        yield return conn.SendWebRequest();

        json = conn.downloadHandler.text;
        JObject my_json = JObject.Parse(json);
        //Debug.Log(json);



        creator = my_json["name"] + " " + my_json["surname"];

    }

    public void listItemClicked(GameObject areaId)
    {
        pipeController.clickedAreaId = areaId.GetComponent<Text>().text;
    }
}