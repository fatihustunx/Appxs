using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Linq;
using System.Text;
using System;

namespace App.Appxs.Exceptions
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message) : base(message)
        {
        }
    }

    public class AuthorizationProblemDetails : ProblemDetails
    {
        public override string ToString() => JsonSerializer.Serialize(this);
    }

    public class BusinessProblemDetails : ProblemDetails
    {
        public override string ToString() => JsonSerializer.Serialize(this);
    }

    public class ValidationProblemDetails : ProblemDetails
    {
        public object Errors { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }

    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}