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
        SceneManager.LoadScene(5);
    }

    public void host2Main()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
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
