﻿using Autofac;
using AutofacRegister;
using Contract;
using IOrder.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Impl
{
    public class FirstImplService : IPlugin
    {
        private IRepositoryProvider reposttory;
        public FirstImplService()
        {
            using (var scope = CompositionRoot.BeginLifetimeScope())
            {
                 reposttory = scope.Resolve<IRepositoryProvider>();
               
            }
            //this.reposttory = CompositionRoot.BeginLifetimeScope().Resolve<IRepository>();
        }
        public async Task<object> Add( string entity)
        {
            await Task.CompletedTask;
            return "add oen 1";
        }

        public Task<object> Get(string entity)
        {
            return reposttory.GetRepository("Order").GetOrder(entity);
        }

        public async Task<object> Update( string entity)
        {
            await Task.CompletedTask;
            return "add one 2";
        }
    }
}
