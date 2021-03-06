﻿using System;

namespace Entities.Http
{
    public abstract class Resource
    {
        protected abstract (int status, string content) Get(IHttpRequest request);
        protected abstract (int status, string content) Post(IHttpRequest request);
        protected abstract (int status, string content) Put(IHttpRequest request);
        protected abstract (int status, string content) Delete(IHttpRequest request);

        public (int status, string content) Map(IHttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            
            var httpRequest = (IHttpRequest)request;
            switch (httpRequest.Method)
            {
                case "GET":
                    return Get(httpRequest);
                case "POST":
                    return Post(httpRequest);
                case "PUT":
                    return Put(httpRequest);
                case "DELETE":
                    return Delete(httpRequest);
                default:
                    throw new ApplicationException("Method not supported");
            }
        }
    }
}
