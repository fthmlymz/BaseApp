﻿using System.Text.Json.Serialization;

namespace InventoryManagement.Frontend.Services.Token
{
    public class TokenPermissions
    {
        [JsonPropertyName("upgraded")]
        public bool Upgraded { get; set; }

        [JsonPropertyName("access_token")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string AccessToken { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }


        [JsonPropertyName("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }



        [JsonPropertyName("refresh_token")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string RefreshToken { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [JsonPropertyName("token_type")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string TokenType { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.



    }
}
