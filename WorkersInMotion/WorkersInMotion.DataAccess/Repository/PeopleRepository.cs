using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;



namespace WorkersInMotion.DataAccess.Repository
{
    public class PeopleRepository : IPeopleRepository, IDisposable
    {
        private WorkersInMotionDB context;

        public PeopleRepository(WorkersInMotionDB context)
        {
            this.context = context;
        }

        public IEnumerable<Person> GetPeopleByUserGUID(Guid UserGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.People
            //            where p.UserGUID == UserGUID
            //            select p).ToList().OrderBy(x => x.FirstName);

            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            return context.Database.SqlQuery<Person>("select * from  People where UserGUID=@pUserGUID order by FirstName", Param);
        }
        public string GetPeopleNameByPeopleGUID(Guid PeopleGUID)
        {
            //return context.People.Find(PeopleGUID).FirstName;

            Person person = GetPeopleByID(PeopleGUID);
            if (person != null)
            {
                return person.FirstName;
            }
            else
            {
                return string.Empty;
            }
        }
        public Person GetPeopleByID(Guid PeopleGUID)
        {
            // return context.People.Find(PeopleGUID);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pPeopleGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = PeopleGUID;
            return context.Database.SqlQuery<Person>("select * from People where PeopleGUID=@pPeopleGUID", Param).FirstOrDefault();
        }


        public IEnumerable<Person> GetPeopleByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.People
            //            where p.OrganizationGUID == OrganizationGUID
            //            select p).ToList().OrderBy(x => x.FirstName);

            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.SqlQuery<Person>("select * from People where OrganizationGUID=@pOrganizationGUID order by FirstName", Param);
        }


        public IEnumerable<Person> GetPeopleByPlaceGUID(Guid PlaceGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.People
            //            where p.PlaceGUID == PlaceGUID
            //            select p).ToList().OrderBy(x => x.FirstName);

            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pPlaceGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = PlaceGUID;
            return context.Database.SqlQuery<Person>("select * from People where PlaceGUID=@pPlaceGUID order by FirstName", Param);

        }

