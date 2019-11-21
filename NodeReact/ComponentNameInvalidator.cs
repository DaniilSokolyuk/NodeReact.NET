using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;

namespace NodeReact
{
    public interface IComponentNameInvalidator
    {
        bool IsValid(string componentName);
    }

    internal class ComponentNameInvalidator : IComponentNameInvalidator
    {
        /// <summary>
        /// Regular expression used to validate JavaScript identifiers. Used to ensure component
        /// names are valid.
        /// Based off https://gist.github.com/Daniel15/3074365
        /// </summary>
        private static readonly Regex _identifierRegex =
            new Regex(@"^[a-zA-Z_$][0-9a-zA-Z_$]*(?:\[(?:"".+""|\'.+\'|\d+)\])*?$", RegexOptions.Compiled);


        private static readonly ConcurrentDictionary<string, bool> _componentNameValidCache =
            new ConcurrentDictionary<string, bool>(StringComparer.Ordinal);

        public bool IsValid(string componentName)
        {
            return _componentNameValidCache.GetOrAdd(
                componentName,
                compName => compName.Split('.').All(segment => _identifierRegex.IsMatch(segment)));
        }
    }
}
