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
#endregion

namespace BigWoo.Apps.SqlCodeGen
{
    #region ColumnSubstitutionManager implementation
    /// <summary>
    /// The purpose of the column substitutions is to allow the C# data members and properties to be
    /// represented with different names than the database columns they eventually bind to.  This can be important
    /// as database column names can be cryptic but the C# names do not need to be.
    /// 
    /// The data is read from the datatypes file, as specified by the ApplicationSettings.Settings.DefaultSQLNameChart
    /// entry in the config file.
    /// 
    /// The current assumption is that every instance of the same db column name gets the same substitution values.  Later on
    /// it would be nice to let tables have overrides as needed.
    /// 
    /// This class is a singleton
    /// </summary>
    internal class ColumnSubstitutionManager
    {
        #region private data
        private static ColumnSubstitutionManager _instance = new ColumnSubstitutionManager();
        private ColumnSubstitutionsCollection _substitutions = null;
        #endregion

        #region properties
        public static ColumnSubstitutionManager Instance
        {
            get { return _instance; }
        }

        public ColumnSubstitutionsCollection Substitutions
        {
            get { return _substitutions; }
        }
        #endregion

        #region private methods
        /// <summary>
        /// loads the subsitution information from the file, file name specified in the application config file
        /// </summary>
        private void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ColumnSubstitutionsCollection));
            
            using (TextReader reader = new StreamReader(ProgramConfiguration.Instance.DefaultNamesChart))
            {
                _substitutions = (ColumnSubstitutionsCollection)serializer.Deserialize(reader);
                reader.Close();
            }
        }
        #endregion

        #region ctor/init/cleanup
        private ColumnSubstitutionManager() 
        {
            _substitutions = new ColumnSubstitutionsCollection();
            Load();
        }
        #endregion

        #region public methods    
        /// <summary>
        /// Given a column name, name as found in the database (expectation is ITableInquiry built this information)
        /// find a matching ColumnSubstitution.  It is possible there is no ColumnSubstitution entry.
        /// </summary>
        /// <param name="sqlColumnName">string, sql colum name</param>
        /// <returns>ColumnSubstitution, null if no match found</returns>
        public ColumnSubstitution GetColumnBySqlName(string sqlColumnName)
        {
            ColumnSubstitution ret = null;

            foreach (ColumnSubstitution col in _substitutions)
            {
                if (0 == string.Compare(col.SqlName, sqlColumnName, true))
                {
                    ret = col;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Helper function.  Returns the value of the datamembername property of a ColumnSubstitution having 
        /// matching column name.  It is possible no match is found.  Assumption is that the caller already knows
        /// a ColumnSubstitution exists.  
        /// </summary>
        /// <param name="sqlColumnName">string</param>
        /// <returns>string, empty of no match is found</returns>
        public string GetDataMemberNameBySqlName(string sqlColumnName)
        {
            string ret = string.Empty;

            foreach (ColumnSubstitution col in _substitutions)
            {
                if (0 == string.Compare(col.SqlName, sqlColumnName, true))
                {
                    ret = col.DataMemberName;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Helper function.  Returns the value of the propertyname property of a ColumnSubstitution having
        /// matching column name.  It is possible no match is found.  Assumption is that the caller already knows
        /// a ColumnSubstitution exists.  
        /// </summary>
        /// <param name="sqlColumnName">string</param>
        /// <returns>string, empty of no match is found</returns>
        public string GetPropertyNameBySqlName(string sqlColumnName)
        {
            string ret = string.Empty;

            foreach (ColumnSubstitution col in _substitutions)
            {
                if (0 == string.Compare(col.SqlName, sqlColumnName, true))
                {
                    ret = col.PropertyName;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Helper function.  Returns the value of the KeepCase property a ColumnSubstitution having
        /// matching column name.  It is possible no match is found.  Assumption is that the caller already knows
        /// a ColumnSubstitution exists.  
        /// </summary>
        /// <param name="sqlColumnName">string</param>
        /// <returns>bool, false does not ensure a match was found</returns>
        public bool GetKeepCaseBySqlName(string sqlColumnName)
        {
            foreach (ColumnSubstitution col in _substitutions)
            {
                if (0 == string.Compare(col.SqlName, sqlColumnName, true))
                {
                    return col.KeepCase;
                }
            }

            return false;
        }
  
        #endregion
    }
    #endregion
}
