using Microsoft.Data.SqlClient;


namespace Infrastructure.Data.Helpers
{
    public class MsSqlDatabase
    {
        public SqlConnection SqlConnection { get; }
        public SqlCommand SqlCommand { get; }
        public SqlDataAdapter SqlDataAdapter { get; }


        public MsSqlDatabase(SqlConnection sqlConnection, SqlCommand sqlCommand, SqlDataAdapter sqlDataAdapter) 
        {
            SqlConnection = sqlConnection;
            SqlCommand = sqlCommand;
            SqlDataAdapter = sqlDataAdapter;
        }
    }
}
