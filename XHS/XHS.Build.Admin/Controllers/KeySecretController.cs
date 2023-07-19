using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Response;
using XHS.Build.Services.KeySecretConfig;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class KeySecretController : ControllerBase
    {
        private readonly IKeySecretService _keySecretService;
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        public KeySecretController(IKeySecretService keySecretService, IUser user)
        {
            _keySecretService = keySecretService;
            _user = user;
        }


        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(string keyword = "", int page = 1, int size = 20)
        {
            var data = await _keySecretService.GetList(keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(TCCKeySecretConfig input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var rows = await _keySecretService.Add(input);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int id)
        {
            if (id <=0 )
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }

            bool isDel = await _keySecretService.DeleteById(id);
            if (isDel)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("删除失败");
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(TCCKeySecretConfig input)
        {
            if (input == null || string.IsNullOrEmpty(input.Name))
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            bool suc = await _keySecretService.Update(input);
            if (suc)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("修改失败");
            }
        }

        /// <summary>
        /// 获取详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(int id)
        {
            return ResponseOutput.Ok(await _keySecretService.QueryById(id));
        }
    }
}
