using Microsoft.Extensions.Configuration;
using Smart_retail_manager_website.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    LineTotal = reader.IsDBNull(ordTotal) ? 0m : Convert.ToDecimal(reader.GetDouble(ordTotal))

                };

                result.Add(row);
            }

            return result;
        }

        //
        public async Task<int> InsertCustomerAsync(Customer c)
        {
            var sql = @"INSERT INTO Customer (Name, Email, Phone)
                VALUES (@Name, @Email, @Phone);
                SELECT SCOPE_IDENTITY();";

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Name", c.Name);
            cmd.Parameters.AddWithValue("@Email", c.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", c.Phone ?? (object)DBNull.Value);

            await conn.OpenAsync();
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<int> InsertBillAsync(int customerId, DateTime date, double TaxRate)
        {
            var sql = @"INSERT INTO Bill (CustomerID, Date,TaxRate)
                VALUES (@CID, @Date, @TaxRate);
                SELECT SCOPE_IDENTITY();";

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@CID", customerId);
            cmd.Parameters.AddWithValue("@Date", date);
            cmd.Parameters.AddWithValue("@TaxRate", TaxRate);

            await conn.OpenAsync();
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task InsertBillItemAsync(int billId, int productId, decimal price, int qty)
        {
            var sql = @"INSERT INTO Bill_Products (BillID, ProductID, Price)
                VALUES (@BID, @PID, @Price)";

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@BID", billId);
            cmd.Parameters.AddWithValue("@PID", productId);
            cmd.Parameters.AddWithValue("@Price", price);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Bill>> GetAllBillsAsync()
        {
            var list = new List<Bill>();
            var connString = _config.GetConnectionString("DefaultConnection");

            const string sql = @"
        SELECT b.BillID, b.Date, c.CustomerID, c.Name
        FROM Bill b
        JOIN Customer c ON b.CustomerID = c.CustomerID;
    ";

            using var conn = new SqlConnection(connString);
            using var cmd = new SqlCommand(sql, conn);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Bill
                {
                    BillID = reader.GetInt32(0),
                    Date = reader.GetDateTime(1),
                    Customer = new Customer
                    {
                        CustomerID = reader.GetInt32(2),
                        Name = reader.GetString(3)
                    }
                });
            }

            return list;
        }

        public async Task<BillDetailsDTO> GetBillDetailsAsync(int billId)
        {
            var result = new BillDetailsDTO();
            var connString = _config.GetConnectionString("DefaultConnection");

            // 1) Get bill header
            const string sqlHeader = @"
        SELECT b.BillID, b.Date, c.CustomerID, c.Name
        FROM Bill b
        JOIN Customer c ON b.CustomerID = c.CustomerID
        WHERE b.BillID = @id;
    ";

            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(sqlHeader, conn))
            {
                cmd.Parameters.AddWithValue("@id", billId);
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    result.BillID = reader.GetInt32(0);
                    result.Date = reader.GetDateTime(1);
                    result.CustomerName = reader.GetString(3);
                }
            }

            // 2) Get bill items
            const string sqlItems = @"
        SELECT p.Name, p.UnitPrice
        FROM Bill_Products bi
        JOIN Product p ON bi.ProductID = p.ProductID
        WHERE bi.BillID = @id;
    ";

            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(sqlItems, conn))
            {
                cmd.Parameters.AddWithValue("@id", billId);
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Items.Add(new BillItemRow
                    {
                        Name = reader.GetString(0),
                        UnitPrice = reader.GetDecimal(1)
                    });
                }
            }

            return result; // return your DTO directly
        }


    }
}
