using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBooksASPNetCore2OpenIDConnect.Models.QuickBooksOnline
{
    public class BillAddr
    {
        public string Id { get; set; }
        public string Country { get; set; }
        public string CountrySubDivisionCode { get; set; }
    }

    public class CurrencyRef
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class MetaData
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class PrimaryEmailAddr
    {
        public string Address { get; set; }
    }

    public class PrimaryPhone
    {
        public string FreeFormNumber { get; set; }
    }

    public class Customer
    {
        public bool Taxable { get; set; }
        public BillAddr BillAddr { get; set; }
        public bool Job { get; set; }
        public bool BillWithParent { get; set; }
        public double Balance { get; set; }
        public double BalanceWithJobs { get; set; }
        public CurrencyRef CurrencyRef { get; set; }
        public string PreferredDeliveryMethod { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public MetaData MetaData { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string FullyQualifiedName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public PrimaryEmailAddr PrimaryEmailAddr { get; set; }
        public PrimaryPhone PrimaryPhone { get; set; }
    }

    public class QueryResponse
    {
        public List<Customer> Customer { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
    }

    public class RootObject
    {
        public QueryResponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }
}
