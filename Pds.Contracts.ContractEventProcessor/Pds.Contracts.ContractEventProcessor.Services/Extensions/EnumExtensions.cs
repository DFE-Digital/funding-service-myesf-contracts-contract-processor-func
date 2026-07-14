using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Pds.Contracts.ContractEventProcessor.Services.Extensions
{
    /// <summary>
    /// This is a extension class of enum.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get Enum Display Name.
        /// </summary>
        /// <param name="enumType">Enum.</param>
        /// <returns>Returns the display name of the Enum.</returns>
        public static string GetEnumDisplayName(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .Name;
        }

        /// <summary>
        /// Get Enum Short Name.
        /// </summary>
        /// <param name="enumType">Enum.</param>
        /// <returns>Returns the display name of the Enum.</returns>
        public static string GetEnumShortName(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .ShortName;
        }
    }
}