using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Logging;

using SSS.Utilities.Exceptions;

namespace SSS.Data
{
    public static class StoredProcs
    {
        #region Execute Stored Proc

        /// <summary>
        /// Run a stored proceedure
        /// </summary>
        /// <param name="connectionString">Connection string to the CMAS DB</param>
        /// <param name="storedProcName">Name of Stored Proceedure to run</param>
        /// <param name="sqlParams">Parameters to add to query</param>
        /// <param name="logDBMessages">Whether or not we should attached a DB listener</param>
        /// <param name="logger"></param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled datatable</returns>
        public static DataTableResult GetDataTable(string connectionString, string storedProcName, List<SqlParameter> sqlParams, bool logDBMessages, ILogger logger, bool exceptionOnError = true)
        {
            DataTableResult dt = null;

            try
            {
                using (SqlConnection conn = CreateConnection(connectionString, logDBMessages, logger))
                using (SqlCommand cmd = new SqlCommand(storedProcName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (sqlParams != null && sqlParams.Count > 0)
                        cmd.Parameters.AddRange(sqlParams.ToArray());

                    //Add the return value
                    SqlParameter returnParameter = new SqlParameter("@return_value", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnParameter);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        dt = new DataTableResult();
                        adapter.Fill(dt);
                        dt.Result = (int)cmd.Parameters["@return_value"].Value;
                    }

                    //prevent 'The SqlParameter is already contained by another SqlParameterCollection' exception when params are reused
                    cmd.Parameters.Clear();
                }

                //check for db error
                if (dt.Result < 0)
                    throw new StoredProcedureException("Stored procedure returned an error code:" + dt.Result.ToString(), dt.Result);
            }
            catch (Exception ex)
            {
                string error = string.Format("Unable to run {0}, Params: {1}", storedProcName, ParamsToString(sqlParams));
                DataAccessException e = new DataAccessException(error, ex);
                //logger.LogWarning(new EventId(), e, error);
                throw e;
            }

            return dt;
        }

        /// <summary>
        /// Run a stored proceedure
        /// </summary>
        /// <param name="transaction">transaction to use</param>
        /// <param name="storedProcName">Name of Stored Proceedure to run</param>
        /// <param name="sqlParams">Parameters to add to query</param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled datatable</returns>
        public static DataTableResult GetDataTable(SqlTransaction transaction, string storedProcName, List<SqlParameter> sqlParams, bool exceptionOnError = true)
        {
            DataTableResult dt = null;

            try
            {
                SqlConnection conn = transaction.Connection;
                using (SqlCommand cmd = new SqlCommand(storedProcName, conn, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (sqlParams != null && sqlParams.Count > 0)
                        cmd.Parameters.AddRange(sqlParams.ToArray());

                    //Add the return value
                    SqlParameter returnParameter = new SqlParameter("@return_value", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnParameter);

                    //verify we have an open connection or the query will fail
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        dt = new DataTableResult();
                        adapter.Fill(dt);
                        dt.Result = (int)cmd.Parameters["@return_value"].Value;
                    }

                    //prevent 'The SqlParameter is already contained by another SqlParameterCollection' exception when params are reused
                    cmd.Parameters.Clear();
                }

                //check for db error
                if (dt.Result < 0)
                    throw new StoredProcedureException("Stored procedure returned an error code:" + dt.Result.ToString(), dt.Result);
            }
            catch (Exception ex)
            {
                string error = string.Format("Unable to run {0}, Params:: {1}", storedProcName, ParamsToString(sqlParams));
                DataAccessException e = new DataAccessException(error, ex);
                //logger.LogWarning(new EventId(), e, error);
                throw e;
            }

            return dt;
        }

        /// <summary>
        /// Run a stored proceedure
        /// </summary>
        /// <param name="connectionString">Connection string to the CMAS DB</param>
        /// <param name="storedProcName">Name of Stored Proceedure to run</param>
        /// <param name="logDBMessages">Whether or not we should attached a DB listener</param>
        /// <param name="logger"></param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled datatable</returns>
        public static DataTableResult GetDataTable(string connectionString, string storedProcName, bool logDBMessages, ILogger logger, bool exceptionOnError = true)
        {
            return GetDataTable(connectionString, storedProcName, null, logDBMessages, logger, exceptionOnError);
        }

        /// <summary>
        /// Run a stored proceedure
        /// </summary>
        /// <param name="connectionString">Connection string to the CMAS DB</param>
        /// <param name="storedProcName">Name of Stored Proceedure to run</param>
        /// <param name="sqlParams">Parameters to add to query</param>
        /// <param name="logDBMessages">Whether or not we should attached a DB listener</param>
        /// <param name="logger"></param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled dataset</returns>
        public static DataSetResult GetDataSet(string connectionString, string storedProcName, List<SqlParameter> sqlParams, bool logDBMessages, ILogger logger, bool exceptionOnError = true)
        {
            DataSetResult ds = null;

            try
            {
                using (SqlConnection conn = CreateConnection(connectionString, logDBMessages, logger))
                using (SqlCommand cmd = new SqlCommand(storedProcName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (sqlParams != null && sqlParams.Count > 0)
                        cmd.Parameters.AddRange(sqlParams.ToArray());

                    //Add the return value
                    SqlParameter returnParameter = new SqlParameter("@return_value", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnParameter);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        ds = new DataSetResult();
                        adapter.Fill(ds);
                        ds.Result = (int)cmd.Parameters["@return_value"].Value;

                        if (ds.Tables.Count > 1)
                            DataBind.SetDataTableNames(ds);
                        else
                            throw new DataBindException("Incorrect number of datatables returned");
                    }

                    //prevent 'The SqlParameter is already contained by another SqlParameterCollection' exception when params are reused
                    cmd.Parameters.Clear();
                }

                //check for db error
                if (ds.Result < 0)
                    throw new StoredProcedureException("Stored procedure returned an error code:" + ds.Result.ToString(), ds.Result);
            }
            catch (Exception ex)
            {
                string error = string.Format("Unable to run {0}, Params:: {1}", storedProcName, ParamsToString(sqlParams));
                DataAccessException e = new DataAccessException(error, ex);
                //logger.LogCritical(new EventId(), e, error);
                throw e;
            }

            return ds;
        }

