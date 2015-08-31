using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public  enum eLoginType
    {
        WebLogin=0,
        DeviceLogin=1,
        APITest=2
    }
    public class LoginRequest
    {
        public string Cred { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public eLoginType LoginType { get; set; } //0: Web Login, 1: Devie, 2: ApiTest
        public string PushID { get; set; }
        public DeviceInfo DeviceInfo { get; set; }
    }
    //public class Crediential
    //{
    //    
    //    
    //}
    public class DeviceInfo
    {

        public string deviceid { get; set; }
        public string deviceipaddress { get; set; }
        public short devicetype { get; set; }
        public float TimeZone { get; set; }
        public string deviceindus { get; set; }
        public string cellid { get; set; }
        public string model { get; set; }
        public string countrycode { get; set; }
        public string buildtype { get; set; }
        public string networktypename { get; set; }
        public string osVer { get; set; }
        public string batterystatus { get; set; }
        public string networksubtypename { get; set; }
        public string macaddress { get; set; }
        public string camera { get; set; }
        public string imei { get; set; }
        public string buildnum { get; set; }
        public string networkoprname { get; set; }
        public string hasgps { get; set; }
        public string changelist { get; set; }
        public string issimpresent { get; set; }
        public string buildNo { get; set; }
        public string product { get; set; }
        public string phonetype { get; set; }
        public string userID { get; set; }
        public string manufacture { get; set; }
        public string brand { get; set; }
        public string imsi { get; set; }
        public string devicebuild { get; set; }
        public string devicezone { get; set; }
        public string Devicezoneoffset { get; set; }
        public string deviceLocale { get; set; }
        public string deviceDateFormat { get; set; }
        public string currentTime { get; set; }
    }
}