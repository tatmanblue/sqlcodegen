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
using BigWoo.Common;
using BigWoo.Apps.SqlCodeGen.Interfaces;
#endregion

namespace BigWoo.Apps.SqlCodeGen.Classes
{
    #region Table Definition class Implementation
    /// <summary>
    /// This class contains information about 1 table.  Some of the information
    /// comes from the command line. Other information is gleened from the database
    /// during processing.  
    /// 
    /// This class also contain overrides of program wide configuration data.  This information
    /// is loaded from the configuration file (TODO)
    /// </summary>
    public class TableDefinition
    {
        #region private data
        private string              _name = string.Empty;
        private string              _templateName = string.Empty;
        private ClassProperty       _fileName;
        List<ColumnDefinition>      _columns = new List<ColumnDefinition>();
        DataTypeCollection          _typeOverrides = null;
        #endregion

        #region properties
        /// <summary>
        /// name of the table, as received from command line input
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Template
        {
            get { return _templateName; }
            set { _templateName = value; }
        }

        public string FileName
        {
            get { return _fileName.Value.ToString(); }
            set { _fileName.Value = value; }
        }

        public List<ColumnDefinition> Columns
        {
            get { return _columns; }
        }
        
        public DataTypeCollection TypeOverrides
        { 
            get { return _typeOverrides;}
            set { _typeOverrides = value;}
        }
        #endregion

        #region ctor/init/cleaup
        private TableDefinition() { }

        public TableDefinition(string name)
        {
            _name = name;
            _fileName = new ClassProperty(_name + ".cs");
        }

        /// <summary>
        /// used by the ProgramConfiguration class to create instance of this class from
        /// a command line arguement.  By having this method here we keep the "table" knowledge
        /// in this class rather than in the ProgramConfiguration object
        /// </summary>
        /// <param name="arg">string, something like MyTable,baseclass or MyTable,baseclass,MyTable.cs</param>
        /// <returns></returns>
        public static TableDefinition FromCmdLineArgument(string arg)
        {
            const int TABLE_NAME_POS = 0;
            const int BASE_CLASS_POS = 1;
            const int OUT_FILE_POS = 2;
            const int MIN_REQUIRED = 2;

            string[] splitArgs = arg.Split(new char[] { ',' });

            if (MIN_REQUIRED > splitArgs.Length)
                throw new Exception("-t was not formatted correctly.  At a minimum, it should be tablename,baseclasstemplatename");

            TableDefinition ret = new TableDefinition();

            ret.Name = splitArgs[TABLE_NAME_POS];
            ret.Template = splitArgs[BASE_CLASS_POS];
            if (MIN_REQUIRED < splitArgs.Length)
                ret.FileName = splitArgs[OUT_FILE_POS];

            return ret;
        }
        #endregion

        #region public methods
        /// <summary>
        /// This method provides a means of having type overrides unique to a table which
        /// supercedes the global type overrides
        /// 
        /// TODO: _typeOverrides not loaded yet
        /// </summary>
        /// <param name="sqlType">string</param>
        /// <param name="csType">string</param>
        /// <returns></returns>
        public bool GetCSTypeFromSqlType(string sqlType, ref string csType)
        {
            bool ret = false;

            if (null != _typeOverrides)
            {
                foreach (DataType type in _typeOverrides)
                {
                    if (0 == string.Compare(type.SqlType, sqlType, true))
                    {
                        csType = type.CSType;
                        ret = true;
                        break;
                    }
                }
            }

            return ret;
        }
        #endregion
    }
    #endregion

  
    #region TableDefinitionCollection class Implementation
    /// <summary>
    /// </summary>
    public class TableDefinitionCollection : List<TableDefinition> {}
    #endregion
 
}
