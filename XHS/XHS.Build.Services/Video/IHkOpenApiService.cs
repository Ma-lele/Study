using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Base;
using XHS.Build.Model.Base;

namespace XHS.Build.Service.Video
{
    public interface IHkOpenApiService : IBaseServices<BaseEntity>
    {

        /// <summary>
        /// 获取摄像头直播流地址
        /// </summary>
        /// <param name="cameracode">摄像头编号</param>
        /// <param name="channel">通道号</param>
        /// <param name="cameraparam">参数</param>
        ///  <param name="streamType">码流类型(0-主码流,1-子码流),未填默认为主码流</param>
        ///  <param name="protocol">协议类型（rtsp-rtsp协议,rtmp-rtmp协议,hls-hLS协议），未填写为rtsp协议</param>
        ///  <param name="transmode">协议类型( 0-udp，1-tcp),默认为tcp</param
        /// <returns></returns>
        public BnCameraResult<BnPlaybackURL> GetRealurl(BnCamera bc);

        /// <summary>
        /// 获取摄像头回看流地址
        /// </summary>
        /// <param name="cameracode">摄像头编号</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="recordLocation">录像存储位置（0-中心存储，1-设备存储）,默认设备存储</param>
        /// <param name="protocol">协议类型（rtsp-rtsp协议,rtmp-rtmp协议,hLS-hLS协议，默认为rtsp）</param>
        /// <returns></returns>
        public BnCameraResult<BnPlaybackURL> GetPlayBackurl(BnCamera bc);

        /// <summary>
        /// 云台操作，对监控点云台方向，转动速度进行操作。
        /// </summary>
        /// <param name="cameracode">摄像头编号</param>
        /// <param name="action">开始或停止操作(1 开始 0 停止)</param>
        /// <param name="command">控制命令(不区分大小写) 说明： LEFT 左转 RIGHT 右转 UP 上转 DOWN 下转 ZOOM_IN 焦距变大 ZOOM_OUT 焦距变小 
        ///                       LEFT_UP 左上 LEFT_DOWN 左下 RIGHT_UP 右上 RIGHT_DOWN 右下 FOCUS_NEAR 焦点前移 FOCUS_FAR 焦点后移 
        ///                       IRIS_ENLARGE 光圈扩大 IRIS_REDUCE 光圈缩小 以下命令presetIndex不可为空： GOTO_PRESET到预置点
        /// <param name="presetIndex"> 预置点编号(取值范围为1-128)</param>
        /// <param name="speed">云台速度(取值范围为1-100，默认40)</param>
        /// <returns></returns>
        public BnCameraResult<BnPlaybackURL> ptz(BnCamera bc);

        /// <summary>
        /// 更新摄像头在线状态
        /// </summary>
        /// <param name="cameracode"></param>
        /// <returns></returns>
        public int UpdateCameraState(string cameracode,int type, int upstatehis = 0);
    }
}
