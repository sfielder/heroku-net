using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkersInMotion.Model;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.Controllers
{
    [SessionExpireFilter]
    public class PeopleController : BaseController
    {
        #region Constructor
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IPeopleRepository _IPeopleRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        public PeopleController()
        {
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            this._IPeopleRepository = new PeopleRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
        }
        public PeopleController(WorkersInMotionDB context)
        {
            this._IPlaceRepository = new PlaceRepository(context);
            this._IPeopleRepository = new PeopleRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._IUserProfileRepository = new UserProfileRepository(context);
        }

        #endregion

        // GET: /People/
        public ActionResult Index(string id = "")
        {

            Logger.Debug("Inside People Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    var placeList = new PlaceViewModel();
                    placeList.PlaceList = new List<PlaceModel>();
                    var appPlace = new List<Place>();

                    appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    placeList.PlaceList.Add(new PlaceModel
                    {
                        PlaceGUID = "",
                        PlaceName = "All",
                        UserGUID = "",
                        OrganizationGUID = "",
                    });
                    foreach (var place in appPlace.ToList())
                    {
                        placeList.PlaceList.Add(new PlaceModel
                        {
                            PlaceGUID = place.PlaceGUID.ToString(),
                            PlaceID = place.PlaceID,
                            PlaceName = place.PlaceName,
                            UserGUID = place.UserGUID.ToString(),
                            OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                        });
                    }

                    var peopleList = new PeopleViewModel();
                    peopleList.PeopleList = new List<PeopleModel>();
                    var appPeople = new List<Person>();
                    if (string.IsNullOrEmpty(id) || id == Guid.Empty.ToString())
                    {
                        //if (Session["UserType"].ToString() == "ENT_A")
                        {
                            appPeople = _IPeopleRepository.GetPeopleByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                        }
                        //else
                        //{
                        //    appPeople = _IPeopleRepository.GetPeopleByUserGUID(new Guid(Session["UserGUID"].ToString())).ToList();
                        //}

                    }
                    else
                    {
                        appPeople = _IPeopleRepository.GetPeopleByPlaceGUID(new Guid(id)).ToList();
                        ViewBag.PlaceGUID = id;
                    }
                    foreach (var people in appPeople.ToList())
                    {
                        peopleList.PeopleList.Add(new PeopleModel
                        {
                            PeopleGUID = people.PeopleGUID.ToString(),
                            PlaceGUID = people.PlaceGUID != null ? people.PlaceGUID.ToString() : Guid.Empty.ToString(),
                            CompanyName = people.CompanyName,
                            BusinessPhone = people.BusinessPhone,
                            MarketGUID = people.MarketGUID != null ? people.MarketGUID.ToString() : Guid.Empty.ToString(),
                            FirstName = people.FirstName,
                            LastName = people.LastName,
                            UserGUID = people.UserGUID.ToString(),
                            OrganizationGUID = people.OrganizationGUID != null ? people.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                            MobilePhone = people.MobilePhone,
                            HomePhone = people.HomePhone,
                            Emails = people.Emails,
                            AddressLine1 = people.AddressLine1,
                            AddressLine2 = people.AddressLine2,
                            City = people.City,
                            State = people.State,
                            Country = people.Country,
                            ZipCode = people.ZipCode

                        });
                    }

                    var viewModel = new PeopleViewModel();
                    viewModel.Place = placeList.PlaceList.AsEnumerable();
                    viewModel.People = peopleList.PeopleList.AsEnumerable();

                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Login", "User");

            }
        }

        public ActionResult Create(string id = "", string customerid = "")
        {
            Logger.Debug("Inside People Controller- Create");
            try
            {

                if (Session["OrganizationGUID"] != null)
                {
                    TempData["TabName"] = "Contacts";
                    var userList = new AspNetUserViewModel();
                    userList.Users = new List<AspUser>();
                    var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).OrderBy(sort => sort.FirstName).ToList();
                    //if (string.IsNullOrEmpty(id))
                    {
                        userList.Users.Add(new AspUser { FirstName = "All", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                        foreach (var user in appUser.ToList())
                        {
                            userList.Users.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                        }
                    }

                    var placeList = new PlaceViewModel();
                    placeList.PlaceList = new List<PlaceModel>();
                    var appPlace = new List<Place>();
                    if (string.IsNullOrEmpty(id) || Guid.Empty == new Guid(id))
                    {
                        appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                        if (!string.IsNullOrEmpty(id) && Guid.Empty == new Guid(id))
                            ViewBag.UserID = Guid.Empty.ToString();
                    }
                    else
                    {
                        appPlace = _IPlaceRepository.GetPlaceByUserGUID(new Guid(id)).ToList();
                        ViewBag.UserID = id;
                    }
                    foreach (var place in appPlace.ToList())
                    {
                        placeList.PlaceList.Add(new PlaceModel
                        {
                            PlaceGUID = place.PlaceGUID.ToString(),
                            PlaceID = place.PlaceID,
                            PlaceName = place.PlaceName,
                            PlacePhone = place.PlacePhone,
                            FirstName = place.FirstName,
                            LastName = place.LastName,
                            UserGUID = place.UserGUID.ToString(),
                            OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                            MobilePhone = place.MobilePhone,
                            HomePhone = place.HomePhone,
                            Emails = place.Emails,
                            TimeZone = place.TimeZone,
                            AddressLine1 = place.AddressLine1,
                            AddressLine2 = place.AddressLine2,
                            City = place.City,
                            State = place.State,
                            Country = place.Country,
                            ZipCode = place.ZipCode

                        });
                    }
                    var placeviewModel = new PlaceViewModel();
                    placeviewModel.Place = placeList.PlaceList.AsEnumerable();
                    placeviewModel.User = userList.Users.AsEnumerable();

                    var PeopleViewForCreate = new PeopleViewForCreate();
                    PeopleViewForCreate.PlaceViewModel = placeviewModel;
                    if (TempData["PeopleModel"] != null)
                        PeopleViewForCreate.PeopleModel = (PeopleModel)TempData["PeopleModel"];
                    //if (string.IsNullOrEmpty(id))
                    return View(PeopleViewForCreate);
                    //else
                    //    return PartialView("../Place/_customer", PeopleViewForCreate);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PeopleViewForCreate peoplecreate)
        {
            Logger.Debug("Inside Place Controller- Create Http Post");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    TempData["TabName"] = "Contacts";
                    if (ModelState.IsValid)
                    {
                        PeopleModel people = new PeopleModel();
                        people = peoplecreate.PeopleModel;
                        Person People = new Person();
                        People.PeopleGUID = Guid.NewGuid();
                        People.UserGUID = new Guid(people.UserGUID.ToString());
                        if (!string.IsNullOrEmpty(people.OrganizationGUID) && people.OrganizationGUID != Guid.Empty.ToString())
                        {
                            People.OrganizationGUID = new Guid(people.OrganizationGUID.ToString());
                        }
                        else
                        {
                            People.OrganizationGUID = null;
                        }
                        People.IsPrimaryContact = true;
                        if (!string.IsNullOrEmpty(people.PlaceGUID) && people.PlaceGUID != Guid.Empty.ToString())
                        {
                            People.PlaceGUID = new Guid(people.PlaceGUID.ToString());
                        }
                        else
                        {
                            People.PlaceGUID = null;
                        }

                        People.FirstName = people.FirstName;
                        People.LastName = people.LastName;
                        People.MobilePhone = people.MobilePhone;
                        People.CompanyName = people.CompanyName;
                        People.BusinessPhone = people.BusinessPhone;
                        People.HomePhone = people.HomePhone;
                        People.Emails = people.Emails;
                        People.AddressLine1 = people.AddressLine1;
                        People.AddressLine2 = people.AddressLine2;
                        People.City = people.City;
                        People.State = people.State;
                        People.Country = people.Country;
                        People.ZipCode = people.ZipCode;
                        People.CategoryID = 0;
                        People.IsDeleted = false;
                        People.CreatedDate = DateTime.UtcNow;
                        People.UpdatedDate = DateTime.UtcNow;


                        int peopleInsertResult = _IPeopleRepository.InsertPeople(People);
                        //int peopleInsertResult = _IPeopleRepository.Save();
                        if (peopleInsertResult > 0)
                        {

                            return RedirectToAction("Index", "CustomerView", new { id = "Contacts", customerid = People.PlaceGUID.ToString() });
                        }
                        else
                        {
                            return View(peoplecreate);
                        }

                    }
                    else
                    {
                        var userList = new AspNetUserViewModel();
                        userList.Users = new List<AspUser>();
                        var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).OrderBy(sort => sort.FirstName).ToList();
                        //if (string.IsNullOrEmpty(id))
                        {
                            userList.Users.Add(new AspUser { FirstName = "All", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                            foreach (var user in appUser.ToList())
                            {
                                userList.Users.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                            }
                        }

                        var placeList = new PlaceViewModel();
                        placeList.PlaceList = new List<PlaceModel>();
                        var appPlace = new List<Place>();

                        appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                        foreach (var place in appPlace.ToList())
                        {
                            placeList.PlaceList.Add(new PlaceModel
                            {
                                PlaceGUID = place.PlaceGUID.ToString(),
                                PlaceID = place.PlaceID,
                                PlaceName = place.PlaceName,
                                PlacePhone = place.PlacePhone,
                                FirstName = place.FirstName,
                                LastName = place.LastName,
                                UserGUID = place.UserGUID.ToString(),
                                OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                                MobilePhone = place.MobilePhone,
                                HomePhone = place.HomePhone,
                                Emails = place.Emails,
                                TimeZone = place.TimeZone,
                                AddressLine1 = place.AddressLine1,
                                AddressLine2 = place.AddressLine2,
                                City = place.City,
                                State = place.State,
                                Country = place.Country,
                                ZipCode = place.ZipCode

                            });
                        }
                        var placeviewModel = new PlaceViewModel();
                        placeviewModel.Place = placeList.PlaceList.AsEnumerable();
                        placeviewModel.User = userList.Users.AsEnumerable();

                        var PeopleViewForCreate = new PeopleViewForCreate();
                        PeopleViewForCreate.PlaceViewModel = placeviewModel;
                        return View(PeopleViewForCreate);
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
                return View(peoplecreate);
            }
        }

        public ActionResult Edit(string id = "", string peopleguid = "")
        {
            Logger.Debug("Inside People Controller- Create");
            try
            {

                if (Session["OrganizationGUID"] != null)
                {
                    TempData["TabName"] = "Contacts";
                    Person EditPeople = _IPeopleRepository.GetPeopleByID(new Guid(peopleguid));
                    PeopleModel people = new PeopleModel();
                    if (EditPeople != null)
                    {
                        ViewBag.ContactName = EditPeople.FirstName;
                        people.PeopleGUID = EditPeople.PeopleGUID.ToString();
                        people.UserGUID = EditPeople.UserGUID.ToString();
                        people.OrganizationGUID = EditPeople.OrganizationGUID != null ? EditPeople.OrganizationGUID.ToString() : Guid.Empty.ToString();
                        people.IsPrimaryContact = Convert.ToBoolean(EditPeople.IsPrimaryContact);
                        people.PlaceGUID = EditPeople.PlaceGUID != null ? EditPeople.PlaceGUID.ToString() : Guid.Empty.ToString();
                        people.FirstName = EditPeople.FirstName;

                        people.LastName = EditPeople.LastName;
                        people.MobilePhone = EditPeople.MobilePhone;
                        people.CompanyName = EditPeople.CompanyName;
                        people.BusinessPhone = EditPeople.BusinessPhone;
                        people.HomePhone = EditPeople.HomePhone;
                        people.Emails = EditPeople.Emails;
                        people.AddressLine1 = EditPeople.AddressLine1;
                        people.AddressLine2 = EditPeople.AddressLine2;
                        people.City = EditPeople.City;
                        people.State = EditPeople.State;
                        people.Country = EditPeople.Country;
                        people.ZipCode = EditPeople.ZipCode;

                    }
                    var userList = new AspNetUserViewModel();
                    userList.Users = new List<AspUser>();
                    var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).OrderBy(sort => sort.FirstName).ToList();
                    //if (string.IsNullOrEmpty(id))
                    {
                        userList.Users.Add(new AspUser { FirstName = "All", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                        foreach (var user in appUser.ToList())
                        {
                            userList.Users.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                        }
                    }

                    var placeList = new PlaceViewModel();
                    placeList.PlaceList = new List<PlaceModel>();
                    var appPlace = new List<Place>();
                    if (string.IsNullOrEmpty(id) || Guid.Empty == new Guid(id))
                    {
                        appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                        if (!string.IsNullOrEmpty(id) && Guid.Empty == new Guid(id))
                            ViewBag.UserID = Guid.Empty.ToString();
                    }
                    else
                    {
                        appPlace = _IPlaceRepository.GetPlaceByUserGUID(new Guid(id)).ToList();
                        ViewBag.UserID = id;
                    }
                    foreach (var place in appPlace.ToList())
                    {
                        placeList.PlaceList.Add(new PlaceModel
                        {
                            PlaceGUID = place.PlaceGUID.ToString(),
                            PlaceID = place.PlaceID,
                            PlaceName = place.PlaceName,
                            PlacePhone = place.PlacePhone,
                            FirstName = place.FirstName,
                            LastName = place.LastName,
                            UserGUID = place.UserGUID.ToString(),
                            OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                            MobilePhone = place.MobilePhone,
                            HomePhone = place.HomePhone,
                            Emails = place.Emails,
                            TimeZone = place.TimeZone,
                            AddressLine1 = place.AddressLine1,
                            AddressLine2 = place.AddressLine2,
                            City = place.City,
                            State = place.State,
                            Country = place.Country,
                            ZipCode = place.ZipCode

                        });
                    }
                    var placeviewModel = new PlaceViewModel();
                    placeviewModel.Place = placeList.PlaceList.AsEnumerable();
                    placeviewModel.User = userList.Users.AsEnumerable();

                    var PeopleViewForCreate = new PeopleViewForCreate();
                    PeopleViewForCreate.PlaceViewModel = placeviewModel;
                    PeopleViewForCreate.PeopleModel = people;
                    return View(PeopleViewForCreate);

                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PeopleViewForCreate peoplecreate)
        {
            Logger.Debug("Inside Place Controller- Create Http Post");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    TempData["TabName"] = "Contacts";
                    ViewBag.ContactName = !string.IsNullOrEmpty(peoplecreate.PeopleModel.FirstName) ? peoplecreate.PeopleModel.FirstName.ToString() : _IPeopleRepository.GetPeopleByID(new Guid(peoplecreate.PeopleModel.PeopleGUID)).FirstName;
                    if (ModelState.IsValid)
                    {
                        PeopleModel people = new PeopleModel();
                        people = peoplecreate.PeopleModel;
                        Person People = new Person();
                        People.PeopleGUID = new Guid(people.PeopleGUID);
                        People.UserGUID = new Guid(people.UserGUID.ToString());

                        if (!string.IsNullOrEmpty(people.OrganizationGUID) && people.OrganizationGUID != Guid.Empty.ToString())
                        {
                            People.OrganizationGUID = new Guid(people.OrganizationGUID.ToString());
                        }
                        else
                        {
                            People.OrganizationGUID = null;
                        }
                        People.IsPrimaryContact = true;
                        if (!string.IsNullOrEmpty(people.PlaceGUID) && people.PlaceGUID != Guid.Empty.ToString())
                        {
                            People.PlaceGUID = new Guid(people.PlaceGUID.ToString());
                        }
                        else
                        {
                            People.PlaceGUID = null;
                        }
                        //People.OrganizationGUID = new Guid(people.OrganizationGUID.ToString());
                        //People.IsPrimaryContact = true;
                        //People.PlaceGUID = new Guid(people.PlaceGUID.ToString());

                        People.FirstName = people.FirstName;
                        People.LastName = people.LastName;
                        People.MobilePhone = people.MobilePhone;
                        People.CompanyName = people.CompanyName;
                        People.BusinessPhone = people.BusinessPhone;
                        People.HomePhone = people.HomePhone;
                        People.Emails = people.Emails;
                        People.AddressLine1 = people.AddressLine1;
                        People.AddressLine2 = people.AddressLine2;
                        People.City = people.City;
                        People.State = people.State;
                        People.Country = people.Country;
                        People.ZipCode = people.ZipCode;
                        People.CategoryID = 0;
                        People.IsDeleted = false;
                        People.UpdatedDate = DateTime.UtcNow;


                        int peopleInsertResult = _IPeopleRepository.UpdatePeople(People);
                        // int peopleInsertResult = _IPeopleRepository.Save();
                        if (peopleInsertResult > 0)
                        {
                            return RedirectToAction("Index", "CustomerView", new { id = "Contacts", customerid = People.PlaceGUID.ToString() });
                        }
                        else
                        {
                            return View(peoplecreate);
                        }

                    }
                    else
                    {
                        var userList = new AspNetUserViewModel();
                        userList.Users = new List<AspUser>();
                        var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).OrderBy(sort => sort.FirstName).ToList();
                        //if (string.IsNullOrEmpty(id))
                        {
                            userList.Users.Add(new AspUser { FirstName = "All", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                            foreach (var user in appUser.ToList())
                            {
                                userList.Users.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                            }
                        }

                        var placeList = new PlaceViewModel();
                        placeList.PlaceList = new List<PlaceModel>();
                        var appPlace = new List<Place>();

                        appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                        foreach (var place in appPlace.ToList())
                        {
                            placeList.PlaceList.Add(new PlaceModel
                            {
                                PlaceGUID = place.PlaceGUID.ToString(),
                                PlaceID = place.PlaceID,
                                PlaceName = place.PlaceName,
                                PlacePhone = place.PlacePhone,
                                FirstName = place.FirstName,
                                LastName = place.LastName,
                                UserGUID = place.UserGUID.ToString(),
                                OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                                MobilePhone = place.MobilePhone,
                                HomePhone = place.HomePhone,
                                Emails = place.Emails,
                                TimeZone = place.TimeZone,
                                AddressLine1 = place.AddressLine1,
                                AddressLine2 = place.AddressLine2,
                                City = place.City,
                                State = place.State,
                                Country = place.Country,
                                ZipCode = place.ZipCode

                            });
                        }
                        var placeviewModel = new PlaceViewModel();
                        placeviewModel.Place = placeList.PlaceList.AsEnumerable();
                        placeviewModel.User = userList.Users.AsEnumerable();

                        var PeopleViewForCreate = new PeopleViewForCreate();
                        PeopleViewForCreate.PlaceViewModel = placeviewModel;
                        PeopleViewForCreate.PeopleModel = peoplecreate.PeopleModel;
                        return View(PeopleViewForCreate);
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
                return RedirectToAction("Login", "User");
            }
        }
        public ActionResult Delete(string id = "", string customerguid = "")
        {
            Logger.Debug("Inside People Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    PeopleModel people = new PeopleModel();
                    people.PeopleGUID = id;
                    _IPeopleRepository.DeletePeople(new Guid(people.PeopleGUID));
                    // _IPeopleRepository.Save();
                    //string placeGUID = _IPeopleRepository.GetPeopleByID(new Guid(people.PeopleGUID)).PlaceGUID.ToString();
                    return RedirectToAction("Index", "CustomerView", new { id = "Contacts", customerid = customerguid });
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Index");
            }
        }

        public JsonResult SaveItem(string _peopleModel)
        {
            //ViewBag.PeopleModelItem = _peopleModel;
            Logger.Debug("Inside People Controller- SaveItem");
            JsonResult result = new JsonResult();
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var people = js.Deserialize<dynamic>(_peopleModel);
                PeopleModel People = new PeopleModel();
                People.FirstName = people["FirstName"];
                People.LastName = people["LastName"];
                People.MobilePhone = people["MobilePhone"];
                People.BusinessPhone = people["BusinessPhone"];
                People.HomePhone = people["HomePhone"];
                People.Emails = people["Email"];
                People.AddressLine1 = people["Address1"];
                People.AddressLine2 = people["Address2"];
                People.City = people["City"];
                People.State = people["State"];
                People.Country = people["Country"];
                People.ZipCode = people["ZipCode"];

                TempData["PeopleModel"] = People;
                result.Data = "success";
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                result.Data = "failure";
                return result;
            }
            //  return View("Create",Create(people["ID"], );
            //  people["ID"] 

        }

    }
}