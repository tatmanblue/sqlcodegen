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
#endregion

namespace BigWoo.Apps.SqlCodeGen.Classes
{
    #region Column Definition class Implementation
    /// <summary>
    /// This class contains information about 1 column in 1 table
    /// it knows name and type and size information.  The information
    /// is generated through a query analized by the ITableInquiry object
    /// </summary>
    public class ColumnDefinition
    {
        #region private data
        private string _name = string.Empty;
        private string _schemaName = string.Empty;
        private bool   _isPrimaryKey;
        #endregion

        #region properties
        /// <summary>
        /// effectively from the name column in the sys.columns table
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// essentially the value from the name column in the sys.types table
        /// as joined on the sys.columns table 
        /// </summary>
        public string SchemeType
        {
            get { return _schemaName; }
            set { _schemaName = value; }
        }

        /// <summary>
        /// Indicates that the column is part of the primary key 
        /// which is needed for update and delete WHERE clauses in
        /// CRUD code
        /// </summary>
        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { _isPrimaryKey = value; }
        }
        #endregion

        #region ctor/init/cleaup
        public ColumnDefinition(string name, string schemaName)
        {
            _name = name;
            _schemaName = schemaName;
        }

        public ColumnDefinition(string name)
        {
            _name = name;
        } 
        #endregion

    }
    #endregion

    #region Column Definition class Implementation
    /// <summary>
    /// </summary>
    public class ColumnDefinitionCollection : List<ColumnDefinition> {}
    #endregion
}
