using System.Linq;
using System.Threading.Tasks;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Mantle.Caching;
using Mantle.Data;
using Mantle.Data.Services;
using Mantle.Infrastructure;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Mantle.Web.Mvc
{
    /// <summary>
    /// <para>Generic controller for data Select/Insert/Update/Delete... ONly to be used temporarily.</para>
    /// <para>After OData is released and stable on ASP.NET Core, then we will use that instead.</para>
    /// <para>See GenericODataController from MantleCMS for what to change this to in future.</para>
    /// </summary>
    public abstract class MantleGenericDataController<TEntity, TKey> : MantleController
        where TEntity : class
    {
        #region Non-Public Properties

        protected IGenericDataService<TEntity> Service { get; private set; }

        //protected ILogger Logger { get; private set; }

        #endregion Non-Public Properties

        #region Constructors

        public MantleGenericDataController(IGenericDataService<TEntity> service)
        {
            Service = service;
            var loggerFactory = EngineContext.Current.Resolve<ILoggerFactory>();
            //Logger = loggerFactory.CreateLogger(GetType());
        }

        public MantleGenericDataController(IRepository<TEntity> repository)
        {
            var cacheManager = EngineContext.Current.Resolve<ICacheManager>();
            Service = new GenericDataService<TEntity>(cacheManager, repository);
            var loggerFactory = EngineContext.Current.Resolve<ILoggerFactory>();
            //Logger = loggerFactory.CreateLogger(GetType());
        }

        #endregion Constructors

        #region Public Methods

        // NOTE: Tempoarily using HtppPost because of problem with KendoGrid which sends querystrings with brackets ([]) instead of dots (.)
        // Example: Instead of sending sorts[0].name=something, it will instead send sorts[0][name]=something and the model binders in MVC
        //  can't handle that. So we change to use HttpPost instead.. and will change back to HttpGet when using OData and also change route
        //  from "get" to "" (empty) later as well.
        [HttpPost]
        [Route("get")]
        public virtual async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Unauthorized();
            }

            using (var connection = Service.OpenConnection())
            {
                var query = connection.Query();
                query = ApplyMandatoryFilter(query);

                var grid = new CustomKendoGridEx<TEntity>(request, query);
                return Json(grid);
            }
        }

        [HttpGet]
        [Route("{key}")]
        public virtual async Task<IActionResult> Get(TKey key)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Unauthorized();
            }
            var entity = await Service.FindOneAsync(key);

            if (!CanViewEntity(entity))
            {
                return Unauthorized();
            }

            return Json(JObject.FromObject(entity));
        }

        [HttpPut]
        [Route("{key}")]
        public virtual async Task<IActionResult> Put(TKey key, [FromBody]TEntity entity)
        {
            if (!CanModifyEntity(entity))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!key.Equals(GetId(entity)))
            {
                return BadRequest();
            }

            try
            {
                OnBeforeSave(entity);
                await Service.UpdateAsync(entity);
                OnAfterSave(entity);
            }
            catch (DbUpdateConcurrencyException x)
            {
                Logger.LogError(new EventId(), x, x.Message);

                if (!EntityExists(key))
                {
                    return NotFound();
                }
                else { throw; }
            }

            return Ok(entity);
        }

        [HttpPost]
        [Route("")]
        public virtual async Task<IActionResult> Post([FromBody]TEntity entity)
        {
            if (!CanModifyEntity(entity))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SetNewId(entity);

            OnBeforeSave(entity);
            await Service.InsertAsync(entity);
            OnAfterSave(entity);

            return Ok(entity);
        }

        // NOT USED HERE
        //// PATCH: odata/<Entity>(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public virtual async Task<IActionResult> Patch(TKey key, TEntity entity)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    TEntity existingEntity = await Service.FindOneAsync(key);

        //    if (existingEntity == null)
        //    {
        //        return NotFound();
        //    }

        //    if (!CanModifyEntity(existingEntity))
        //    {
        //        return Unauthorized();
        //    }

        //    try
        //    {
        //        await Service.UpdateAsync(entity);
        //        //db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException x)
        //    {
        //        Logger.LogError(new EventId(), x, x.Message);

        //        if (!EntityExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else { throw; }
        //    }

        //    return Ok(entity);
        //}

        [HttpDelete]
        [Route("{key}")]
        public virtual async Task<IActionResult> Delete(TKey key)
        {
            TEntity entity = await Service.FindOneAsync(key);

            if (entity == null)
            {
                return NotFound();
            }

            if (!CanModifyEntity(entity))
            {
                return Unauthorized();
            }

            await Service.DeleteAsync(entity);

            return NoContent();
        }

        #endregion Public Methods

        #region Non-Public Methods

        protected virtual bool EntityExists(TKey key)
        {
            return Service.FindOne(key) != null;
        }

        protected abstract TKey GetId(TEntity entity);

        /// <summary>
        /// Should only be necessary for Guid types
        /// </summary>
        /// <param name="entity"></param>
        protected abstract void SetNewId(TEntity entity);

        /// <summary>
        /// Add any filters which must be applied for the client. Mostly used for fields such as "TenantId", where you don't want
        /// the user viewing data for a different site (tenant)
        /// </summary>
        /// <param name="entity"></param>
        protected virtual IQueryable<TEntity> ApplyMandatoryFilter(IQueryable<TEntity> query)
        {
            // Do nothing, by default
            return query;
        }

        protected virtual bool CanViewEntity(TEntity entity)
        {
            if (entity == null)
            {
                return false;
            }

            return CheckPermission(ReadPermission);
        }

        protected virtual bool CanModifyEntity(TEntity entity)
        {
            if (entity == null)
            {
                return false;
            }

            return CheckPermission(WritePermission);
        }

        protected virtual void OnBeforeSave(TEntity entity)
        {
        }

        protected virtual void OnAfterSave(TEntity entity)
        {
        }

        //protected static bool CheckPermission(Permission permission)
        //{
        //    if (permission == null)
        //    {
        //        return true;
        //    }

        //    var authorizationService = EngineContext.Current.Resolve<IAuthorizationService>();
        //    var workContext = EngineContext.Current.Resolve<IWorkContext>();
        //    return authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
        //}

        protected abstract Permission ReadPermission { get; }

        protected abstract Permission WritePermission { get; }

        #endregion Non-Public Methods
    }
}