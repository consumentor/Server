namespace Consumentor.ShopGun.Domain
{
   //NOTE: We have to discuss if we are going use this class!
    public static class DatabaseType
    {
        public const string Int = "int";
        //NOTE: Is now replaced with : CanBeNull = false; DbType = DatabaseType.Int
        //public const string IntNotNull = "int NOT NULL";
        public const string IntIdentity = "int IDENTITY"; 
        public const string Datetime2 = "datetime2";
        //NOTE:Variables shall not contain numbers
        public const string Nvarchar50 = "NVarChar(50)";
        public const string NvarcharMax = "NVarChar(MAX)";
    }
}
