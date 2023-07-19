using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Repository.SmsQueue;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.SmsQueue
{
    public class SmsQueueService : BaseServices<SmsQueueEntity>, ISmsQueueService
    {
        private readonly IUser _user;
        private readonly ISmsQueueRepository _smsQueueRepository;
        public SmsQueueService(ISmsQueueRepository smsQueueRepository, IUser user)
        {
            _user = user;
            _smsQueueRepository = smsQueueRepository;
            BaseDal = smsQueueRepository;
        }

        public async Task<int> doInsert(params SugarParameter[] param)
        {
            return await _smsQueueRepository.doInsert(param);
        }

        public async Task<int> doUpdate(long SQID, string resultmessage)
        {
            return await _smsQueueRepository.doUpdate(SQID, resultmessage);
        }

        public async Task<DataTable> getList(int SITEID)
        {
            return await _smsQueueRepository.getList(SITEID);
        }

        public async Task<DataTable> getSendList()
        {
            return await _smsQueueRepository.getSendList();
        }

        public async void SendSmsAll()
        {
            try
            {
                DataTable dt =await getSendList();
                int result = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        string smsResult = HpAliSMS.SendSms(Convert.ToString(dt.Rows[i]["phonenumber"]),
                                                            Convert.ToString(dt.Rows[i]["templatecode"]),
                                                            HttpUtility.HtmlDecode(Convert.ToString(dt.Rows[i]["jsonparam"])));
                        int ret =await _smsQueueRepository.doUpdate(Convert.ToInt64(dt.Rows[i]["SQID"]), smsResult);
                        result += ret;
                    }
                    catch (Exception ex)
                    {
                        //ULog.WriteError(ex.Message + Environment.NewLine +string.Format("SQID {0} ,phonenumber {1} ", dt.Rows[i]["SQID"], dt.Rows[i]["phonenumber"]), appName);
                    }
                }
                if (result > 0)
                {
                    //ULog.Write(string.Format("提醒发送了 {0} 次.", result), appName);
                }


            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message + Environment.NewLine + ex.StackTrace, appName);
            }
        }
    }
}
