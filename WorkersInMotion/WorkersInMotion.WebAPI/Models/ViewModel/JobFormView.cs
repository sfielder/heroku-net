using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class JobFormView
    {
        public string JobGUID { get; set; }
        public string StartTime { get; set; }
        public JobFormDetails Form { get; set; }
        public int Status { get; set; }
        public int ElapsedTime { get; set; }


    }
    public class JobFormNew
    {
        public string FormID { get; set; }
        public List<JobFormValues> Values { get; set; }
        public string JobGUID { get; set; }
        public List<JobFormValueDetails> FormValues { get; set; }
        public JobFormHeading JobFormHeading { get; set; }
        public List<CoOrdinates> CoordinateList { get; set; }
    }
    public class CoOrdinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string JobName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StoreName { get; set; }
        public int Count { get; set; }
    }

    public class JobFormHeading
    {
        public string JobName { get; set; }
        public string PlaceName { get; set; }
        public string MarketID { get; set; }
        public string MarketName { get; set; }
        public string MarketAddress { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string Status { get; set; }
        public string PoNumber { get; set; }
        public string PlaceID { get; set; }
        public string JobGUID { get; set; }
    }

    public class GeneratePDF
    {
        public List<JobFormNew> JobFormNewList { get; set; }

    }
    public class JobFormValues
    {
        public string Value { get; set; }
        public string ControlID { get; set; }
        public string ControlLabel { get; set; }
        public string parentID { get; set; }
        public string controlParentLabel { get; set; }
        public int ValueID { get; set; }
        public int currentValueID { get; set; }
    }

    public class JobFormValueDetails
    {
        public string Value { get; set; }
        public string FormID { get; set; }
        public int ControlID { get; set; }
        public ControlType ControlType { get; set; }
        public string ControlLabel { get; set; }
        public int parentID { get; set; }
        public string controlParentLabel { get; set; }
        public int ValueID { get; set; }
        public int currentValueID { get; set; }
        public string ImagePath { get; set; }
        public string OrganizationGUID { get; set; }
        public string JobGUID { get; set; }
    }
    public class JobFormDetails
    {
        public int FormID { get; set; }
        public List<FormValues> Values { get; set; }

        public List<FormDetails> FormDetailsList { get; set; }
    }

    public class FormValues
    {
        public string ControlID { get; set; }
        public int ValueID { get; set; }
        public int ParentID { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
    }

    public class FormDetails
    {
        public int ControlID { get; set; }
        public string FormID { get; set; }
        public ControlType ControlType { get; set; }
        public int ValueID { get; set; }
        public int ParentID { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
    }

    public enum ControlType
    {
        CONTROL_TYPE_EDIT = 1,
        CONTROL_TYPE_LABEL = 2,
        CONTROL_TYPE_DROP_DOWN = 3,
        CONTROL_TYPE_CHECKBOX_GROUP = 4,
        CONTROL_TYPE_CHECKBOX = 5,
        CONTROL_TYPE_IMAGE = 6,
        CONTROL_TYPE_DATE_TIME = 7,
        CONTROL_TYPE_CAMERA = 8,
        CONTROL_TYPE_LOCATION = 9,
        CONTROL_TYPE_LINE = 10,
        CONTROL_TYPE_PAGE = 11,
        CONTROL_TYPE_FORM = 12,
        CONTROL_TYPE_LIST = 13,
        CONTROL_TYPE_LISTS = 14,
        CONTROL_TYPE_SWITCH = 15,
        CONTROL_TYPE_RADIO_GROUP = 16,
        CONTROL_TYPE_RADIO_BUTTON = 17,
        CONTROL_TYPE_ISSUES_TO_REPORT = 18,
        CONTROL_TYPE_OPEN_CHARGEBACKS = 19,
        CONTROL_TYPE_UNSOLD_QUOTES = 20,
        CONTROL_TYPE_IMAGE_DESC = 21,
        CONTROL_TYPE_MULTITEXT = 22

    }
}