        /// <summary>
        /// Run a stored proceedure
        /// </summary>
        /// <param name="transaction">transaction to use</param>
        /// <param name="storedProcName">Name of Stored Proceedure to run</param>
        /// <param name="sqlParams">Parameters to add to query</param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled dataset</returns>
        public static DataSetResult GetDataSet(SqlTransaction transaction, string storedProcName, List<SqlParameter> sqlParams, bool exceptionOnError = true)
        {
            DataSetResult ds = null;

            try
            {
                SqlConnection conn = transaction.Connection;
                using (SqlCommand cmd = new SqlCommand(storedProcName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (sqlParams != null && sqlParams.Count > 0)
                        cmd.Parameters.AddRange(sqlParams.ToArray());

                    //Add the return value
                    SqlParameter returnParameter = new SqlParameter("@return_value", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnParameter);

                    //verify we have an open connection or the query will fail
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        ds = new DataSetResult();
                        adapter.Fill(ds);
                        ds.Result = (int)cmd.Parameters["@return_value"].Value;

                        if (ds.Tables.Count > 1)
                            DataBind.SetDataTableNames(ds);
                        else
                            throw new DataBindException("Incorrect number of datatables returned");
                    }

                    //prevent 'The SqlParameter is already contained by another SqlParameterCollection' exception when params are reused
                    cmd.Parameters.Clear();
                }

                //check for db error
                if (ds.Result < 0)
                    throw new StoredProcedureException("Stored procedure returned an error code:" + ds.Result.ToString(), ds.Result);
            }
            catch (Exception ex)
            {
                string error = string.Format("Unable to run {0}, Params:: {1}", storedProcName, ParamsToString(sqlParams));
                DataAccessException e = new DataAccessException(error, ex);
                //logger.LogCritical(new EventId(), e, error);
                throw e;
            }

            return ds;
        }

        /// <summary>
        /// Run a stored proceedure
        /// </summary>
        /// <param name="connectionString">Connection string to the CMAS DB</param>
        /// <param name="storedProcName">Name of Stored Proceedure to run</param>
        /// <param name="logDBMessages">Whether or not we should attached a DB listener</param>
        /// <param name="logger"></param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled dataset</returns>
        public static DataSetResult GetDataSet(string connectionString, string storedProcName, bool logDBMessages, ILogger logger, bool exceptionOnError = true)
        {
            return GetDataSet(connectionString, storedProcName, null, logDBMessages, logger, exceptionOnError);
        }

        #endregion

        public static string ParamsToString(List<SqlParameter> list)
        {
            if (list == null || list.Count == 0)
                return "[null]";

            StringBuilder sb = new StringBuilder();
            foreach (SqlParameter param in list)
                sb.AppendFormat("{0}: {1}, ", param.ParameterName, param.Value);

            //trim off extra ", " at the end
            string s = sb.ToString();
            if (s.Length > 3)
                s = s.Substring(0, s.Length - 2);

            return s;
        }

        public static SqlConnection CreateConnection(string connectionString, bool logMessages, ILogger logger)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            if (logMessages)
            {
                DatabaseLogger dbLogger = new DatabaseLogger(logger);
                conn.InfoMessage += new SqlInfoMessageEventHandler(dbLogger.LogDBMessages);
            }
            conn.Open();
            return conn;
        }

        public static string GetValue(string connectionString, string storedProcName, List<SqlParameter> sqlParams, bool logDBMessages, ILogger logger, bool exceptionOnError = true)
        {
            DataTableResult dt = null;

            try
            {
                using (SqlConnection conn = CreateConnection(connectionString, logDBMessages, logger))
                using (SqlCommand cmd = new SqlCommand(storedProcName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (sqlParams != null && sqlParams.Count > 0)
                        cmd.Parameters.AddRange(sqlParams.ToArray());

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        dt = new DataTableResult();
                        adapter.Fill(dt);
                    }

                    //prevent 'The SqlParameter is already contained by another SqlParameterCollection' exception when params are reused
                    cmd.Parameters.Clear();
                }

                return dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                string error = string.Format("Unable to run {0}, Params: {1}", storedProcName, ParamsToString(sqlParams));
                DataAccessException e = new DataAccessException(error, ex);
                //logger.LogWarning(new EventId(), e, error);
                throw e;
            }

        }
    }
}
