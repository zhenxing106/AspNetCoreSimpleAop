﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AopLibrary.CusImplement
{
    internal class RootServiceFactory<T> : IRootServiceFactory<T>
    {
        private T _instance { get; set; }
        public RootServiceFactory(T t, ISimpleAop aop)
        {
            _instance = t;
            _aop = aop;
        }

        private ISimpleAop _aop { get; set; }
        private T Imp => _instance;

        public async Task<TResponse> Invoke<TResponse>(string methodName, object[] args)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new Exception("需要调用的方法名不能为空");
            }
            var method = _instance.GetType().GetMethods().Where(x => x.DeclaringType.Name == _instance.GetType().Name).Where(x => x.Name.ToLower().Equals(methodName.ToLower())).FirstOrDefault();
            if (method == null)
            {
                throw new Exception($"方法{methodName}不存在请检查");
            }
            var task = await InvokeCore(method, args);
            if (task.GetType().BaseType.Name == "Task")
                return await (Task<TResponse>)task;
            else
                return (TResponse)task;
        }

        public async Task Invoke(string methodName, object[] args)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new Exception("需要调用的方法名不能为空");
            }
            var method = _instance.GetType().GetMethods().Where(x => x.DeclaringType.Name == _instance.GetType().Name).Where(x => x.Name.ToLower().Equals(methodName.ToLower())).FirstOrDefault();
            if (method == null)
            {
                throw new Exception($"方法{methodName}不存在请检查");
            }
            await InvokeCore(method, args);
        }

        private async Task<object> InvokeCore(MethodInfo targetMethod, object[] args)
        {
            await _aop.Before(args);
            var result = targetMethod.Invoke(_instance, args);
            var afterResult = await _aop.After(result);
            return afterResult;
        }
    }
}