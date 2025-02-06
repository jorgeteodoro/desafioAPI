using DesafioAPISimulacao.Application.Interfaces;
using DesafioAPISimulacao.Core.Models;
using DesafioAPISimulacao.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Reflection;


namespace DesafioAPISimulacao.API.Contract
{
    [AllowAnonymous]
    public abstract class ServiceBaseController<TEntity> : ControllerBase where TEntity : BaseEntity
    {
        //private readonly ServiceException _serviceException;
        private readonly IServiceBase<TEntity> _serviceBase;

        protected IServiceBase<TEntity> ServiceBase => _serviceBase;

        public ServiceBaseController(IServiceBase<TEntity> serviceBase)
        {
            _serviceBase = serviceBase;
        }

        public virtual async Task<ResultRequest> Insert([FromBody] TEntity entity)
        {
            try
            {
                await _serviceBase.Insert(entity);
                return new ResultRequest(true, entity);
            }
            catch (Exception ex)
            {
                throw ex;
                //return _serviceException.ResultHandleException(false, ex);
            }
        }

    }
}
