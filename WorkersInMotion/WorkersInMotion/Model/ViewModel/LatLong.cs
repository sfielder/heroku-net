using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class LatLong
    {
        private double m_cLatitude;

        public double Latitude
        {
            get { return m_cLatitude; }
            set { m_cLatitude = value; }
        }
        private double m_cLongitude;

        public double Longitude
        {
            get { return m_cLongitude; }
            set { m_cLongitude = value; }
        }
        public LatLong()
        {

        }
        public LatLong(double lat, double lng)
        {
            this.Latitude = lat;
            this.Longitude = lng;
        }
    }
}