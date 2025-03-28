﻿namespace PhloSystemsApi.Models
{
    /// <summary>
    /// Represents the login model containing the username and password.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }
    }
}