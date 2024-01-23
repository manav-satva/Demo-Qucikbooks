using ModelDataLibrary.ViewModels;
using Intuit.Ipp.Data;

namespace ModelDataLibrary.Helper
{
    public class ModelMapper
    {
        public static Customer Map_Customer_to_CustomerViewModel(CustomerViewModel obj)
        {
            Customer customer = new Customer();
            customer.Id = obj.Id;
            customer.DisplayName = obj.Name;
            customer.CompanyName = obj.CompanyName;
            customer.PrimaryEmailAddr = new EmailAddress { Address = obj.Email };
            customer.PrimaryPhone = new TelephoneNumber { FreeFormNumber = obj.Phone };
            customer.WebAddr = new WebSiteAddress { URI = obj.Website };
            customer.SyncToken = obj.SyncToken;
            return customer;
        }
        public static CustomerViewModel Map_CustomerViewModel_to_Customer(Customer customer)
        {
            CustomerViewModel obj = new CustomerViewModel();
            obj.Id = customer.Id;
            obj.Name = customer.DisplayName;
            obj.CompanyName = customer.CompanyName;
            obj.Email = customer.PrimaryEmailAddr?.Address ?? "";
            obj.Phone = customer.PrimaryPhone?.FreeFormNumber ?? "";
            obj.Website = customer.WebAddr?.URI ?? "";
            obj.SyncToken = customer.SyncToken;
            return obj;
        }
    }
}
