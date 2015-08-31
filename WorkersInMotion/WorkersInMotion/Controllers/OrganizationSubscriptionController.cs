using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkersInMotion.DataAccess.Repository;
using System.Globalization;
using WorkersInMotion.Model;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.Controllers
{
    [SessionExpireFilter]
    public class OrganizationSubscriptionController : BaseController
    {
        //
        // GET: /OrganizationSubscription/
        private readonly IOrganizationSubscriptionRepository _IOrganizationSubscriptionRepository;
        private readonly IOrganizationRepository _IOrganizationRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        public OrganizationSubscriptionController()
        {
            this._IOrganizationSubscriptionRepository = new OrganizationSubscriptionRepository(new WorkersInMotionDB());
            this._IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
            this._IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
        }
        public OrganizationSubscriptionController(WorkersInMotionDB context)
        {
            this._IOrganizationSubscriptionRepository = new OrganizationSubscriptionRepository(context);
            this._IOrganizationRepository = new OrganizationRepository(context);
            this._IGlobalUserRepository = new GlobalUserRepository(context);
        }

        public ActionResult Index(string id = "")
        {
            Logger.Debug("Inside Organization Subscription Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    OrganizationSubscriptionViewModel pOrganizationSubscriptionViewModel = new OrganizationSubscriptionViewModel();
                    pOrganizationSubscriptionViewModel.OrganizationSubscriptionViewList = new List<OrganizationSubscriptionView>();
                    if (!string.IsNullOrEmpty(id))
                    {
                        OrganizationSubscription organizationSubscription = _IOrganizationSubscriptionRepository.GetOrganizationSubscriptionByOrgID(new Guid(id));
                        OrganizationSubscriptionView organizationSubscriptionView = new OrganizationSubscriptionView
                        {
                            OrganizationSubscriptionGUID = organizationSubscription.OrganizationSubscriptionGUID.ToString(),
                            OrganizationGUID = organizationSubscription.OrganizationGUID.ToString(),
                            Version = organizationSubscription.Version,
                            IsActive = organizationSubscription.IsActive,
                            SubscriptionPurchased = organizationSubscription.SubscriptionPurchased,
                            SubscriptionConsumed = organizationSubscription.SubscriptionConsumed,
                            StartDate = Convert.ToDateTime(organizationSubscription.StartDate),
                            CreatedDate = Convert.ToDateTime(organizationSubscription.CreatedDate),
                            ExpiryDate = Convert.ToDateTime(organizationSubscription.ExpiryDate).ToString("dd-MMM-yy"),
                            OrganizationName = _IOrganizationRepository.GetOrganizationByID(organizationSubscription.OrganizationGUID).OrganizationFullName

                        };
                        ViewBag.OrganizationName = organizationSubscriptionView.OrganizationName;
                        pOrganizationSubscriptionViewModel.OrganizationSubscriptionViewList.Add(organizationSubscriptionView);
                    }
                    else
                    {
                        List<OrganizationSubscription> organizationSubscriptionList = _IOrganizationSubscriptionRepository.GetOrganizationSubscription().ToList();
                        foreach (OrganizationSubscription organizationSubscription in organizationSubscriptionList)
                        {
                            OrganizationSubscriptionView organizationSubscriptionView = new OrganizationSubscriptionView
                            {
                                OrganizationSubscriptionGUID = organizationSubscription.OrganizationSubscriptionGUID.ToString(),
                                OrganizationGUID = organizationSubscription.OrganizationGUID.ToString(),
                                Version = organizationSubscription.Version,
                                IsActive = organizationSubscription.IsActive,
                                SubscriptionPurchased = organizationSubscription.SubscriptionPurchased,
                                SubscriptionConsumed = organizationSubscription.SubscriptionConsumed,
                                StartDate = Convert.ToDateTime(organizationSubscription.StartDate),
                                CreatedDate = Convert.ToDateTime(organizationSubscription.CreatedDate),
                                ExpiryDate = Convert.ToDateTime(organizationSubscription.ExpiryDate).ToString("dd-MMM-yy"),
                                OrganizationName = _IOrganizationRepository.GetOrganizationByID(organizationSubscription.OrganizationGUID).OrganizationFullName
                            };
                            pOrganizationSubscriptionViewModel.OrganizationSubscriptionViewList.Add(organizationSubscriptionView);
                        }
                        ViewBag.OrganizationName = "admin";
                    }
                    ViewBag.UserType = _IGlobalUserRepository.GetUserType(new Guid(Session["UserGUID"].ToString()));

                    return View(pOrganizationSubscriptionViewModel);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("../User/Login");
            }
        }
        public ActionResult Edit(string id = "")
        {
            Logger.Debug("Inside Organization Subscription Controller- Edit");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        OrganizationSubscription organizationSubscription = _IOrganizationSubscriptionRepository.GetOrganizationSubscriptionByOrgID(new Guid(id));
                        if (organizationSubscription != null)
                        {
                            System.Globalization.DateTimeFormatInfo dateInfo = new System.Globalization.DateTimeFormatInfo();
                            dateInfo.ShortDatePattern = "dd-MMM-yy";
                            OrganizationSubscriptionView organizationSubscriptionView = new OrganizationSubscriptionView
                            {
                                OrganizationSubscriptionGUID = organizationSubscription.OrganizationSubscriptionGUID.ToString(),
                                OrganizationGUID = organizationSubscription.OrganizationGUID.ToString(),
                                Version = organizationSubscription.Version,
                                IsActive = organizationSubscription.IsActive,
                                SubscriptionPurchased = organizationSubscription.SubscriptionPurchased,
                                SubscriptionConsumed = organizationSubscription.SubscriptionConsumed,
                                StartDate = Convert.ToDateTime(organizationSubscription.StartDate),
                                CreatedDate = Convert.ToDateTime(organizationSubscription.CreatedDate),
                                ExpiryDate = Convert.ToDateTime(organizationSubscription.ExpiryDate).ToString("dd-MMM-yy")
                                //DateTime.ParseExact(organizationSubscription.ExpiryDate.ToString(), "dd-MMM-yy hh:mm:ss tt", CultureInfo.InvariantCulture)
                                //Convert.ToDateTime(organizationSubscription.ExpiryDate)
                            };

                            ViewBag.UserType = _IGlobalUserRepository.GetUserType(new Guid(Session["UserGUID"].ToString()));
                            ViewBag.OrganizationName = _IOrganizationRepository.GetOrganizationByID(organizationSubscription.OrganizationGUID).OrganizationFullName;

                            return View(organizationSubscriptionView);
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else
                    {
                        return RedirectToAction("../User/Login");
                    }
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("../User/Login");
            }
        }
        [HttpPost]
        public ActionResult Edit(OrganizationSubscriptionView organizationSubscriptionView)
        {

            Logger.Debug("Inside Organization Subscription Controller- Edit HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    OrganizationSubscription organizationSubscription = new OrganizationSubscription();
                    organizationSubscription.OrganizationSubscriptionGUID = new Guid(organizationSubscriptionView.OrganizationSubscriptionGUID);
                    organizationSubscription.OrganizationGUID = new Guid(organizationSubscriptionView.OrganizationGUID);
                    organizationSubscription.IsActive = organizationSubscriptionView.IsActive;
                    organizationSubscription.Version = organizationSubscriptionView.Version;
                    organizationSubscription.SubscriptionPurchased = organizationSubscriptionView.SubscriptionPurchased;
                    organizationSubscription.SubscriptionConsumed = organizationSubscriptionView.SubscriptionConsumed;
                    // organizationSubscription.StartDate = organizationSubscriptionView.StartDate;
                    organizationSubscription.ExpiryDate = Convert.ToDateTime(organizationSubscriptionView.ExpiryDate);
                    //organizationSubscription.CreatedDate = organizationSubscriptionView.CreatedDate;

                    int result = _IOrganizationSubscriptionRepository.UpdateOrganizationSubscription(organizationSubscription);
                    //int result = _IOrganizationSubscriptionRepository.Save();
                    if (result > 0)
                        return RedirectToAction("../Organization/Index");
                    else
                        return View();
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("../User/Login");
            }
        }
    }
}