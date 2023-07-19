
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using XHS.Build.Common.Helps;

namespace XHS.Build.Common.Sqlsugar
{
    /// <summary>
    /// SugarParameter参数生成器
    /// </summary>
    public class SgParams
    {
        const string AT = "@";
        private List<SugarParameter> _params = new List<SugarParameter>();
        /// <summary>
        /// 参数列表
        /// </summary>
        public List<SugarParameter> Params { get => _params; set => _params = value; }

        private SugarParameter _returnValue;
        private SugarParameter _output;
        /// <summary>
        /// 存储过程ReturnValue
        /// </summary>
        public int ReturnValue { get => _returnValue.Value.ObjToInt(); }
        /// <summary>
        /// 存储过程Output
        /// </summary>
        public string Output { get => Convert.ToString(_output.Value); }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="needReturnValue">是否需要ReturnValue</param>
        public SgParams(bool needReturnValue = false, bool needOutput = false)
        {
            this.NeetReturnValue(needReturnValue);
            this.NeetOutput(needOutput);
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, object value)
        {
            if (_params.Any(p => p.ParameterName == AT + name))
                this.Remove(name);
            _params.Add(new SugarParameter(AT + name, value));
        }

        /// <summary>
        /// 实体类转SugarParameter列表
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="item">实体类</param>
        /// <param name="needReturnValue">是否需要ReturnValue</param>
        /// <param name="needOutput">是否需要Output</param>
        /// <returns></returns>
        public List<SugarParameter> SetParams<T>(T item, bool needReturnValue = false, bool needOutput = false) where T : class
        {
            foreach (PropertyInfo pi in item.GetType().GetProperties())
            {
                this.Add(pi.Name, pi.GetValue(item));
            }

            this.NeetReturnValue(needReturnValue);
            this.NeetOutput(needOutput);

            return this._params;
        }

        /// <summary>
        /// 需要生成ReturnValue
        /// </summary>
        public void NeetReturnValue(bool isNeed = true)
        {
            if (isNeed)
            {
                _returnValue = new SugarParameter(AT + "return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
                _params.Add(_returnValue);
            }
        }

        /// <summary>
        /// 需要生成Output
        /// </summary>
        public void NeetOutput(bool isNeed = true)
        {
            if (isNeed)
            {
                _output = new SugarParameter(AT + "output", null, System.Data.DbType.String, ParameterDirection.Output);
                _params.Add(_returnValue);
            }
        }

        /// <summary>
        /// 删除参数
        /// </summary>
        /// <param name="name">参数名</param>
        public void Remove(string name)
        {
            SugarParameter item = this._params.Where(item => item.ParameterName == AT + name).FirstOrDefault();
            if (item != null)
                this._params.Remove(item);
        }
    }
}
