using Mantle.Data;

namespace Mantle.Tenants.Domain
{
    public class Tenant : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        // TODO: Support SSL
        //public bool SslEnabled { get; set; }

        //public string SecureUrl { get; set; }

        public string Hosts { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }
}