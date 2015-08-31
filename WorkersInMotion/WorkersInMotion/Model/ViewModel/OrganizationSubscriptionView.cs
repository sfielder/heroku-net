using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class OrganizationSubscriptionView
    {
        [ScaffoldColumn(false)]
        [Key]
        public string OrganizationSubscriptionGUID { get; set; }
        [ScaffoldColumn(false)]
        public string OrganizationGUID { get; set; }
        [ScaffoldColumn(false)]
        public int Version { get; set; }
        [Display(Name = "Subscription enabled :")]
        public bool IsActive { get; set; }
        [Display(Name = "Paid subscriptions authorized :")]
        public int SubscriptionPurchased { get; set; }
        [Display(Name = "Paid subscriptions in use :")]
        public int SubscriptionConsumed { get; set; }
        [ScaffoldColumn(false)]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "Paid subscriptions expire on :")]
        public string ExpiryDate { get; set; }
        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; }

        public string OrganizationName { get; set; }
    }
    public class OrganizationSubscriptionViewModel
    {
        public IList<OrganizationSubscriptionView> OrganizationSubscriptionViewList { get; set; }
    }
}