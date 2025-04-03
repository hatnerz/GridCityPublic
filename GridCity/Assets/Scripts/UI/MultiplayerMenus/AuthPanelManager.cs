using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    internal class AuthPanelManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _usernameField;
        [SerializeField] private TMP_InputField _passwordField;
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _registerButton;
        [SerializeField] private TMP_Text _errorText;

        private AuthClient authService;
        public event Action LoggedIn;

        private void Start()
        {
            authService = new AuthClient();
            _loginButton.onClick.AddListener(async() => await LoginAsync());
            _registerButton.onClick.AddListener(async() => await RegisterAsync());
        }

        private async Task LoginAsync()
        {
            _errorText.text = "";
            string username = _usernameField.text;
            string password = _passwordField.text;

            var loginResponse = await authService.LoginAsync(username, password);

            if (loginResponse.IsSuccess)
            {
                CredentialsManager.SetAuthToken(loginResponse.Token);
                LoggedIn?.Invoke();
            }
            else
            {
                _errorText.text = loginResponse.Error;
            }
        }

        private async Task RegisterAsync()
        {
            _errorText.text = "";
            string username = _usernameField.text;
            string password = _passwordField.text;

            var registerResponse = await authService.RegisterAsync(username, password);

            if (registerResponse.IsSuccess)
            {
                CredentialsManager.SetAuthToken(registerResponse.Token);
                LoggedIn?.Invoke();
            }
            else
            {
                _errorText.text = registerResponse.Error;
            }
        }
    }
}
