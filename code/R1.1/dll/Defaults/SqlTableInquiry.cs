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
using System.Text;
using CLIENT = System.Data.SqlClient;
using BigWoo.Apps.SqlCodeGen.Classes;
using BigWoo.Apps.SqlCodeGen.Interfaces;
#endregion

namespace BigWoo.Apps.SqlCodeGen.Classes
{
    /// <summary>
    /// This class is responsible for getting the schema of a table and converting
    /// that information into program data which can then be used to generate code
    /// </summary>
    public class SqlTableInquiry : ITableInquiry
    {
        #region private data
        private CLIENT.SqlConnection _connection;
        private string _connectStr = string.Empty;
        #endregion

        #region properties
        public string ConnectionStr
        {
            get { return _connectStr; }
            set { _connectStr = value; }
        }

        public CLIENT.SqlConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }
        #endregion

        #region private methods
        private void GetConnection()
        {
            if (null == _connection)
                _connection = new CLIENT.SqlConnection(_connectStr);

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
            using (CLIENT.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandText = string.Format("SELECT COUNT(name) AS TableCount FROM Sys.Tables WHERE name=@table_name");
                cmd.Parameters.AddWithValue("@table_name", table.Name);
                CLIENT.SqlDataReader reader = cmd.ExecuteReader();

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
            const int FIELD_TYPE_NAME = 3;
            const int FIELD_IS_PRIMARY = 9;

            const string sql = "SELECT c.name AS column_name "
                            + ", c.column_id "
                            + ", SCHEMA_NAME(t.schema_id) AS type_schema "
                            + ", t.name AS type_name "
                            + ", t.is_user_defined "
                            + ", t.is_assembly_type "
                            + ", c.max_length "
                            + ", c.precision "
                            + ", c.scale "
                            + ", (SELECT 1"
                            + "   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC"
                            + "   JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CCU ON "
                            + "   TC.TABLE_NAME = CCU.TABLE_NAME AND "
                            + "   TC.CONSTRAINT_NAME = CCU.CONSTRAINT_NAME "
                            + "   WHERE "
                            + "      TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND"
                            + "      CCU.TABLE_NAME = @table_name AND"
                            + "   CCU.COLUMN_NAME = c.name) AS IsPrimaryKey "
                            + " FROM sys.columns AS c  "
                            + " JOIN sys.types AS t ON c.user_type_id=t.user_type_id "
                            + " WHERE c.object_id = OBJECT_ID(@table_name) "
                            + " ORDER BY c.column_id; ";

            using (CLIENT.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@table_name", table.Name);
                CLIENT.SqlDataReader reader = cmd.ExecuteReader();

                while (true == reader.Read())
                {
                    ColumnDefinition col = new ColumnDefinition(reader.GetString(FIELD_NAME));
                    string typeName = reader.GetString(FIELD_TYPE_NAME);
                    col.SchemeType = typeName;

                    int isPrimary = 0;                    
                    if (false == reader.IsDBNull(FIELD_IS_PRIMARY))
                        isPrimary = reader.GetInt32(FIELD_IS_PRIMARY);

                    col.IsPrimaryKey = (isPrimary > 0 ? true : false);

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
        public SqlTableInquiry(CLIENT.SqlConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 
        /// </summary>
        public SqlTableInquiry() { }
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

                foreach(TableDefinition tableInstance in tables)
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
