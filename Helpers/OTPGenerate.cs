using System;
using System.Security.Cryptography;
using System.Text;

namespace pro.backend.Helpers
{
    public  static class OTPGenerate
    {

    public static string OTPCharacters()
    {
        string OTPLength = "6";
        
        string NewCharacters = "";
 
        //This one tells you which characters are allowed in this new password
        string allowedChars = "";
        //Here Specify your OTP Characters
        allowedChars = "1,2,3,4,5,6,7,8,9,0";
        //If you Need more secure OTP then uncomment Below Lines 
        //  allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";        
        // allowedChars += "~,!,@,#,$,%,^,&,*,+,?";
 
        
 
        char[] sep = { ',' };
        string[] arr = allowedChars.Split(sep);
 
        string IDString = "";
        string temp = "";
 
        //utilize the "random" class
        Random rand = new Random();
 
        
        for (int i = 0; i < Convert.ToInt32(OTPLength); i++)
        {
            temp = arr[rand.Next(0, arr.Length)];
            IDString += temp;
            NewCharacters = IDString;
        }
 
        return NewCharacters;
    }
    public static string OTPGenerator(string uniqueIdentity, string uniqueCustomerIdentity)
    {
        int length = 6;
        string oneTimePassword = "";
        DateTime dateTime = DateTime.Now;
        string _strParsedReqNo = dateTime.Day.ToString();
        _strParsedReqNo = _strParsedReqNo + dateTime.Month.ToString();
        _strParsedReqNo = _strParsedReqNo + dateTime.Year.ToString();
        _strParsedReqNo = _strParsedReqNo + dateTime.Hour.ToString();
        _strParsedReqNo = _strParsedReqNo + dateTime.Minute.ToString();
        _strParsedReqNo = _strParsedReqNo + dateTime.Second.ToString();
        _strParsedReqNo = _strParsedReqNo + dateTime.Millisecond.ToString();
        _strParsedReqNo = _strParsedReqNo + uniqueCustomerIdentity;
 
 
        _strParsedReqNo = uniqueIdentity + uniqueCustomerIdentity;
        using (MD5 md5 = MD5.Create())
        {
            //Get hash code of entered request id in byte format.
            byte[] _reqByte = md5.ComputeHash(Encoding.UTF8.GetBytes(_strParsedReqNo));
            //convert byte array to integer.
            int _parsedReqNo = BitConverter.ToInt32(_reqByte, 0);
            string _strParsedReqId = Math.Abs(_parsedReqNo).ToString();
            //Check if length of hash code is less than 9.
            //If so, then prepend multiple zeros upto the length becomes atleast 9 characters.
            if (_strParsedReqId.Length < 9)
            {
                StringBuilder sb = new StringBuilder(_strParsedReqId);
                for (int k = 0; k < (9 - _strParsedReqId.Length); k++)
                {
                    sb.Insert(0, '0');
                }
                _strParsedReqId = sb.ToString();
            }
            oneTimePassword = _strParsedReqId;
        }
        
        if (oneTimePassword.Length >= length)
        {
            oneTimePassword = oneTimePassword.Substring(0, length);
        }
        return oneTimePassword;
    }
    }
}