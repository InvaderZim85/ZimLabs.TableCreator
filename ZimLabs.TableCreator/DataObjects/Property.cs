using System.Reflection;

namespace ZimLabs.TableCreator.DataObjects
{
    /// <summary>
    /// Represents the information about a property
    /// </summary>
    internal sealed class Property
    {
        /// <summary>
        /// Gets or sets the name of the property
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the appearance
        /// </summary>
        public AppearanceAttribute Appearance { get; set; }

        /// <summary>
        /// Gets the value which indicates if the property should be ignored
        /// </summary>
        public bool Ignore => Appearance?.Ignore ?? false;

        /// <summary>
        /// Converts a <see cref="PropertyInfo"/> into a <see cref="Property"/>
        /// </summary>
        /// <param name="prop">The original property</param>
        public static explicit operator Property(PropertyInfo prop)
        {
            var attribute = prop.GetCustomAttribute<AppearanceAttribute>();
            return new Property
            {
                Name = prop.Name,
                Appearance = attribute
            };
        }
    }
}
