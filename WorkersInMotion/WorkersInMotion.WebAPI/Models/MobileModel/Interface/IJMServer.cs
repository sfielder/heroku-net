using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.WebAPI.Models.MobileModel.Interface
{
    public interface IJMServer
    {
        bool ValidateUser(string SessionGUID);
        string convertdate(DateTime date);
        System.Guid GetUserGUID(string SessionGUID);
        string GetOrganizationName(Guid JobGUID);
        JMResponse GetJobs(Guid UserGUID);
        JMResponse GetOpenJobs(Guid UserGUID);
        int UpdateJobStatus(UpdateJobStatusRequest JMOwnJobRequest, Guid UserGUID);
        //int UploadJobs(UploadJobRequest UploadJobRequest, Guid UserGUID);
        int UploadJobs(UploadJobRequestNew UploadJobRequest, Guid UserGUID);
        ConfigurationResponse GetConfiguration(Guid UserGUID);
        int InsertJobFormData(JobFormDataRequest JobFormDataRequest);
        MobileJob ConvertJobForMobile(Job Job, string POJson);
        MobileJob ConvertJobForMobile(Job Job, string POJson, string UserGUID, string OrganizationGUID);
        MobilePO ConvertPOtoMobilePO(POs PO);
        POs ConvertMobilePOtoPO(MobilePO PO);

        JobStatusResponse GetJobStatus(JobStatusRequest JobStatusRequest);
        int AssignJob(AssignJobRequest AssignJobRequest, Guid UserGUID);
        int UpdateForumStatus(JobForumRequest JobForumRequest, Guid UserGUID);
        int InsertForumEntries(CreateForumEntryRequest InsertForumEntryRequest, Guid UserGUID);
        Forum GetForumEntries(Guid JobGUID);
        MobilePO GetPOs(MobilePO MobilePO);
        List<MobilePO> GetPOList(string SessionID);
        int InsertPO(MobilePO MobilePO);
        MobileJob CreateJobForCustomerStop(CreateJobForCustomerStopRequest jobRequest, Guid UserGUID, ref int errorCode);
        MobileJob CreateJobForPO(CreateJobForPORequest jobRequest, Guid UserGUID, ref int errorCode);
        MobileJob CreateVisitForPO(CreateVisitForPORequest jobRequest, Guid UserGUID, ref int errorCode);


        //JMResponse GetJobSchemaByGroupID(string GroupCode);
        //JMResponse GetJobSchemaByJobLogicalID(string JobLogicalID);
        //JMResponse GetJobByFilter(JobRequest jobrequest);
        //JMResponse GetJobByOrganization(string SessionID);
        //void UploadJob(MUploadJob _uploadJob);
        string GetUploadFileAttachmetsPath(string pJobGUID, string pSessionID);
        UploadJobAttachmentResponse UploadJobAttachment(UploadJobAttachmentRequest jobAttachmentRequest, Guid UserGUID);

        int DeleteJobs(DeleteJobRequest pDeleteJobRequest, string SessionID);

        Guid GetOrganizationGUID(Guid USerGUID);
    }
}
