using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace XHS.Build.Common.Helps
{
    public class HpWcfInvoker
    {
        /// <summary>
        /// 执行方法   WSHttpBinding
        /// </summary>
        /// <typeparam name="T">服务接口</typeparam>
        /// <param name="url">wcf地址</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数列表</param>
        public static object ExecuteMetod<T>(string url, string methodName, params object[] args)
        {
            WSHttpBinding binding = new WSHttpBinding();
            EndpointAddress endpoint = new EndpointAddress(url);

            using (ChannelFactory<T> channelFactory = new ChannelFactory<T>(binding, endpoint))
            {
                T instance = channelFactory.CreateChannel();
                using (instance as IDisposable)
                {
                    try
                    {
                        Type type = typeof(T);
                        MethodInfo mi = type.GetMethod(methodName);
                        return mi.Invoke(instance, args);
                    }
                    catch (Exception ex)
                    {
                        (instance as ICommunicationObject).Abort();
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 执行方法 (nettcpbinding 绑定方式)
        /// </summary>
        /// <typeparam name="T">服务接口</typeparam>
        /// <param name="url">wcf地址</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数列表</param>
        /// <returns></returns>
        public static object ExecuteMethod<T>(string url, string methodName, Dictionary<string, string> pairs = null)
        {
            //EndpointAddress address = new EndpointAddress(url);
            //Binding bindinginstance = null;
            //BasicHttpBinding ws = new BasicHttpBinding();
            //ws.MaxReceivedMessageSize = 20971520;
            //bindinginstance = ws;
            //using (ChannelFactory<T> channel = new ChannelFactory<T>(bindinginstance, address))
            //{
            //    T instance = channel.CreateChannel();
            //    using (instance as IDisposable)
            //    {
            //        try
            //        {
            //            Type type = typeof(T);
            //            MethodInfo mi = type.GetMethod(methodName);
            //            return mi.Invoke(instance, args);
            //        }
            //        catch (Exception ex)
            //        {
            //            (instance as ICommunicationObject).Abort();
            //            throw ex;
            //        }
            //    }
            //}

            try
            {
                string param = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                    "<s:Header/>" +
                    "<s:Body>" +
                    "<" + methodName + " xmlns=\"http://tempuri.org/\">";
                if (pairs != null && pairs.Count > 0)
                {
                    foreach (var item in pairs)
                    {
                        param += "<" + item.Key + ">" + item.Value + "</" + item.Key + ">";
                    }

                }
                param += "</" + methodName + ">" +
                "</s:Body>" +
                "</s:Envelope>";
                byte[] bs = Encoding.UTF8.GetBytes(param);
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url + "?wsdl");

                myRequest.Method = "POST";
                myRequest.ContentType = "text/xml;charset=utf-8";
                myRequest.Headers.Add("SOAPAction", "http://tempuri.org/IWSTemplateMessage/" + methodName);
                myRequest.ContentLength = bs.Length;

                using (Stream reqStream = myRequest.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                }
                using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse())
                {
                    StreamReader mysr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                    string responseResult = mysr.ReadToEnd();
                    return responseResult;
                }
            }
            //捕获异常
            catch (Exception ex)
            {
                return null;
            }
        }



        /// <summary>
        /// 执行方法 (nettcpbinding 绑定方式)
        /// </summary>
        /// <typeparam name="T">服务接口</typeparam>
        /// <param name="url">wcf地址</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数列表</param>
        /// <returns></returns>
        public static object ExecuteMethd<T>(string url, string methodName, params object[] args)
        {
            EndpointAddress address = new EndpointAddress(url);
            Binding bindinginstance = null;
            BasicHttpBinding ws = new BasicHttpBinding();
            ws.MaxReceivedMessageSize = 20971520;
            bindinginstance = ws;
            using (ChannelFactory<T> channel = new ChannelFactory<T>(bindinginstance, address))
            {
                T instance = channel.CreateChannel();
                using (instance as IDisposable)
                {
                    try
                    {
                        Type type = typeof(T);
                        MethodInfo mi = type.GetMethod(methodName);
                        return mi.Invoke(instance, args);
                    }
                    catch (Exception ex)
                    {
                        (instance as ICommunicationObject).Abort();
                        throw ex;
                    }
                }
            }
        }
    }
}
