using System.Linq;
using System.Reflection;

namespace RavenDB.AspNet.Identity
{
    public static class ChangeUpdater
    {
        /// <summary>
        /// Updates properties of the calling object if public properties differ.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The calling object.</param>
        /// <param name="to">The object to compare to the calling object./param>
        /// <param name="ignoredPropertyNames">Public properties that should be excluded from the comparisons.</param>
        /// <returns>True if any public properties have been updated on the calling object. False otherwise.</returns>
        public static bool SetUpdatedProperties<T>(this T self, T to, params string[] ignoredPropertyNames) where T : class
        {
            var propertiesWereChanged = false;

            if (self != null && to != null)
            {
                var type = typeof(T);
                var publicProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var propertyInfo in publicProperties.Where(propertyInfo => !ignoredPropertyNames.Contains(propertyInfo.Name)))
                {
                    var selfValue = propertyInfo.GetValue(self, null);
                    var toValue = propertyInfo.GetValue(to, null);

                    if (selfValue != toValue 
                        && propertyInfo.CanWrite)
                    {
                        propertyInfo.SetValue(self, toValue);
                        propertiesWereChanged = true;
                    }
                }
            }

            return propertiesWereChanged;
        }
    }
}
