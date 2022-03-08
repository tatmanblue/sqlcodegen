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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using BigWoo.Apps.SqlCodeGen.Classes;
using BigWoo.Apps.SqlCodeGen.Interfaces;
#endregion

namespace BigWoo.Apps.SqlCodeGen
{
    #region DataTypeConversions implementation
    /// <summary>
    /// this is the class used to translate database types into C# types
    /// such as varchar to string for the purpose of building 
    /// data members and properties and reader function calls
    /// 
    /// The data is read from the datatypes file, as specified by the ApplicationSettings.Settings.DefaultDataTypesFile
    /// entry in the config file.
    /// 
    /// This class is a singleton
    /// </summary>
    internal class DefaultDataTypeConversions : IDataTypeConversions
    {
        #region private data
        private static DefaultDataTypeConversions   _instance = new DefaultDataTypeConversions();
        private DataTypeCollection                  _defaultCollection = null;  
        #endregion

        #region properties
        public static DefaultDataTypeConversions Instance
        {
            get { return _instance; }
        }

        public DataTypeCollection List
        {
            get { return _defaultCollection; }
        }
        #endregion

        #region private methods
        /// <summary>
        /// reads in the datatypes from the file, file name is specified in the config file
        /// </summary>
        private void LoadDefaults()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataTypeCollection));
            
            using (TextReader reader = new StreamReader(ProgramConfiguration.Instance.DefaultDataTypesFile))
            {
                _defaultCollection = (DataTypeCollection)serializer.Deserialize(reader);
                reader.Close();
            }        
        }
        #endregion

        #region ctor init cleanup        
        private DefaultDataTypeConversions()
        {
            _defaultCollection = new DataTypeCollection();
            LoadDefaults();
        }
        #endregion

        #region public methods
        /// <summary>
        /// For a given SQL type, such as int (that would be SQL="INT" in the datatypes file) return the 
        /// data reader function expected in the code.  in the example here, would return GetInt32 (CSReaderFunc="GetInt32")
        /// </summary>
        /// <param name="sqlType">string</param>
        /// <returns>string</returns>
        public string FindDataReaderFunc(string sqlType)
        {
            string ret = string.Empty;

            foreach (DataType type in _defaultCollection)
            {
                if (0 == string.Compare(type.SqlType, sqlType, true))
                {
                    ret = type.CSReaderFunctionName;
                }
            }

            // the default.datatypes file is the last stop gag system for identifying a c# type
            // for an sql type, so throw an assert if its not found.  TODO: is this really the best
            // way?
            System.Diagnostics.Trace.Assert(false == string.IsNullOrEmpty(ret), string.Format("the default file is missing '{0}'", sqlType));

            return ret;

        }

        /// <summary>
        /// For a given SQL type, such as int (that would be SQL="INT" in the datatypes file) return the 
        /// expected C# data type.  In the example here would return int (CS="int" )
        /// </summary>
        /// <param name="sqlType">string</param>
        /// <returns>string</returns>
        public string FindConversion(string sqlType)
        {
            string ret = string.Empty;

            foreach (DataType type in _defaultCollection)
            {
                if (0 == string.Compare(type.SqlType, sqlType, true))
                {
                    ret = type.CSType;
                }
            }

            // the default.datatypes file is the last stop gag system for identifying a c# type
            // for an sql type, so throw an assert if its not found.  TODO: is this really the best
            // way?
            System.Diagnostics.Trace.Assert(false == string.IsNullOrEmpty(ret), string.Format("the default file is missing '{0}'", sqlType));

            return ret;
        }

        /// <summary>
        /// Helper method that first queries the table if it has the CS type for sqlType and if not
        /// uses the defaults.  (Currently TableDefinition never loads its own types so this method
        /// always returns default but its here in place for when TableDefinition is properly updated)
        /// </summary>
        /// <param name="table">TableDefinition</param>
        /// <param name="sqlType">string, database column type</param>
        /// <returns>string, C# type</returns>
        public string FindConversion(TableDefinition table, string sqlType)
        {
            string csType = string.Empty;

            if (false == table.GetCSTypeFromSqlType(sqlType, ref csType))
                csType = FindConversion(sqlType);

            // the default.datatypes file is the last stop gag system for identifying a c# type
            // for an sql type, so throw an assert if its not found.  TODO: is this really the best
            // way?
            System.Diagnostics.Trace.Assert(false == string.IsNullOrEmpty(csType), string.Format("the default file is missing '{0}'", sqlType));

            return csType;
        }
        #endregion

    } 
    #endregion
}
