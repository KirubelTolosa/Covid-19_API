using Covid_API.DAL.DADto;
using CovidPipeline.Covid_API.BAL;
using CovidPipeline.Covid_API.DAL;
using CovidPipeline.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;

namespace CovidPipeline.DAL
{
    public class CovidDataRepository : ICovidDataRepository
    {        
        public void InsertLocations(List<CovidLocationBLDto> bALRecords)
        {
            List<CovidLocationDADto> dALRecords = Utils.Utilities.MapLocationsBLDTOtoDADTO(bALRecords);
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_MainDB"].ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("insert into LocationsGlobal(Country, State, Lat, Long) values (@Country, @State, @Lat, @Long)");
                SqlParameter Country = new SqlParameter("Country", System.Data.SqlDbType.NVarChar);
                SqlParameter State = new SqlParameter("State", System.Data.SqlDbType.NVarChar);
                SqlParameter Lat = new SqlParameter("Lat", System.Data.SqlDbType.Float);
                SqlParameter Long = new SqlParameter("Long", System.Data.SqlDbType.Float);
                command.Parameters.Add(Country);
                command.Parameters.Add(State);
                command.Parameters.Add(Lat);
                command.Parameters.Add(Long);                
                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    foreach (var record in dALRecords)
                    {
                        Country.Value = record.Country;
                        State.Value = record.State;
                        Lat.Value = record.Lat;
                        Long.Value = record.Long;
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }

            }
        }      
        public void InsertNewLocations(string dbCompositeKey)
        {            
            List<CovidLocationDADto> locationRecords = new List<CovidLocationDADto>();
            string[] keys = dbCompositeKey.Split('_');
            CovidLocationDADto locationRecord = new CovidLocationDADto
            {
                State = keys[0],
                Country = keys[1],
                Lat = Convert.ToDouble(keys[2]),
                Long = Convert.ToDouble(keys[3])
            };
            locationRecords.Add(locationRecord);
            InsertLocations(Utils.Utilities.MapLocationsDADTOtoBLDTO(locationRecords));
        }
        public void InsertCases(List<CovidCaseCountBLDto> bALRecords, Metrics metrics)
        {
            List<CovidCaseCountDADto> dALRecords = Utils.Utilities.MapCaseCountsBLDTOtoDADTO(bALRecords);
            Dictionary<string, int> locationTableCompositeKeys = GetLocationTableCompositeKeys();
            string[] keys;
            double lat;
            double Long; 
            foreach (var rec in dALRecords)
            {
                keys = rec.dbCompositeKey.Split('_');
                lat = Convert.ToDouble(keys[2]);
                Long = Convert.ToDouble(keys[3]);                
                if (keys[2].Split('.')[1] == "0")
                {
                    keys[2] = keys[2].Split('.')[0];
                }
                else
                {
                    keys[2] = lat.ToString();
                }
                if (keys[3].Split('.')[1] == "0")
                {
                    keys[3] = keys[3].Split('.')[0];
                }
                else
                {
                    keys[3] = Long.ToString();
                }
                rec.dbCompositeKey = (keys[0] + '_' + keys[1] + '_' + keys[2] + '_' + keys[3]);                
                if (!locationTableCompositeKeys.ContainsKey(rec.dbCompositeKey))
                {                    
                    InsertNewLocations(rec.dbCompositeKey);
                    locationTableCompositeKeys = GetLocationTableCompositeKeys();
                }
                
            }            
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_MainDB"].ConnectionString))
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand();
                if (metrics == Metrics.CONFIRMED_CASES)
                {
                    sqlCommand.CommandText = "insert into ConfirmedCases(Date, ConfirmedCasesCount, Id) values(@Date, @Count, @id)";
                }
                else if (metrics == Metrics.DEATHS)
                {
                    sqlCommand.CommandText = "insert into Deaths(Date, DeathCount, Id) values(@Date, @Count, @id)";
                }
                else if (metrics == Metrics.RECOVERIES)
                {
                    sqlCommand.CommandText = "insert into Recoveries(Date, RecoveriesCount, Id) values(@Date, @Count, @id)";
                }

                SqlParameter Date = new SqlParameter("Date", System.Data.SqlDbType.DateTime);
                SqlParameter Id = new SqlParameter("Id", System.Data.SqlDbType.Int);
                SqlParameter Count = new SqlParameter("Count", System.Data.SqlDbType.Int);

                sqlCommand.Parameters.Add(Date);
                sqlCommand.Parameters.Add(Id);
                sqlCommand.Parameters.Add(Count);
                SqlTransaction transaction = connection.BeginTransaction("InsertCases");
                sqlCommand.Connection = connection;
                sqlCommand.Transaction = transaction;
                try
                {
                    foreach (var record in dALRecords)
                    {
                        Date.Value = record.Date;                        
                        Count.Value = record.Count;
                        Id.Value = locationTableCompositeKeys[record.dbCompositeKey];
                        sqlCommand.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }
            }
        }     
        public Dictionary<string, int> GetLocationTableCompositeKeys()
        {
            Dictionary<string, int> locationTableCompositeKeys = new Dictionary<string, int>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_MainDB"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("SELECT Id, [State], Country, Lat, Long  FROM LocationsGlobal", connection);          
            connection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            string state = "Null";
            string country = "Null";
            double Lat = 0;
            double Long = 0;
            try
            {
                while (reader.Read())
                {                    
                    if ((string)reader["State"] != "")
                    {
                        state = (string)reader["State"];
                    }                       
                    if ((string)reader["Country"] != "")
                    {
                        country = (string)reader["Country"];
                    }
                    if ((reader["Lat"]) != null)
                    {
                        Lat = Convert.ToDouble(reader["Lat"]);
                    }
                    if (reader["Long"] != null)
                    {
                        Long = Convert.ToDouble(reader["Long"]);
                    }
                    locationTableCompositeKeys.Add((state + '_' + country + '_' + Lat + '_' + Long), (int)reader["Id"]);
                    state = "Null";
                    country = "Null";
                    Lat = 0;
                    Long = 0;
                }                
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return locationTableCompositeKeys;
        }
        public DateTime GetLastUpdateDate(Metrics metrics)
        {
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_MainDB"].ConnectionString);
            SqlCommand sqlCommandConfirmedCases = new SqlCommand("SELECT MAX ([Date]) FROM ConfirmedCases Where ID = 1", sqlConnection);
            SqlCommand sqlCommandDeaths = new SqlCommand("SELECT MAX ([Date]) FROM Deaths Where ID = 1", sqlConnection);
            SqlCommand sqlCommandRecoveries = new SqlCommand("SELECT MAX ([Date]) FROM Recoveries Where ID = 1", sqlConnection);
            DateTime LastUpdateDate = new DateTime();
            try
            {
                sqlConnection.Open();
                if (metrics == Metrics.CONFIRMED_CASES)
                {
                    LastUpdateDate = (DateTime)sqlCommandConfirmedCases.ExecuteScalar();
                }
                else if (metrics == Metrics.DEATHS)
                {
                    LastUpdateDate = (DateTime)sqlCommandDeaths.ExecuteScalar();
                }
                else if (metrics == Metrics.RECOVERIES)
                {
                    LastUpdateDate = (DateTime)sqlCommandRecoveries.ExecuteScalar();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return LastUpdateDate;
        }
        public List<NationalCasesDADto> GetCountOfCasesForAllNations(Metrics metrics)
        {
            List<NationalCasesDADto> covidCasesDALRecords = new List<NationalCasesDADto>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_MainDB"].ConnectionString); 
            SqlCommand sqlCommand = new SqlCommand("select trim(LocationsGlobal.Country) AS 'Country', Max(ConfirmedCases.ConfirmedCasesCount) AS 'Count' from LocationsGlobal INNER JOIN  ConfirmedCases ON  ConfirmedCases.Id = LocationsGlobal.Id group by LocationsGlobal.Country", connection);
            if (metrics == Metrics.DEATHS)
            {
                sqlCommand = new SqlCommand("select trim(LocationsGlobal.Country) as Country, Max(Deaths.DeathCount) AS Count from LocationsGlobal INNER JOIN  Deaths ON  Deaths.Id = LocationsGlobal.Id group by LocationsGlobal.Country", connection);
            }
            else if (metrics == Metrics.RECOVERIES)
            {
                sqlCommand = new SqlCommand("select trim(LocationsGlobal.Country)as Country, Max(Recoveries.RecoveriesCount) AS 'Count' from LocationsGlobal INNER JOIN  Recoveries ON  Recoveries.Id = LocationsGlobal.Id group by LocationsGlobal.Country", connection);
            }
            connection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            try
            {
                while(reader.Read())
                {
                    NationalCasesDADto CaseCountDto = new NationalCasesDADto
                    {
                       Country = (string)reader["Country"],
                       Count = (Int32)reader["Count"],
                    };
                    covidCasesDALRecords.Add(CaseCountDto);
                }                   

            }
            catch (SqlException ex)
            {
                Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return covidCasesDALRecords;
        }
        public List<NationalCasesDADto> GetCountOfCasesByCountry(Metrics metrics, string Country, DateTime? Date)
        {
            List<NationalCasesDADto> covidCasesDALRecords = new List<NationalCasesDADto>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_MainDB"].ConnectionString);
            
            SqlCommand sqlCommand = new SqlCommand(@"select trim(l.Country) AS 'Country', Max(c.ConfirmedCasesCount) AS 'Count' 
                                                        from LocationsGlobal l INNER JOIN  ConfirmedCases c ON  c.Id = l.Id 
                                                        WHERE l.Country = @ctry 
                                                        group by l.Country", connection);
            if(Date != DateTime.MinValue)
            {
                sqlCommand = new SqlCommand(@"select trim(l.Country) AS 'Country', Max(c.ConfirmedCasesCount) AS 'Count' 
                                                        from LocationsGlobal l INNER JOIN  ConfirmedCases c ON  c.Id = l.Id 
                                                        WHERE Country = @ctry  and c.[Date] = @dte  
                                                        group by l.Country", connection);
            }
            if (metrics == Metrics.DEATHS)
            {
                if(Date == DateTime.MinValue)
                {
                    sqlCommand = new SqlCommand(@"select trim(l.Country) as Country, Max(d.DeathCount) AS Count 
                                                        from LocationsGlobal l INNER JOIN  Deaths d ON  d.Id = l.Id 
                                                        WHERE Country = @ctry 
                                                        group by l.Country", connection);
                }
                else
                {
                    sqlCommand = new SqlCommand(@"select trim(l.Country) as Country, Max(d.DeathCount) AS Count 
                                                        from LocationsGlobal l INNER JOIN  Deaths d ON  d.Id = l.Id 
                                                        WHERE l.Country = @ctry and d.[Date] = @dte 
                                                        group by l.Country", connection);
                }                
            }
            else if (metrics == Metrics.RECOVERIES)
            {
                if (Date == DateTime.MinValue)
                {
                    sqlCommand = new SqlCommand(@"select trim(l.Country)as Country, Max(r.RecoveriesCount) AS 'Count' 
                                                    from LocationsGlobal l INNER JOIN  Recoveries r ON  r.Id = l.Id 
                                                    WHERE l.Country = @ctry  
                                                    group by l.Country", connection);
                }
                else
                {
                    sqlCommand = new SqlCommand(@"select trim(l.Country)as Country, Max(r.RecoveriesCount) AS 'Count' 
                                                    from LocationsGlobal l INNER JOIN  Recoveries r ON  r.Id = l.Id 
                                                    WHERE l.Country = @ctry and r.[Date] = @dte  
                                                    group by l.Country", connection);
                }
                
            }
            SqlParameter ctry = new SqlParameter("ctry", System.Data.SqlDbType.NVarChar);
            ctry.Value = Country;
            sqlCommand.Parameters.Add(ctry);
            if(Date != DateTime.MinValue)
            {
                SqlParameter dte = new SqlParameter("dte", System.Data.SqlDbType.NVarChar);
                dte.Value = Date;
                sqlCommand.Parameters.Add(dte);
            }
                
            connection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    NationalCasesDADto CaseCountDto = new NationalCasesDADto
                    {
                        Country = (string)reader["Country"],
                        Count = (Int32)reader["Count"],
                    };
                    covidCasesDALRecords.Add(CaseCountDto);
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return covidCasesDALRecords;
        }
        public List<GlobalTotalCountsDADto> GetGlobalTotalCounts(Metrics metrics)
        {
            List<GlobalTotalCountsDADto> globalTotalCases = new List<GlobalTotalCountsDADto>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_MainDB"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("select sum(DISTINCT ConfirmedCases.ConfirmedCasesCount) AS 'Count' FROM LocationsGlobal INNER JOIN  ConfirmedCases ON  ConfirmedCases.Id = LocationsGlobal.Id WHERE ConfirmedCases.[Date] = (select MAX([Date]) FROM ConfirmedCases)", connection);
            connection.Open();
            if (metrics == Metrics.DEATHS)
            {
                sqlCommand = new SqlCommand("select sum(DISTINCT Deaths.DeathCount) AS 'Count' FROM LocationsGlobal INNER JOIN  Deaths ON  LocationsGlobal.Id = Deaths.Id WHERE Deaths.[Date] = (select MAX([Date]) FROM ConfirmedCases)", connection);
            }
            else if (metrics == Metrics.RECOVERIES)
            {
                sqlCommand = new SqlCommand("select sum(DISTINCT Recoveries.RecoveriesCount) AS 'Count' FROM LocationsGlobal INNER JOIN  Recoveries ON  Recoveries.Id = LocationsGlobal.Id WHERE Recoveries.[Date] = (select MAX([Date]) FROM ConfirmedCases) ", connection);
            }
            
            SqlDataReader reader = sqlCommand.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    GlobalTotalCountsDADto CaseCountDto = new GlobalTotalCountsDADto
                    {                       
                        Count = (int)reader["Count"]
                    };
                    globalTotalCases.Add(CaseCountDto);
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return globalTotalCases;
        }
        public List<DailyCaseCountsDADto> GetDailyCaseCountsByCountry(Metrics metrics, string Country)
        {
            List<DailyCaseCountsDADto> DailyCaseCounts = new List<DailyCaseCountsDADto>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_MainDB"].ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("SELECT ConfirmedCases.[Date], ConfirmedCases.ConfirmedCasesCount As Count FROM ConfirmedCases INNER JOIN LocationsGlobal ON LocationsGlobal.Id = ConfirmedCases.Id WHERE LocationsGlobal.Country = @ctry", connection);
            connection.Open();
            if (metrics == Metrics.DEATHS)
            {
                sqlCommand = new SqlCommand("SELECT Deaths.[Date], Deaths.DeathCount As Count FROM Deaths INNER JOIN LocationsGlobal ON LocationsGlobal.Id = Deaths.Id WHERE LocationsGlobal.Country = @ctry", connection);
            }
            else if (metrics == Metrics.RECOVERIES)
            {
                sqlCommand = new SqlCommand("SELECT Recoveries.[Date], Recoveries.RecoveriesCount As Count FROM Recoveries INNER JOIN LocationsGlobal ON LocationsGlobal.Id = Recoveries.Id WHERE LocationsGlobal.Country = @ctry", connection);
            }
            SqlParameter ctry = new SqlParameter("ctry", System.Data.SqlDbType.NVarChar);
            ctry.Value = Country;
            sqlCommand.Parameters.Add(ctry);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    DailyCaseCountsDADto DailyCaseCountsDto = new DailyCaseCountsDADto
                    {
                        Date = (DateTime)reader["Date"],
                        Count = (int)reader["Count"]
                    };
                    DailyCaseCounts.Add(DailyCaseCountsDto);
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return DailyCaseCounts;
        }
    }

}




