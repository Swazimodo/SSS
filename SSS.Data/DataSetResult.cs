using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;

using SSS.Exceptions;
using SSS.Interfaces;

namespace SSS.Data
{
    public static class DataBind
    {
        const string TABLE_NAME = "DataTableName";
        const string TABLE_INDEX = "Index";
        const string INFO_TABLE_NAME = "DataSetInfo";
        const string ROW_INDEX_COLUMN = "RowNumber";

        /// <summary>
        /// Uses the DataSetInfo table to name all tables returned
        /// </summary>
        /// <param name="ds">Dataset with multiple tables, the first table must be the DataSetInfo table</param>
        public static void SetDataTableNames(DataSet ds)
        {
            //for this to function we need multiple tables with the first table being the DataSet ID
            ////EXAMPLE return value:
            ////Index DataTableName
            ////0     DataSetInfo
            ////1     ContractInfo
            ////2     InvoiceInfo
            if (ds.Tables.Count < 2) throw new DataBindException("Invalide DataSet format - incorrect number of return tables");
            if (ds.Tables[0].Columns.Count != 2) throw new DataBindException("Invalide DataSet format - incorrect number of columns");
            ds.Tables[0].TableName = TABLE_NAME;

            DataTable setIds = ds.Tables[0];

            try
            {
                foreach (DataRow table in setIds.Rows)
                {
                    //skip the first ID table
                    if (table[TABLE_NAME].ToString() == INFO_TABLE_NAME) continue;

                    int index = (int)table[TABLE_INDEX];
                    string name = table[TABLE_NAME].ToString();
                    ds.Tables[index].TableName = name;
                }
            }
            catch (Exception ex)
            {
                throw new DataBindException("Unable to set datatable names", ex);
            }

            //remove the DataSetInfo table 
            ds.Tables.RemoveAt(0);
        }

        /// <summary>
        /// Extension Method to bind DataSet to model properties
        /// </summary>
        /// <typeparam name="T">Type of model</typeparam>
        /// <param name="dt">DataTable with one row</param>
        /// <param name="obj">object to bind to</param>
        public static T ToModel<T>(DataTable dt)
        {
            T obj;

            if (dt.Rows.Count == 0) return default(T);
            if (dt.Rows.Count > 1) throw new DataException("There were multiple rows returned when only one was expected");
            DataRow dr = dt.Rows[0];

            //var ob = Activator.CreateInstance<T>();

            List<string> columns = (from DataColumn dc in dt.Columns select dc.ColumnName).ToList();

            //create default if needed
            if (typeof(T).IsValueType)
            {
                //bind to struct
                obj = default(T);

                //sets fields values that match a column in the DataTable
                FieldInfo[] fields = typeof(T).GetFields();
                foreach (var fieldInfo in fields.Where(fieldInfo => columns.Contains(fieldInfo.Name)))
                    fieldInfo.SetValue(obj, !dr.IsNull(fieldInfo.Name) ? dr[fieldInfo.Name] : fieldInfo.FieldType.IsValueType ? Activator.CreateInstance(fieldInfo.FieldType) : null);
            }
            else
            {
                //bind to class
                obj = (T)Activator.CreateInstance(typeof(T));

                //sets property values that match a column in the DataTable
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (var propertyInfo in properties.Where(propertyInfo => columns.Contains(propertyInfo.Name)))
                    propertyInfo.SetValue(obj, !dr.IsNull(propertyInfo.Name) ? dr[propertyInfo.Name] : propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null);
            }

            return obj;
        }

