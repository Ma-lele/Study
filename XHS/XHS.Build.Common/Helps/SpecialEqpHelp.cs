using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Helps
{
    public class SpecialEqpHelp
    {
        private const string TIME_FORMAT = "yyyy-MM-dd HH:mm:ss ";
        public static List<int> getSpecErrList(int value)
        {
            var errcodelist = new List<int>();
            int tempvalue = 0;

            if (value == 0)
            {
                return new List<int>() { 0 };
            }

            if (value % 2 != 0)
            {
                errcodelist.Add(1);
                value = value - 1;
            }

            while (value > 0)
            {
                var pow = Math.Floor(Math.Log(value) / Math.Log(2));
                tempvalue = (int)Math.Pow(2, pow);
                errcodelist.Add(tempvalue);
                value = value - tempvalue;
            }

            return errcodelist;
        }

        public static string GetPass(string SECRTE= "cag4adg412fa2dc2", string AES = "da2gaf4afdasfea1")
        {
            string result = string.Empty;
            result = string.Format("{0}:{1}", SECRTE, TIME_FORMAT);
            result = UEncrypter.EncryptAES128ECBPKCS5Padding(result, AES);
            return result;
        }

        public static string GetTowerPass()
        {
            string SECRTE = "CCnArZl4E7CBnnOJ";
            string AES = "qiNzOg3Z9teDEMvF";
            string result = string.Empty;
            result = string.Format("{0}:{1}", SECRTE, TIME_FORMAT);
            result = UEncrypter.EncryptAES128ECBPKCS5Padding(result, AES);
            return result;
        }

        public static string GetElevatorPass()
        {
            string SECRTE = "9tOsmw9A2vC6vZvT";
            string AES = "YwR427z3vzEuGNsL";
            string result = string.Empty;
            result = string.Format("{0}:{1}", SECRTE, TIME_FORMAT);
            result = UEncrypter.EncryptAES128ECBPKCS5Padding(result, AES);
            return result;
        }
    }
}
