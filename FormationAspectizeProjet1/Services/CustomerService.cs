using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using System.Security.Permissions;

namespace FormationAspectizeProjet1.Services
{

    public interface ICustomerService
    {

        DataSet GetCustomers(DataSet ds);

    }

    

    [Service(Name = "CustomerService")]
    public class CustomerService : ICustomerService //, IInitializable, ISingleton
    {

        IDataManager dm;
        public CustomerService()
        {
            dm = EntityManager.FromDataBaseService("DataService");
        }


        DataSet ICustomerService.GetCustomers(DataSet ds)
        {
           System.Diagnostics.Debug.WriteLine("Hello fonction GetCustomers exécutée");

            dm.LoadEntities<Customer>();
            return dm.Data;
        }




    }
}