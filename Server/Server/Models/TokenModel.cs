﻿namespace Server.Models
{
    public class TokenModel
    {
        public string? RefreshToken { get; set; }
        public string? AccessToken { get; set; }
        public string? ConfirmationToken { get; set; }
    }
}