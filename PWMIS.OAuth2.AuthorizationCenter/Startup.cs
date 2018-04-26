﻿using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PWMIS.OAuth2.AuthorizationCenter
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                AuthenticationMode = AuthenticationMode.Active,
                TokenEndpointPath = new PathString("/api/token"), //获取 access_token 授权服务请求地址
                AuthorizeEndpointPath = new PathString("/authorize"), //获取 authorization_code 授权服务请求地址
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(60), //access_token 过期时间，默认10秒太短

                Provider = new OpenAuthorizationServerProvider(), //access_token 相关授权服务
                AuthorizationCodeProvider = new OpenAuthorizationCodeProvider(), //authorization_code 授权服务
                RefreshTokenProvider = new OpenRefreshTokenProvider() //refresh_token 授权服务
            };
            app.UseOAuthBearerTokens(OAuthOptions); //表示 token_type 使用 bearer 方式
        }

        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            ConfigureAuth(app);

            var configuration = new HttpConfiguration();
            WebApiConfig.Register(configuration);
            app.UseWebApi(configuration);

            WriteEventLog("Authorization Center Start OK.", EventLogEntryType.Information);
        }

        /// <summary>
        /// 将日志写到Windows系统日志的 Application事件源
        /// </summary>
        /// <param name="message"></param>
        /// <param name="enventType"></param>
        public static void WriteEventLog(string message, EventLogEntryType enventType)
        {
            EventLog eventLog = new EventLog();
            eventLog.Source = "Application";
            eventLog.WriteEntry("PWMIS OAuth2  Event:" + message, enventType);
        }
    }
}