        /// <summary>
        /// Extension Method to bind DataTable to model properties
        /// </summary>
        /// <typeparam name="T">Type of model</typeparam>
        public static List<T> ToList<T>(DataTable dt)
        {
            List<T> lst = new List<T>();
            if (dt.Rows.Count == 0) return lst;

            //FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<string> columns = (from DataColumn dc in dt.Columns select dc.ColumnName).ToList();

            foreach (DataRow dr in dt.Rows)
            {
                var ob = Activator.CreateInstance<T>();

                ////for performance we will only look at properties
                ////sets field values that match a column in the DataTable
                //foreach (var fieldInfo in fields.Where(fieldInfo => columns.Contains(fieldInfo.Name)))
                //    fieldInfo.SetValue(ob, !dr.IsNull(fieldInfo.Name) ? dr[fieldInfo.Name] : fieldInfo.FieldType.IsValueType ? Activator.CreateInstance(fieldInfo.FieldType) : null);

                //sets property values that match a column in the DataTable
                foreach (var propertyInfo in properties.Where(propertyInfo => columns.Contains(propertyInfo.Name)))
                    propertyInfo.SetValue(ob, !dr.IsNull(propertyInfo.Name) ? dr[propertyInfo.Name] : propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null);
                lst.Add(ob);
            }

            return lst;
        }

        /// <summary>
        /// Extension Method to bind DataTable to model properties
        /// </summary>
        /// <typeparam name="T">Type of model</typeparam>
        public static void ToPagingList<T>(DataTable dt, IPageable<T> table, int pageSize)
            where T : IPageableItem
        {
            //get paging information if records were found
            if (dt.Rows.Count > 0)
            {
                table.NextPageId = GetNextPageId(dt, pageSize);

                //use integer division to get a whole number
                table.CurrentPageNumber = ((int)dt.Rows[0][ROW_INDEX_COLUMN]) / pageSize;
            }

            List<T> lst = new List<T>();
            if (dt.Rows.Count != 0)
            {
                //FieldInfo[] fields = typeof(T).GetFields();
                PropertyInfo[] properties = typeof(T).GetProperties();
                List<string> columns = (from DataColumn dc in dt.Columns select dc.ColumnName).ToList();

                foreach (DataRow dr in dt.Rows)
                {
                    var ob = Activator.CreateInstance<T>();

                    ////for performance we will only look at properties
                    ////sets field values that match a column in the DataTable
                    //foreach (var fieldInfo in fields.Where(fieldInfo => columns.Contains(fieldInfo.Name)))
                    //    fieldInfo.SetValue(ob, !dr.IsNull(fieldInfo.Name) ? dr[fieldInfo.Name] : fieldInfo.FieldType.IsValueType ? Activator.CreateInstance(fieldInfo.FieldType) : null);

                    //sets property values that match a column in the DataTable
                    foreach (var propertyInfo in properties.Where(propertyInfo => columns.Contains(propertyInfo.Name)))
                        propertyInfo.SetValue(ob, !dr.IsNull(propertyInfo.Name) ? dr[propertyInfo.Name] : propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null);
                    lst.Add(ob);
                }
            }

            table.Items = lst;
        }

        /// <summary>
        /// Extension Method to bind DataTable column to list
        /// </summary>
        /// <typeparam name="T">List data type</typeparam>
        public static List<T> ColToList<T>(DataTable dt, string name)
        {
            List<T> lst = new List<T>();
            if (dt.Rows.Count == 0) return lst;

            //FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<string> columns = (from DataColumn dc in dt.Columns select dc.ColumnName).ToList();

            foreach (DataRow dr in dt.Rows)
                lst.Add((T)dr[name]);

            return lst;
        }

        /// <summary>
        /// Gets the ID of the last row if it exceeds the page size,
        /// The extra row is then removed so that the correct page size is returned
        /// </summary>
        /// <param name="IdColName">column ID must be ordered in the result set</param>
        /// <returns></returns>
        static int GetNextPageId(DataTable dt, int pageCount)
        {
            if (dt != null && dt.Rows.Count > pageCount)
            {
                try
                {
                    //get the id of the row one past the page size
                    int id = (int)dt.Rows[pageCount][ROW_INDEX_COLUMN];

                    //delete extra row that is not intended to be displayed on this page
                    dt.Rows.RemoveAt(pageCount);
                    return id;
                }
                catch (Exception ex)
                {
                    throw new DataBindException(string.Format("Unable to lookup column {0} in datatabe {1}", ROW_INDEX_COLUMN, dt.TableName), ex);
                }
            }
            else
            {
                return -1;
            }
        }
    }
}
