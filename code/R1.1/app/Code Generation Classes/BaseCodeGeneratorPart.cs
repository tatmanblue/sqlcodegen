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
using BigWoo.Apps.SqlCodeGen.Interfaces;
using BigWoo.Apps.SqlCodeGen.Classes;
#endregion

namespace BigWoo.Apps.SqlCodeGen
{
    /// <summary>
    /// This class is the base class for the code generation classes that handle specific parts of the code generation logic
    /// 
    /// The current design has this class as a base class for the "concrete" code generator classes.  There is more refactoring
    /// that needs to be done as the hierchy itself isnt really OO
    /// </summary>
    internal class BaseCodeGeneratorPart
    {
        #region private classes
        /// <summary>
        /// This class is a "refactored" ColumnDefinition class that contains the information
        /// we need to translate a ColumnDefinition into C# code
        /// </summary>
        protected class PrivateDataMemberProperty
        {
            #region private data
            private string _dataType = string.Empty;
            private string _memberName = string.Empty;
            private string _propertyName = string.Empty;
            private string _dbColName = string.Empty;
            private ColumnDefinition _definition = null;
            #endregion

            #region properties
            public ColumnDefinition Definition
            {
                get { return _definition; }
                set { _definition = value; }
            }

            public string ColumnName
            {
                get { return _dbColName; }
                set { _dbColName = value; }
            }

            public string DataType
            {
                get { return _dataType; }
                set { _dataType = value; }
            }

            public string MemberName
            {
                get { return _memberName; }
                set { _memberName = value; }
            }

            public string PropertyName
            {
                get { return _propertyName; }
                set { _propertyName = value; }
            }
            #endregion

            #region ctor/init
            /// <summary>
            /// 
            /// </summary>
            public PrivateDataMemberProperty() { }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dataType"></param>
            /// <param name="memberName"></param>
            /// <param name="propertyName"></param>
            /// <param name="colName"></param>
            public PrivateDataMemberProperty(string dataType, string memberName, string propertyName, string colName)
            {
                _dataType = dataType;
                _memberName = memberName;
                _propertyName = propertyName;
                _dbColName = colName;
            }
            #endregion

        }
        #endregion

        #region private data/protected data
        // references to instances so that the code is more compact
        protected readonly TemplateSnippetManager _templateMgr = TemplateSnippetManager.Instance;
        protected readonly ProgramConfiguration _programConfig = ProgramConfiguration.Instance;
        protected readonly DefaultDataTypeConversions _ddtInstance = DefaultDataTypeConversions.Instance;
        protected readonly ColumnSubstitutionManager _csInstance = ColumnSubstitutionManager.Instance;

        protected TableDefinition _currentTable = null;
        protected TemplateData _currentTemplate = null;
        protected List<PrivateDataMemberProperty> _privateData = new List<PrivateDataMemberProperty>();
        protected IDataMemberNameFormatter _dataMemberFormatter = null;
        #endregion

        #region properties
        private IDataMemberNameFormatter DataMemberNameFormatter
        {
            get { return GetDataMemberNameFormatter(); }
        }
        #endregion

        #region private methods
        /// <summary>
        /// lazy load the formatter because we know it can throw an exception and we dont
        /// want that happening in the constructor
        /// </summary>
        /// <returns></returns>
        private IDataMemberNameFormatter GetDataMemberNameFormatter()
        {
            if (null != _dataMemberFormatter)
                return _dataMemberFormatter;

            string formatterConfig = ProgramConfiguration.Instance.MemberNameFormatterConfigString;

            _dataMemberFormatter = MemberNameFormatterLoader.LoadFormatter(formatterConfig, true);

            return _dataMemberFormatter;
        }
        #endregion

