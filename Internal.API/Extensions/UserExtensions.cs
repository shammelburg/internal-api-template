using System.Security.Claims;

namespace Distrupol.Bank.API.Extensions;

public static class UserExtensions
{
    public static string GetDomainName(this ClaimsPrincipal User)
    {

        string domain = Environment.UserDomainName;
        string username = User.Identity.Name;

        if (string.IsNullOrEmpty(username))
        {
            username = User.FindFirstValue("preferred_username");
        }
        
        if(string.IsNullOrEmpty(domain)){
            domain = Environment.MachineName;
        }

        if (string.IsNullOrEmpty(username))
        {
            username = !string.IsNullOrEmpty(Environment.UserName)
                ? $"{domain}\\{Environment.UserName}"
                : "No user found";
        }

        return username;
    }
}