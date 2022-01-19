using System.Diagnostics;
using System.Reflection;

namespace ZimLabs.TableCreator.DataObjects
{
    /// <summary>
    /// Represents the information about a property
    /// </summary>
    [DebuggerDisplay("{Name}")]
    internal sealed class Property
    {
        /// <summary>
        /// Gets the name of the property
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the custom name if available, otherwise the original name will be returned
        /// </summary>
        public string CustomName
        {
            get
            {
                if (Appearance == null)
                    return Name;

                return string.IsNullOrEmpty(Appearance.Name) ? Name : Appearance.Name;
            }
        }
        
        /// <summary>
        /// Gets the appearance
        /// </summary>
        public AppearanceAttribute Appearance { get; }

        /// <summary>
        /// Gets the value which indicates if the property should be ignored
        /// </summary>
        public bool Ignore => Appearance?.Ignore ?? false;

        /// <summary>
        /// Gets the order value of the property
        /// </summary>
        public int Order => Appearance?.Order ?? -1;

        /// <summary>
        /// Creates a new instance of the <see cref="Property"/>
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="appearance">The appearance value</param>
        private Property(string name, AppearanceAttribute appearance)
        {
            Name = name;
            Appearance = appearance;
        }

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a <see cref="Property"/>
        /// </summary>
        /// <param name="prop">The original property</param>
        public static explicit operator Property(PropertyInfo prop)
        {
            var attribute = prop.GetCustomAttribute<AppearanceAttribute>();
            return new Property(prop.Name, attribute);
        }
    }
}