        #region protected (virtual) methods
        /// <summary>
        /// This method takes template (or snippet) content and replaces the tags with appropriate "syntax".  The 
        /// appropriate syntax is associated with a given tag already as input in the Dictionary
        /// </summary>
        /// <param name="sourceCode">string, code from the template or snippet</param>
        /// <param name="paramsAndCode">Dictionary, mapping tags with code that should replace it</param>
        /// <returns>string, reformatted with tags replaced by generated code</returns>
        protected string ReplaceTagsWithCode(string sourceCode, Dictionary<string, string> paramsAndCode)
        {
            foreach (KeyValuePair<string, string> paramWithCode in paramsAndCode)
            {
                sourceCode = sourceCode.Replace(paramWithCode.Key, paramWithCode.Value);
            }

            return sourceCode;
        }

        /// <summary>
        /// Builds the data we need to generate the code from the column information
        /// </summary>
        protected void ProcessColumns()
        {
            foreach (ColumnDefinition col in _currentTable.Columns)
            {
                ColumnSubstitution columnSubstition = _csInstance.GetColumnBySqlName(col.Name);

                // exclude means this column shouldnt show up in the property
                // list...
                if ((null != columnSubstition) && (true == columnSubstition.Exclude))
                    continue;

                string dataType = _ddtInstance.FindConversion(_currentTable, col.SchemeType);
                string dataName = MakeDataMemberName(col, columnSubstition);

                // generate the default Property and data member names                
                string propertyName = col.Name;

                // if there is a propertyname override specified use it
                if ((null != columnSubstition) && (0 < columnSubstition.PropertyName.Length))
                    propertyName = columnSubstition.PropertyName;

                // use a virtual factory method, giving derived classes a chance to create their own
                // PrivateDataMemberProperty derived instances if needed
                PrivateDataMemberProperty codeColumn = CreatePrivateDataMemberProperty(dataType, dataName, propertyName, col.Name);

                codeColumn.Definition = col;

                // give derived classes a chance to evaluate new column
                ProcessColumn(codeColumn, col);

                // get the data into the list
                _privateData.Add(codeColumn);
            }
        }

        /// <summary>
        /// Helper accessor method for generating the data member name for a field.  
        /// 
        /// If a formatter of IDataMemberNameFormatter has already been laoded (aka
        /// _dataMemberFormatter is not null) then it uses that.  Otherwise looks for the specified formatter
        /// in the config file and tries to load that.  If that fails it attempts to
        /// load the default.   If that fails then an exception is thrown.  
        /// 
        /// </summary>
        /// <param name="col">ColumnDefinition</param>
        /// <param name="substitution">ColumnSubstitution</param>
        /// <returns>string, something like _data</returns>
        protected string MakeDataMemberName(ColumnDefinition col, ColumnSubstitution substitution)
        {
            return DataMemberNameFormatter.MakeDataMemberName(col, substitution);
        }

        /// <summary>
        /// Factory method for creating PrivateDataMemberProperty instances.  Virtual so that derived classes could implement their
        /// own derived PrivateDataMemberProperty type and create it instead....
        /// </summary>
        /// <param name="dataType">string</param>
        /// <param name="dataName">string</param>
        /// <param name="propertyName">string</param>
        /// <param name="dbColName">string</param>
        /// <returns></returns>
        protected virtual PrivateDataMemberProperty CreatePrivateDataMemberProperty(string dataType, string dataName, string propertyName, string dbColName)
        {
            return new PrivateDataMemberProperty(dataType, dataName, propertyName, dbColName);
        }

        /// <summary>
        /// At the base level, does nothing.  Since the core information is expected to be needed by everyone, that core code is not in this method
        /// there by preventing accidental "skipping" of core logic
        /// </summary>
        /// <param name="codeColumn">PrivateDataMemberProperty</param>
        /// <param name="databaseColumn">ColumnDefinition</param>
        protected virtual void ProcessColumn(PrivateDataMemberProperty codeColumn, ColumnDefinition databaseColumn) { }
        #endregion

        #region ctor/init/cleanup
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="table">TableDefinition</param>
        /// <param name="template">TemplateData</param>
        public BaseCodeGeneratorPart(TableDefinition table, TemplateData template)
        {
            _currentTable = table;
            _currentTemplate = template;
        }
        #endregion

        #region public + public virtual methods
        #endregion



    }
}