        public int InsertPeople(Person Person)
        {
            // context.People.Add(Person);
            SqlParameter[] Param = new SqlParameter[25];
            Param[0] = new SqlParameter("@pPeopleGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = Person.PeopleGUID;
            Param[1] = new SqlParameter("@pRecordStatus", SqlDbType.Int);
            Param[1].Value = (object)Person.RecordStatus ?? DBNull.Value;
            Param[2] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = Person.UserGUID;
            Param[3] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[3].Value = (object)Person.OrganizationGUID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pIsPrimaryContact", SqlDbType.Bit);
            Param[4].Value = (object)Person.IsPrimaryContact ?? DBNull.Value;
            Param[5] = new SqlParameter("@pPlaceGUID", SqlDbType.UniqueIdentifier);
            Param[5].Value = (object)Person.PlaceGUID ?? DBNull.Value;
            Param[6] = new SqlParameter("@pMarketGUID", SqlDbType.UniqueIdentifier);
            Param[6].Value = (object)Person.MarketGUID ?? DBNull.Value;
            Param[7] = new SqlParameter("@pFirstName", SqlDbType.NVarChar, 50);
            Param[7].Value = (object)Person.FirstName ?? DBNull.Value;
            Param[8] = new SqlParameter("@pLastName", SqlDbType.NVarChar, 50);
            Param[8].Value = (object)Person.LastName ?? DBNull.Value;
            Param[9] = new SqlParameter("@pCompanyName", SqlDbType.NVarChar, 50);
            Param[9].Value = (object)Person.CompanyName ?? DBNull.Value;
            Param[10] = new SqlParameter("@pMobilePhone", SqlDbType.NVarChar, 20);
            Param[10].Value = (object)Person.MobilePhone ?? DBNull.Value;
            Param[11] = new SqlParameter("@pBusinessPhone", SqlDbType.NVarChar, 20);
            Param[11].Value = (object)Person.BusinessPhone ?? DBNull.Value;
            Param[12] = new SqlParameter("@pHomePhone", SqlDbType.NVarChar, 20);
            Param[12].Value = (object)Person.HomePhone ?? DBNull.Value;
            Param[13] = new SqlParameter("@pEmails", SqlDbType.NVarChar, -1);
            Param[13].Value = (object)Person.Emails ?? DBNull.Value;
            Param[14] = new SqlParameter("@pAddressLine1", SqlDbType.NVarChar, 256);
            Param[14].Value = (object)Person.AddressLine1 ?? DBNull.Value;
            Param[15] = new SqlParameter("@pAddressLine2", SqlDbType.NVarChar, 256);
            Param[15].Value = (object)Person.AddressLine2 ?? DBNull.Value;
            Param[16] = new SqlParameter("@pCity", SqlDbType.NVarChar, 128);
            Param[16].Value = (object)Person.City ?? DBNull.Value;
            Param[17] = new SqlParameter("@pState", SqlDbType.NVarChar, 128);
            Param[17].Value = (object)Person.State ?? DBNull.Value;
            Param[18] = new SqlParameter("@pCountry", SqlDbType.NVarChar, 128);
            Param[18].Value = (object)Person.Country ?? DBNull.Value;
            Param[19] = new SqlParameter("@pZipCode", SqlDbType.NVarChar, 20);
            Param[19].Value = (object)Person.ZipCode ?? DBNull.Value;
            Param[20] = new SqlParameter("@pCategoryID", SqlDbType.Int);
            Param[20].Value = (object)Person.CategoryID ?? DBNull.Value;
            Param[21] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[21].Value = (object)Person.IsDeleted ?? DBNull.Value;
            Param[22] = new SqlParameter("@pImageURL", SqlDbType.NVarChar, -1);
            Param[22].Value = (object)Person.ImageURL ?? DBNull.Value;
            Param[23] = new SqlParameter("@pCreatedDate", SqlDbType.DateTime);
            Param[23].Value = (object)Person.CreatedDate ?? DBNull.Value;
            Param[24] = new SqlParameter("@pUpdatedDate", SqlDbType.DateTime);
            Param[24].Value = (object)Person.UpdatedDate ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into People(PeopleGUID,RecordStatus,UserGUID,OrganizationGUID,IsPrimaryContact,PlaceGUID,MarketGUID,FirstName,LastName,CompanyName,"
                    + "MobilePhone,BusinessPhone,HomePhone,Emails,AddressLine1,AddressLine2,City,State,Country,ZipCode,CategoryID,IsDeleted,ImageURL,CreatedDate,UpdatedDate)values"
                    + "(@pPeopleGUID,@pRecordStatus,@pUserGUID,@pOrganizationGUID,@pIsPrimaryContact,@pPlaceGUID,@pMarketGUID,@pFirstName,@pLastName,@pCompanyName,"
                    + "@pMobilePhone,@pBusinessPhone,@pHomePhone,@pEmails,@pAddressLine1,@pAddressLine2,@pCity,@pState,@pCountry,@pZipCode,@pCategoryID,@pIsDeleted,@pImageURL,@pCreatedDate,@pUpdatedDate)", Param);
        }

        public int DeletePeople(Guid Peopleguid)
        {
            //Person People = context.People.Find(Peopleguid);
            //if (People != null)
            //    context.People.Remove(People);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pPeopleGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = Peopleguid;
            return context.Database.ExecuteSqlCommand("delete from People where PeopleGUID=@pPeopleGUID", Param);
        }

        public int DeletePeopleByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Peoplelist = (from p in dataContext.People
            //                      where p.OrganizationGUID == OrganizationGUID
            //                      select p).ToList();
            //    foreach (var item in Peoplelist)
            //    {
            //        dataContext.People.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.ExecuteSqlCommand("delete from People where OrganizationGUID=@pOrganizationGUID", Param);
        }
        public int DeletePeopleByPlaceGUID(Guid PlaceGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Peoplelist = (from p in dataContext.People
            //                      where p.PlaceGUID == PlaceGUID
            //                      select p).ToList();
            //    foreach (var item in Peoplelist)
            //    {
            //        dataContext.People.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pPlaceGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = PlaceGUID;
            return context.Database.ExecuteSqlCommand("delete from People where PlaceGUID=@pPlaceGUID", Param);
        }

        public int UpdatePeople(Person Person)
        {
            //context.Entry(Person).State = EntityState.Modified;
            SqlParameter[] Param = new SqlParameter[25];
            Param[0] = new SqlParameter("@pPeopleGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = Person.PeopleGUID;
            Param[1] = new SqlParameter("@pRecordStatus", SqlDbType.Int);
            Param[1].Value = (object)Person.RecordStatus ?? DBNull.Value;
            Param[2] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = Person.UserGUID;
            Param[3] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[3].Value = (object)Person.OrganizationGUID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pIsPrimaryContact", SqlDbType.Bit);
            Param[4].Value = (object)Person.IsPrimaryContact ?? DBNull.Value;
            Param[5] = new SqlParameter("@pPlaceGUID", SqlDbType.UniqueIdentifier);
            Param[5].Value = (object)Person.PlaceGUID ?? DBNull.Value;
            Param[6] = new SqlParameter("@pMarketGUID", SqlDbType.UniqueIdentifier);
            Param[6].Value = (object)Person.MarketGUID ?? DBNull.Value;
            Param[7] = new SqlParameter("@pFirstName", SqlDbType.NVarChar, 50);
            Param[7].Value = (object)Person.FirstName ?? DBNull.Value;
            Param[8] = new SqlParameter("@pLastName", SqlDbType.NVarChar, 50);
            Param[8].Value = (object)Person.LastName ?? DBNull.Value;
            Param[9] = new SqlParameter("@pCompanyName", SqlDbType.NVarChar, 50);
            Param[9].Value = (object)Person.CompanyName ?? DBNull.Value;
            Param[10] = new SqlParameter("@pMobilePhone", SqlDbType.NVarChar, 20);
            Param[10].Value = (object)Person.MobilePhone ?? DBNull.Value;
            Param[11] = new SqlParameter("@pBusinessPhone", SqlDbType.NVarChar, 20);
            Param[11].Value = (object)Person.BusinessPhone ?? DBNull.Value;
            Param[12] = new SqlParameter("@pHomePhone", SqlDbType.NVarChar, 20);
            Param[12].Value = (object)Person.HomePhone ?? DBNull.Value;
            Param[13] = new SqlParameter("@pEmails", SqlDbType.NVarChar, -1);
            Param[13].Value = (object)Person.Emails ?? DBNull.Value;
            Param[14] = new SqlParameter("@pAddressLine1", SqlDbType.NVarChar, 256);
            Param[14].Value = (object)Person.AddressLine1 ?? DBNull.Value;
            Param[15] = new SqlParameter("@pAddressLine2", SqlDbType.NVarChar, 256);
            Param[15].Value = (object)Person.AddressLine2 ?? DBNull.Value;
            Param[16] = new SqlParameter("@pCity", SqlDbType.NVarChar, 128);
            Param[16].Value = (object)Person.City ?? DBNull.Value;
            Param[17] = new SqlParameter("@pState", SqlDbType.NVarChar, 128);
            Param[17].Value = (object)Person.State ?? DBNull.Value;
            Param[18] = new SqlParameter("@pCountry", SqlDbType.NVarChar, 128);
            Param[18].Value = (object)Person.Country ?? DBNull.Value;
            Param[19] = new SqlParameter("@pZipCode", SqlDbType.NVarChar, 20);
            Param[19].Value = (object)Person.ZipCode ?? DBNull.Value;
            Param[20] = new SqlParameter("@pCategoryID", SqlDbType.Int);
            Param[20].Value = (object)Person.CategoryID ?? DBNull.Value;
            Param[21] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[21].Value = (object)Person.IsDeleted ?? DBNull.Value;
            Param[22] = new SqlParameter("@pImageURL", SqlDbType.NVarChar, -1);
            Param[22].Value = (object)Person.ImageURL ?? DBNull.Value;
            Param[23] = new SqlParameter("@pCreatedDate", SqlDbType.DateTime);
            Param[23].Value = (object)Person.CreatedDate ?? DBNull.Value;
            Param[24] = new SqlParameter("@pUpdatedDate", SqlDbType.DateTime);
            Param[24].Value = (object)Person.UpdatedDate ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("update People set RecordStatus=@pRecordStatus,UserGUID=@pUserGUID,OrganizationGUID=@pOrganizationGUID,IsPrimaryContact=@pIsPrimaryContact,"
                    + "PlaceGUID=@pPlaceGUID,MarketGUID=@pMarketGUID,FirstName=@pFirstName,LastName=@pLastName,CompanyName=@pCompanyName,"
                    + "MobilePhone=@pMobilePhone,BusinessPhone=@pBusinessPhone,HomePhone=@pHomePhone,Emails=@pEmails,AddressLine1=@pAddressLine1,"
                    + "AddressLine2=@pAddressLine2,City=@pCity,State=@pState,Country=@pCountry,ZipCode=@pZipCode,CategoryID=@pCategoryID,IsDeleted=@pIsDeleted,"
                    + "ImageURL=@pImageURL,CreatedDate=@pCreatedDate,UpdatedDate=@pUpdatedDate where PeopleGUID=@pPeopleGUID", Param);
        }

        //public int Save()
        //{
        //    return context.SaveChanges();
        //}

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}


