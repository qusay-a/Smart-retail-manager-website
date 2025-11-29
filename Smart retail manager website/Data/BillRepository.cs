using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Smart_retail_manager_website.Models;

namespace Smart_retail_manager_website.Data
{
    public class BillRepository
    {
        private readonly IConfiguration _config;

        public BillRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<BillSummaryRow>> GetBillSummaryAsync()
        {
            var result = new List<BillSummaryRow>();

            // 1) Get connection string
            var connString = _config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found. " +
                    "Make sure it exists in appsettings.json under ConnectionStrings.");
            }

            // 2) Query the SQL view
            const string sql = @"
                SELECT BillID, Cname, DateOfInvoice, LineTotal
                FROM vw_billSummary;
            ";

            using var conn = new SqlConnection(connString);
            using var cmd = new SqlCommand(sql, conn);

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            var ordBillID = reader.GetOrdinal("BillID");
            var ordCname = reader.GetOrdinal("Cname");
            var ordDate = reader.GetOrdinal("DateOfInvoice");
            var ordTotal = reader.GetOrdinal("LineTotal");

            while (await reader.ReadAsync())
            {
                var row = new BillSummaryRow
                {
                    BillID = reader.IsDBNull(ordBillID) ? 0 : reader.GetInt32(ordBillID),
                    Cname = reader.IsDBNull(ordCname) ? string.Empty : reader.GetString(ordCname),
                    DateOfInvoice = reader.IsDBNull(ordDate) ? DateTime.MinValue : reader.GetDateTime(ordDate),
                    LineTotal = reader.IsDBNull(ordTotal) ? 0m : reader.GetDecimal(ordTotal)
                };

                result.Add(row);
            }

            return result;
        }
    }
}
