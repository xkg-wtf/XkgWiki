using Microsoft.Extensions.Configuration;

namespace XkgWiki.Shared.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T Section<T>(this IConfiguration self)
        {
            return self.GetSection(typeof(T).Name).Get<T>();
        }
    }
}