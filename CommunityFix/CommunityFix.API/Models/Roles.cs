namespace CommunityFix.API.Models
{
    public class Roles
    {
        public const string Resident = "Resident";
        public const string Representative = "Representative";
        public const string Administrator = "Administrator";
        public const string SystemAdmin = "SystemAdmin";

       public static readonly string[] All = { Resident, Representative, Administrator, SystemAdmin };


    }
}
