﻿using System;
using System.Web;
using NLog;

namespace YouMap.Framework.Exceptions.Modules
{
    public class ErrorLoggingModule : IHttpModule
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication application)
        {
            application.Error += OnError;
        }

        private void OnError(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            Exception exception = application.Server.GetLastError();

            if (exception is HttpSecurityException || 
                exception is HttpNotFoundException ||
                (exception is HttpException && ((HttpException)exception).ErrorCode == 404))
            {
                _logger.Warn(String.Format("{1} exception type: {0}", exception.GetType(), exception.Message));
            }
            else
            {
                _logger.FatalException("Global Error at Admin", exception);
            }
        }
    }
}
