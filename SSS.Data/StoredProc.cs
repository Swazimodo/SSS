using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Logging;

using SSS.Utilities;

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
        /// <param name="opts">Options that track how logging should be handled</param>
        /// <param name="logger">App logger reference to use</param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled datatable</returns>
        public static DataTableResult GetDataTable(string connectionString, string storedProcName, List<SqlParameter> sqlParams, IStoredProcOpts opts, ILogger logger, bool exceptionOnError = true)
        {
            DataTableResult dt = null;

            try
            {
                using (SqlConnection conn = CreateConnection(connectionString, opts.LogDBMessages, logger))
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

                CheckForErrors(storedProcName, sqlParams, dt, opts, exceptionOnError, logger);
            }
            catch (Exception ex)
            {
                string error = $"Unable to run {storedProcName}, Params: {ParamsToString(sqlParams)}";
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
        /// <param name="opts">Options that track how logging should be handled</param>
        /// <param name="logger">App logger reference to use</param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled datatable</returns>
        public static DataTableResult GetDataTable(SqlTransaction transaction, string storedProcName, List<SqlParameter> sqlParams, IStoredProcOpts opts, ILogger logger, bool exceptionOnError = true)
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

                CheckForErrors(storedProcName, sqlParams, dt, opts, exceptionOnError, logger);
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
        /// <param name="opts">Options that track how logging should be handled</param>
        /// <param name="logger">App logger reference to use</param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled datatable</returns>
        public static DataTableResult GetDataTable(string connectionString, string storedProcName, IStoredProcOpts opts, ILogger logger, bool exceptionOnError = true)
        {
            return GetDataTable(connectionString, storedProcName, null, opts, logger, exceptionOnError);
        }

        /// <summary>
        /// Run a stored proceedure
        /// </summary>
        /// <param name="connectionString">Connection string to the CMAS DB</param>
        /// <param name="storedProcName">Name of Stored Proceedure to run</param>
        /// <param name="sqlParams">Parameters to add to query</param>
        /// <param name="opts">Options that track how logging should be handled</param>
        /// <param name="logger">App logger reference to use</param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled dataset</returns>
        public static DataSetResult GetDataSet(string connectionString, string storedProcName, List<SqlParameter> sqlParams, IStoredProcOpts opts, ILogger logger, bool exceptionOnError = true)
        {
            DataSetResult ds = null;

            try
            {
                using (SqlConnection conn = CreateConnection(connectionString, opts.LogDBMessages, logger))
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

                CheckForErrors(storedProcName, sqlParams, ds, opts, exceptionOnError, logger);
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
        /// <param name="opts">Options that track how logging should be handled</param>
        /// <param name="logger">App logger reference to use</param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled dataset</returns>
        public static DataSetResult GetDataSet(SqlTransaction transaction, string storedProcName, List<SqlParameter> sqlParams, IStoredProcOpts opts, ILogger logger, bool exceptionOnError = true)
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

                CheckForErrors(storedProcName, sqlParams, ds, opts, exceptionOnError, logger);
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
        /// <param name="opts">Options that track how logging should be handled</param>
        /// <param name="logger">App logger reference to use</param>
        /// <param name="exceptionOnError">should an exception be throw on negative return code</param>
        /// <returns>Filled dataset</returns>
        public static DataSetResult GetDataSet(string connectionString, string storedProcName, IStoredProcOpts opts, ILogger logger, bool exceptionOnError = true)
        {
            return GetDataSet(connectionString, storedProcName, null, opts, logger, exceptionOnError);
        }

        //TODO: possibly add a generic to return a casted type
        public static string GetValue(string connectionString, string storedProcName, List<SqlParameter> sqlParams, IStoredProcOpts opts, ILogger logger, bool exceptionOnError = true)
        {
            DataTableResult dt = null;

            try
            {
                using (SqlConnection conn = CreateConnection(connectionString, opts.LogDBMessages, logger))
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

                CheckForErrors(storedProcName, sqlParams, dt, opts, exceptionOnError, logger);

                //allow exception to be thrown from missing null check
                return dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                string error = string.Format("Unable to run {0}, Params: {1}", storedProcName, ParamsToString(sqlParams));
                DataAccessException e = new DataAccessException(error, ex);
                throw e;
            }
        }

        #endregion

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

        #region private methods

        /// <summary>
        /// takes the parameters for a stored proc and converts it into a string for logging perposes
        /// </summary>
        /// <param name="list">List of parameters</param>
        /// <returns>strified parameter list</returns>
        static string ParamsToString(List<SqlParameter> list)
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

        /// <summary>
        /// Checks DataTableResult for any errors
        /// </summary>
        static void CheckForErrors(string storedProcName, List<SqlParameter> sqlParams, DataTableResult dt, IStoredProcOpts opts, bool exceptionOnError, ILogger logger)
        {
            //check for db error
            if (exceptionOnError && dt.Result < 0)
                throw new StoredProcedureException("Stored procedure returned an error code:" + dt.Result.ToString(), dt.Result);

            if (opts.MaxDBRowsException != null && dt.Rows.Count >= opts.MaxDBRowsException)
            {
                string message = $"Stored proceedure {storedProcName} has exceeded the MaxDBRowsException threshold value. Params: {ParamsToString(sqlParams)}";
                logger.LogCritical(message);
                throw new StoredProcedureException(message, dt.Result);
            }

            if (opts.LogWarningMaxDBRows != null && dt.Rows.Count >= opts.LogWarningMaxDBRows)
                logger.LogWarning($"Stored proceedure {storedProcName} has exceeded the LogWarningMaxDBRows threshold value. Params: {ParamsToString(sqlParams)}");
        }

        /// <summary>
        /// Checks all datatables in a DataSetResult for any errors
        /// </summary>
        static void CheckForErrors(string storedProcName, List<SqlParameter> sqlParams, DataSetResult ds, IStoredProcOpts opts, bool exceptionOnError, ILogger logger)
        {
            //check for db error
            if (exceptionOnError && ds.Result < 0)
                throw new StoredProcedureException("Stored procedure returned an error code:" + ds.Result.ToString(), ds.Result);

            foreach (DataTable dt in ds.Tables)
            {
                if (opts.MaxDBRowsException != null && dt.Rows.Count >= opts.MaxDBRowsException)
                {
                    string message = $"Stored proceedure {storedProcName} has exceeded the MaxDBRowsException threshold value. Params: {ParamsToString(sqlParams)}";
                    logger.LogCritical(message);
                    throw new StoredProcedureException(message, ds.Result);
                }

                if (opts.LogWarningMaxDBRows != null && dt.Rows.Count >= opts.LogWarningMaxDBRows)
                    logger.LogWarning($"Stored proceedure {storedProcName} has exceeded the LogWarningMaxDBRows threshold value. Params: {ParamsToString(sqlParams)}");
            }
        }

        #endregion
    }
}
