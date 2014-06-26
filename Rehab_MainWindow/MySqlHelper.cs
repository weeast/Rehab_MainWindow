using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;

namespace Rehab
{
    public static class MySqlHelper
    {
        #region 分页
        /// <summary>
        /// row_number()分页(MySqlDataReader)
        /// </summary>
        /// <param name="pageSize">取数据条数</param>
        /// <param name="currentPage">当前页</param>
        /// <param name="para">示例 "字段列表", "表名", "条件", "排序"</param>
        /// <returns></returns>
        public static DataTable GetPageList(string connectionString, int currentPage, int pageSize, string[] para, out int recordCount)
        {
            StringBuilder sbSql = new StringBuilder();

            sbSql.AppendFormat("select {0} from {1} ", para[0], para[1]);
            if (!string.IsNullOrEmpty(para[2]))
            {
                sbSql.AppendFormat("where {0} ", para[2]);
            }

            if (!string.IsNullOrEmpty(para[3]))
            {
                sbSql.AppendFormat("order by {0} ", para[3]);
            }

            sbSql.AppendFormat("limit {0},{1}", pageSize * (currentPage - 1), pageSize);

            //计算记录数
            StringBuilder sbCount = new StringBuilder();
            sbCount.AppendFormat("select count(1) from {0} ", para[1]);
            if (!string.IsNullOrEmpty(para[2]))
            {
                sbCount.AppendFormat("where {0} ", para[2]);
            }

            recordCount = Convert.ToInt32(ExecuteScale(connectionString, sbCount.ToString(), null));

            return ExecuteDataTable(connectionString, sbSql.ToString(), null);
            //return ExecuteReader(strSql);
        }
        #endregion

        #region  执行简单SQL语句
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteNonQuery(string connectionString, string sqlString, MySqlParameter[] parameters)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(sqlString, connection))
                {
                    connection.Open();
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteDataSet(string connectionString, string sql, MySqlParameter[] parameters)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();

                connection.Open();

                MySqlCommand cmd = new MySqlCommand(sql, connection);

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                da.Fill(ds);

                return ds;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTable(string connectionString, string sql, MySqlParameter[] parameters)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();

                connection.Open();

                MySqlCommand cmd = new MySqlCommand(sql, connection);

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                da.Fill(dt);

                return dt;
            }
        }

        public static object ExecuteScale(string connectionString, string sql, MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                return cmd.ExecuteScalar();
            }
        }
        #endregion
    }
}
