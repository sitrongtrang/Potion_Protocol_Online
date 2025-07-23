using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Newtonsoft.Json; // optional, if using TextMeshPro UI

public class HttpAuthManager : MonoBehaviour
{
    [Header("Login")]
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;

    [Header("Register")]
    public TMP_InputField confirmpasswordField;
    public TMP_InputField displaynameField;

    [Header("URLs")]
    [SerializeField] private StaticURLSO _loginUrl;
    [SerializeField] private StaticURLSO _registerUrl;

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
            // Debug.Log("Login successful: " + request.downloadHandler.text);
            // Parse token or user info if needed

            LoginSuccess loginSuccess = JsonConvert.DeserializeObject<LoginSuccess>(request.downloadHandler.text);
            //NetworkManager.Instance.SetAuthenToken(loginSuccess.LoginSuccessDat.Token);
            //NetworkManager.Instance.SetClientId(loginSuccess.LoginSuccessDat.UserId);
            //NetworkManager.Instance.Authenticate();
        }
        else
        {
            Debug.LogError("Login failed: " + request.error);
        }
    }

    public void OnRegisterButtonPressed()
    {
        StartCoroutine(SendRegisterRequest(usernameField.text, passwordField.text, confirmpasswordField.text, displaynameField.text));
    }

    IEnumerator SendRegisterRequest(string username, string password, string confirmPassword, string displayName)
    {
        RegisterData registerData = new RegisterData
        {
            Username = username,
            Password = password,
            ConfirmPassword = confirmPassword,
            DisplayName = displayName
        };

        string json = JsonConvert.SerializeObject(registerData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(_registerUrl.StaticURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            RegisterSuccess resp = JsonConvert.DeserializeObject<RegisterSuccess>(
                request.downloadHandler.text
            );
            Debug.Log("Register successful! Message: " + resp.Message);
        }
        else
        {
            Debug.LogError("Register failed: " + request.error);
        }
    }

    [Header("Login Test")]
    [SerializeField] private string _loginUsername;
    [SerializeField] private string _loginPassword;

    [Header("Register Test")]
    [SerializeField] private string _registerUsername;
    [SerializeField] private string _registerPassword;
    [SerializeField] private string _confirmPassword;
    [SerializeField] private string _displayName;

    [ContextMenu("Login")]
    public void Login()
    {   
        StartCoroutine(SendLoginRequest(_loginUsername, _loginPassword));
    }

    [ContextMenu("Register")]
    public void Register()
    {
        StartCoroutine(SendRegisterRequest(_registerUsername, _registerPassword, _confirmPassword, _displayName));
    }
}
