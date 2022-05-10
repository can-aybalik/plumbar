using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void welcome2register(){
        Debug.Log("Register loading...");
        SceneManager.LoadScene("Register", LoadSceneMode.Single);
    }

    public void welcome2login(){
        Debug.Log("Login loading...");
        SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }

    public void login2welcome(){
        Debug.Log("Welcome loading...");
        SceneManager.LoadScene("Welcome", LoadSceneMode.Single);
    }

    public void register2welcome(){
        Debug.Log("Welcome loading...");
        SceneManager.LoadScene("Welcome", LoadSceneMode.Single);
    }

    public void main2HostResolve()
    {
        Debug.Log("Welcome loading...");
        SceneManager.LoadScene(5, LoadSceneMode.Single);
    }

    public void host2Main()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void logout()
    {
        Login.user_id = "";
        Login.name = "";
        Login.surname = "";
        Login.mail = "";

        Register.name_static = "";
        Register.surname_static = "";
        Register.mail_static = "";
        Register.user_id_static = "";

        SceneManager.LoadScene("Welcome", LoadSceneMode.Single);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
