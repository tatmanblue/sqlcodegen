/*
 ******************************************************************************
 This file is part of BigWoo.

    BigWoo is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    BigWoo is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with BigWoo; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA


    architected and written by 
    matt raffel 
    matt.raffel@gmail.com

       copyright (c) 2010 by matt raffel unless noted otherwise

 ******************************************************************************
*/
#region using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BigWoo.Apps.SqlCodeGen.Interfaces;
using BigWoo.Apps.SqlCodeGen.Classes;

using CLIENT = System.Data.Common;
using EFI = System.Data.EffiProz;
#endregion

namespace EffiprozSchemaInquiry
{
    /// <summary>
    /// Builds column definition data for the tables specified for the 
    /// Effiproz database.  http://www.effiproz.com
    /// </summary>
    public class EffiprozTableInquiry : ITableInquiry
    {
        #region private data
        private CLIENT.DbConnection _connection;
        private string _connectStr = string.Empty;
        #endregion

        #region properties
        public string ConnectionStr
        {
            get { return _connectStr; }
            set { _connectStr = value; }
        }

        public CLIENT.DbConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }
        #endregion

        #region private methods
        private void GetConnection()
        {
            if (null == _connection)
                _connection = new EFI.EfzConnection(_connectStr);

            if (null != _connection)
                _connection.Open();
        }

        private void CloseConnection()
        {
            if (null != _connection)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Checks that the table exists
        /// </summary>
        private void CheckForTable(TableDefinition table)
        {
            using (CLIENT.DbCommand cmd = _connection.CreateCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandText = string.Format("SELECT COUNT(TABLE_NAME) AS TableCount FROM INFORMATION_SCHEMA.SYSTEM_TABLES WHERE TABLE_NAME=@table_name_param AND TABLE_TYPE=@table_type_param");
                CLIENT.DbParameter tableName = cmd.CreateParameter();
                tableName.ParameterName = "@table_name_param";
                // it appears the table names are stored in all uppercase names
                tableName.Value = table.Name.ToUpper();
                cmd.Parameters.Add(tableName);

                CLIENT.DbParameter tableType = cmd.CreateParameter();
                tableType.ParameterName = "@table_type_param";
                tableType.Value = "TABLE";
                cmd.Parameters.Add(tableType);

                CLIENT.DbDataReader reader = cmd.ExecuteReader();

                int count = 0;

                if (true == reader.Read())
                    count = reader.GetInt32(0);

                reader.Close();

                if (1 > count)
                    throw new MissingTableException(string.Format("table '{0}' not present", table.Name));
            }

        }

        /// <summary>
        /// Using the system tables, gets the column information and puts it into an array
        /// </summary>
        private void BuildTableInfo(TableDefinition table)
        {
            const int FIELD_NAME = 0;
            const int FIELD_TYPE_NAME = 1;
            const int FIELD_IS_PRIMARY = 4;

            const string sql = "SELECT "
                             + "     SC.COLUMN_NAME"
                             + ",    SC.TYPE_NAME"
                             + ",    SC.SQL_DATA_TYPE"
                             + ",    SC.ORDINAL_POSITION"
                             + ",    SC.IS_PRIMARY_KEY"
                             + " FROM  INFORMATION_SCHEMA.SYSTEM_COLUMNS SC"
                             + " WHERE "
                             + " SC.TABLE_NAME=@table_name_param "
                             + " ORDER BY SC.ORDINAL_POSITION";

            using (CLIENT.DbCommand cmd = _connection.CreateCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandText = sql;
                CLIENT.DbParameter tableNameParam = cmd.CreateParameter();
                tableNameParam.ParameterName = "@table_name_param";
                // it appears the table names are stored in all uppercase names
                tableNameParam.Value = table.Name.ToUpper();

                cmd.Parameters.Add(tableNameParam);
                CLIENT.DbDataReader reader = cmd.ExecuteReader();

                while (true == reader.Read())
                {
                    ColumnDefinition col = new ColumnDefinition(reader.GetString(FIELD_NAME));
                    string typeName = reader.GetString(FIELD_TYPE_NAME);
                    col.SchemeType = typeName;

                    bool isPrimary = false;
                    if (false == reader.IsDBNull(FIELD_IS_PRIMARY))
                        isPrimary = reader.GetBoolean(FIELD_IS_PRIMARY);

                    col.IsPrimaryKey = isPrimary;

                    table.Columns.Add(col);
                }

                reader.Close();
            }
        }

        #endregion

        #region ctor/init/cleanup
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public EffiprozTableInquiry(CLIENT.DbConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 
        /// </summary>
        public EffiprozTableInquiry() { }
        #endregion

        #region public methods
        /// <summary>
        /// At this point TableDefinitions contain nothing more than a table name.  We need to now query
        /// the database to figure out the table schema and populate that information into the TableDefinition.
        /// 
        /// The information gathered here becomes the source for code generation.
        /// </summary>
        /// <param name="tables">TableDefinitionCollection</param>
        public void ProcessTables(TableDefinitionCollection tables)
        {

            GetConnection();

            try
            {

                foreach (TableDefinition tableInstance in tables)
                {
                    CheckForTable(tableInstance);
                    BuildTableInfo(tableInstance);
                }
            }
            finally
            {
                CloseConnection();
            }
        }
        #endregion
    }
}
