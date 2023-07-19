using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Cameras
{
    public interface ICameraService:IBaseServices<GCCameraEntity>
    {

        /// <summary>
        /// 根据siteid获取摄像头列表
        /// </summary>
        /// <param name="siteid">监测点ID</param>
        /// <returns></returns>
        Task<DataTable> getCameraList(int siteid);


        /// <summary>
        /// 获取摄像头列表
        /// </summary>
        /// <param name="siteid">监测点ID</param>
        /// <returns></returns>
        Task<DataTable> getCameraListByDevCode(string  devcode);

        Task<PageOutput<VSiteCamera>> GetSiteCameraPageList(int groupid, string keyword, int page, int size);

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doDyCameraInsert(List<SugarParameter> param);

        /// <summary>
        /// 摄像头编号逗号拼接新合盛流媒体
        /// </summary>
        /// <param name="type">视频类型</param>
        /// <returns></returns>
        Task<string> CameraCodeSpliceAsync(string type);

        /// <summary>
        /// 更新摄像头在线状态并记录
        /// </summary>
        /// <param name="cameracode"></param>
        /// <param name="bonline"></param>
        /// <returns></returns>
        Task<int> CamerabonlineUpdateAsync(string cameracode, int bonline,int upstatehis=0);

        /// <summary>
        /// 获取摄像头信息
        /// </summary>
        /// <param name="cameracode"></param>
        /// <returns></returns>
        Task<List<VSiteCamera>> GetCameraInfoByCameracode(string cameracode);
    }
}
