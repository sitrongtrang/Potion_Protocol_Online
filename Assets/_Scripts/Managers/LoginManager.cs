using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Newtonsoft.Json; // optional, if using TextMeshPro UI

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    [SerializeField] private StaticURLSO _loginUrl;

    public void OnLoginButtonPressed()
    {
        StartCoroutine(SendLoginRequest(usernameField.text, passwordField.text));
    }

    IEnumerator SendLoginRequest(string username, string password)
    {
        LoginData loginData = new LoginData
        {
            Username = username,
            Password = password
        };

        string json = JsonConvert.SerializeObject(loginData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(_loginUrl.StaticURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LoginSuccess loginSuccess = JsonConvert.DeserializeObject<LoginSuccess>(request.downloadHandler.text);
            NetworkManager.Instance.SetAuthenToken(loginSuccess.LoginSuccessDat.Token);
            NetworkManager.Instance.SetClientId(loginSuccess.LoginSuccessDat.UserId);
            NetworkManager.Instance.Authenticate();
        }
        else
        {
            Debug.LogError("Login failed: " + request.error);
        }
    }


    [Header("Test")]
    [SerializeField] private string _username;
    [SerializeField] private string _password;
    [ContextMenu("Login")]
    public void Login()
    {
        StartCoroutine(SendLoginRequest(_username, _password));
    }
}
