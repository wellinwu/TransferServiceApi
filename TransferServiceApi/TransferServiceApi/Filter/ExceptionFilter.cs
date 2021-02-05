using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferServiceApi.Common;
using static TransferServiceApi.Help.LogHelper;

namespace TransferServiceApi.Filter
{
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            #region 添加异常日志
            string body = string.Empty;
            try
            {
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                StreamReader reqStream = new StreamReader(context.HttpContext.Request.Body);
                body = reqStream.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Error("context.HttpContext.Request.Body抛出异常", ex);
            }

            var path = context.HttpContext.Request.Path.ToString().Split('/');
            var method = path[path.Length - 1];//接口名
            var controller = path[path.Length - 2];//控制器
            var bodystr = "";
            if (!string.IsNullOrEmpty(body))
            {
                bodystr = JsonConvert.SerializeObject(JObject.Parse(body.Trim()));//去掉空格
            }

            StringBuilder formStr = new StringBuilder();
            try
            {
                var reqForm = context.HttpContext.Request.Form;

                if (reqForm != null && reqForm.Keys.Count > 0)
                {
                    foreach (var item in reqForm.Keys)
                    {
                        formStr.Append(
                            string.Format("key = {0}，value = {1}  ", item, reqForm[item].ToString())
                            );
                    }
                }
            }
            catch (Exception formEx)
            {
                formStr.Append($"context.HttpContext.Request.Form 抛出异常：{formEx.Message}");
            }

            StringBuilder errorText = new StringBuilder(
                string.Format("请求控制器：{0}  请求路径(Path）= {1}  页面传递的参数值(QueryString) = {2}  请求包体(Body) = {3}  异常信息(Form) = {4}\r\n",
               context.ActionDescriptor.DisplayName,
               context.HttpContext.Request.Path,
               context.HttpContext.Request.QueryString,
               bodystr,
               formStr.ToString()
               ));

            Log.Error(errorText.ToString(), context.Exception);
            #endregion

            OnNormalException(context);
        }

        public void OnNormalException(ExceptionContext context)
        {
            DataResult result = new DataResult
            {
                BS = "-99",
                Msg = "系统发生错误或异常，请联系管理员！"
            };
            ContentResult contentRes = new ContentResult
            {
                Content = JsonConvert.SerializeObject(result),
                ContentType = "application/json"
            };
            context.Result = contentRes;
        }
    }
}
