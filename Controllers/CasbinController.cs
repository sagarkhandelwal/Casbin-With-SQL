using CasbinRBAC.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetCasbin;
using System;
using System.Linq;

namespace CasbinRBAC.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CasbinController : Controller
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly Enforcer _enforcer;
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly string unauthorised = "Unauthorised User";
        #endregion

        #region Ctor
        public CasbinController(CasbinDbContext<int> databaseContext, IConfiguration configuration, ApplicationDBContext applicationDBContext)
        {
            _configuration = configuration;
            var efAdapter = new CasbinDbAdapter<int>(databaseContext);
            _enforcer = new Enforcer("CasbinConfig/rbac_model.conf", efAdapter);
            _applicationDBContext = applicationDBContext;
        }
        #endregion

        /// <summary>
        /// sub is for user
        /// dom is for aff
        /// obj is for roleName
        /// act is for privilege
        /// </summary>
        /// <param name="user"></param>
        /// <param name="aff"></param>
        /// <param name="roleName"></param>
        /// <param name="privilege"></param>
        [HttpGet("checkRbacWithAffiliation")]
        public IActionResult CheckRbacWithAffiliation(string user, string aff, string roleName, string privilege, string permission)
        {
            var response = _enforcer.Enforce(user, aff, roleName, privilege, permission);
            return Ok(response);
        }

        [HttpGet("Read")]
        public Object Read(string user, string aff, string roleName, string privilege, string permission)
        {
            bool response = _enforcer.Enforce(user, aff, roleName, privilege, permission);
            if(response)
            {
                try
                {

                    var a = _applicationDBContext.Contacts.FromSqlRaw("EXECUTE AS USER = {0} SELECT * FROM dbo.Contacts REVERT; ", user).ToList();
                    return a;
                    
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            else
            {
                return unauthorised;
            }
            
        }

        [HttpGet("Insert")]
        public String Insert(string user, string aff, string roleName, string privilege, string permission)
        {
            bool response = _enforcer.Enforce(user, aff, roleName, privilege, permission);
            if (response)
            {
                try
                {
                    string name = "User2", address = "AddUser1", city = "CityUser1", state = "StateUser1", zip = "ZipUser1", email = "User1@gmail.com";

                    var commandText = "EXECUTE AS USER = @p0 Insert into dbo.Contacts(Name, Address, City, State, Zip, Email) Values(@p1, @p2, @p3, @p4, @p5, @p6) REVERT; ";

                    var result = _applicationDBContext.Database.ExecuteSqlRaw(commandText, user, name, address, city, state, zip, email);

                    return "Data Inserted";
                }
                catch (Exception e)
                {
                    return e.Message; 
                }
            }
            else
            {
                return unauthorised;
            }
        }

        [HttpGet("Update")]
        public String Update(string user, string aff, string roleName, string privilege, string permission)
        {
            bool response = _enforcer.Enforce(user, aff, roleName, privilege, permission);
            if (response)
            {
                try
                {
                    string zip = "ZipUser4";
                    int contactId = 4;
                    var commandText = "EXECUTE AS USER = @p0 UPDATE Contacts SET Zip = @p1 WHERE ContactId = @p2 REVERT";
                    var result = _applicationDBContext.Database.ExecuteSqlRaw(commandText, user, zip, contactId);

                    if(result == 0)
                    {
                        return "User don't have permission to update the data on a particular row or 0 affected rows";
                    }
                    return "Data Updated";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            else
            {
                return unauthorised;
            }
        }

        [HttpGet("Delete")]
        public String Delete(string user, string aff, string roleName, string privilege, string permission)
        {
            bool response = _enforcer.Enforce(user, aff, roleName, privilege, permission);
            if (response)
            {
                try
                {
                    int contactId = 4;
                    var commandText = "EXECUTE AS USER = @p0 DELETE FROM dbo.Contacts WHERE ContactId = @p1 REVERT";

                    var result = _applicationDBContext.Database.ExecuteSqlRaw(commandText, user, contactId);

                    if (result == 0)
                    {
                        return "User don't have permission to delete the data on a particular row or 0 affected rows";
                    }

                    return "Data Deleted";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            else
            {
                return unauthorised;
            }
        }
    }
    
}
