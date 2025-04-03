using System;
using UnityEngine;
using JWT.Serializers;
using JWT;
using JWT.Builder;
using JWT.Algorithms;
using System.Collections.Generic;
using System.Security.Claims;

namespace Assets.Scripts.Networking
{
    public static class CredentialsManager
    {
        private static string authTokenPreference = "jwt_token";
        private static string token = null;

        public static string GetAuthToken()
        {
            //var token = PlayerPrefs.GetString(authTokenPreference);
            if (IsTokenExpired(token))
            {
                Debug.Log("expired");
                //PlayerPrefs.DeleteKey(authTokenPreference);
                //PlayerPrefs.Save();
                return null;
            }

            return token;
        }

        public static bool SetAuthToken(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
                return false;

            Debug.Log(authToken);
            //PlayerPrefs.SetString(authTokenPreference, authToken);
            //PlayerPrefs.Save();
            token = authToken;
            return true;
        }

        public static void RemoveAuthToken()
        {
            token = null;
            //PlayerPrefs.DeleteKey(authTokenPreference);
            //PlayerPrefs.Save();
        }

        public static (Guid Id, string Username)? GetCurrentUser()
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var payload = JwtBuilder.Create()
                .WithAlgorithm(new NoneAlgorithm())
                .WithJsonSerializer(new JsonNetSerializer())
                .WithUrlEncoder(new JwtBase64UrlEncoder())
                .WithValidator(new JwtValidator(new JsonNetSerializer(), new UtcDateTimeProvider()))
                .Decode<Dictionary<string, string>>(token);

            return (payload.ContainsKey(ClaimTypes.NameIdentifier) ? Guid.Parse(payload[ClaimTypes.NameIdentifier]) : Guid.Empty,
                payload.ContainsKey(ClaimTypes.Name) ? payload[ClaimTypes.Name] : "");
        }

        private static bool IsTokenExpired(string token)
        {
            if (string.IsNullOrEmpty(token))
                return true;

            Debug.Log(token);
            var payload = JwtBuilder.Create()
                .WithAlgorithm(new NoneAlgorithm())
                .WithJsonSerializer(new JsonNetSerializer())
                .WithUrlEncoder(new JwtBase64UrlEncoder())
                .WithValidator(new JwtValidator(new JsonNetSerializer(), new UtcDateTimeProvider()))
                .Decode<JwtPayload>(token);

            var exp = payload.Exp;
            if (!exp.HasValue)
                return true;

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp.Value);
            return expirationTime < DateTimeOffset.UtcNow;
        }

        private class JwtPayload
        {
            public long? Exp { get; set; }
        }

    }
}
