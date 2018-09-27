using System.Collections.Generic;

namespace Nop.Core.Domain.Customers
{
    public static partial class SystemCustomerRoleNames
    {
        public static string Administrators => "Administrators";
        
        public static string ForumModerators => "ForumModerators";

        public static string Registered => "Registered";

        public static string Designer => "Designer";

        public static string Guests => "Guests";

        public static string Vendors => "Vendors";

        public static string Stores => "Stores";

        public static string LivechatAgent => "LivechatAgent";

        public static string LivechatCustomer => "LivechatCustomer";

        public static string SpecialDayCustomer => "ConsumidorSpecialDayPlanejados";

        public static string LivechatAdmin => "ManageLivechat";

        public static List<string> GetStoreOnlyPermission()
        {
            return new List<string>
            {
                Stores,
                Vendors,
                LivechatAgent,
                LivechatCustomer,
                SpecialDayCustomer
            };
        }

        public static List<string> GetVendorRolePermission()
        {
            return new List<string>
            {
                SpecialDayCustomer
            };
        }
    }
}