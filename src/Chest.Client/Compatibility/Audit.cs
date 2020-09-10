namespace Chest.Client.AutorestClient
{
    public class Audit : IAudit
    {
        public Api.IAuditApi RefitClient { get; private set; }

        public Audit(Api.IAuditApi refitClient)
        {
            RefitClient = refitClient;
        }
    }
}