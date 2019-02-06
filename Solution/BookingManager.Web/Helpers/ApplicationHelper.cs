using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingManager.Web.Helpers
{
    public class ApplicationHelper
    {
        public const string UsernameTagName = "username";
        public const string UserFullnameTagNam = "userfullname";
        public const string AgencyNumberTagName = "agencynumber";
        public const string AgencyNameTagName = "agencyname";
        public const string UserPrivileges = "userprivileges";

        private static ApplicationHelper singleton;
        public static ApplicationHelper Instance {
            get {
                if (singleton == null) singleton = new ApplicationHelper();
                return singleton;
            }
        }

        public string GetTagValueFromIdentity(System.Security.Principal.IIdentity Identity, string TagName) {
            string[] tagdict = Identity.Name.Split('|');
            foreach (string tagkvp in tagdict) {
                string[] kvp = tagkvp.Split(':');
                if (kvp[0].ToUpper() == TagName.ToUpper()) return kvp[1];
            }
            return string.Empty;
        }

        public string CreateTagDictionaryForIdentity(Dictionary<string, string> TagDictionary) {
            string result = string.Empty;
            foreach (KeyValuePair<string, string> kvp in TagDictionary) {
                result += kvp.Key + ":" + kvp.Value + "|";
            }
            result = result.Remove(result.Length - 1, 1);
            return result;
        }

        public KeyValuePair<bool, string> FormatPhoneNumber(string PhoneNumber, bool GetOnlyNumbers)
        {
            if (PhoneNumber == null || PhoneNumber == string.Empty)
                return new KeyValuePair<bool, string>(false, string.Empty);

            string original = PhoneNumber;

            int pointer = 0;
            while (PhoneNumber.Length != 0 & pointer <= PhoneNumber.Length - 1)
                if (char.IsNumber(PhoneNumber[pointer]))
                    pointer += 1;
                else
                    PhoneNumber = PhoneNumber.Remove(pointer, 1);

            long number;
            if (!long.TryParse(PhoneNumber, out number))
                return new KeyValuePair<bool, string>(false, original);
            else if (PhoneNumber.Length != 10)
                return new KeyValuePair<bool, string>(false, original);

            if (!GetOnlyNumbers)
            {
                PhoneNumber = PhoneNumber.Insert(0, "(");
                PhoneNumber = PhoneNumber.Insert(4, ")");
                PhoneNumber = PhoneNumber.Insert(5, " ");
                PhoneNumber = PhoneNumber.Insert(9, "-");
            }

            return new KeyValuePair<bool, string>(true, PhoneNumber);
        }
    }
}