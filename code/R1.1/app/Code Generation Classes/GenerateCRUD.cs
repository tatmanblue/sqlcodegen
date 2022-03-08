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
using BigWoo.Apps.SqlCodeGen.Classes;
#endregion

namespace BigWoo.Apps.SqlCodeGen
{
    /// <summary>
    /// CRUD is the basic database operations.  Since this class generated code that pretty much mimics a database table
    /// it makes sense that we can generate the CRUD functions.  
    /// 
    /// The template can specify the CRUD generation in two different ways:
    /// 1) a generic CRUD_SECTION
    /// 2) or sections for each of the CRUD functions
    /// </summary>
    /// <returns>string</returns>
    internal class GenerateCRUD : BaseCodeGeneratorPart
    {

        #region private data
        private CRUDOptions _options = CRUDOptions.None;
        private List<PrivateDataMemberProperty> _tableKeys = new List<PrivateDataMemberProperty>();
        #endregion

        #region properties
        #endregion

        #region private methods
        /// <summary>
        /// Builds a where clause, sans WHERE keyword, using the columns input
        /// </summary>
        /// <returns>string, SQL WHERE clause sans WHERE keyword</returns>
        private string MakeWhereClause()
        {
            string separator = "";

            System.Text.StringBuilder whereClause = new StringBuilder();

            foreach (PrivateDataMemberProperty col in _tableKeys)
            {
                whereClause.AppendFormat("{1}{0}=@{0}_PARAM", col.ColumnName, separator);

                if (true == string.IsNullOrEmpty(separator))
                    separator = ", ";
            }

            return whereClause.ToString();
        }

        /// <summary>
        /// Build C# code for assigning values to parameters used in the where clause, relies on the values in 
        /// _tableKeys
        /// </summary>
        /// <returns>string</returns>
        private string MakeWhereClauseParams()
        {
            return MakeParamStatements("CRUDAddParamWithValue", _tableKeys);
        }

        /// <summary>
        /// Build C# code for assigning values to parameters used in the where clause, relies on the values in 
        /// _columns for making statements
        /// </summary>
        /// <param name="snippetName">string, name of the snippet for param values</param>
        /// <param name="_columns">List<PrivateDataMemberProperty></param>
        /// <returns>string</returns>
        private string MakeParamStatements(string snippetName, List<PrivateDataMemberProperty> _columns)
        {
            System.Text.StringBuilder paramClause = new StringBuilder();
            SnippetData snippet = _templateMgr.Snippets.GetByName(snippetName);
            string paramNameTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.ParamName).Region;
            string paramValueTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.ParamValue).Region;
            string snippetBody = snippet.Load();

            foreach (PrivateDataMemberProperty col in _columns)
            {
                string newParam = snippetBody;
                newParam = newParam.Replace(paramNameTag, string.Format("\"@{0}_PARAM\"", col.ColumnName));
                newParam = newParam.Replace(paramValueTag, col.MemberName);

                paramClause.Append(newParam);

            }

            return paramClause.ToString();
        }

        /// <summary>
        /// Assumption is that the snippets for property/private data/and combined are similar
        /// enough that text replacement is all that is needed.
        /// </summary>
        /// <param name="snippetName"></param>
        /// <returns></returns>
        private string GenerateFromSnippet(string snippetName)
        {
            // if this assert fires it means initialize was never called
            System.Diagnostics.Debug.Assert(0 < _privateData.Count);

            string dataTypeTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.MemberType).Region;
            string propertyNameTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.MemberPropertyName).Region;
            string dataNameTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.MemberDataName).Region;

            SnippetData snippet = _templateMgr.Snippets.GetByName(snippetName);
            string snippetTemplate = snippet.Load();

            System.Text.StringBuilder builder = new StringBuilder();
            foreach (PrivateDataMemberProperty member in _privateData)
            {
                string generatedText = snippetTemplate;
                generatedText = generatedText.Replace(dataTypeTag, member.DataType);
                generatedText = generatedText.Replace(dataNameTag, member.MemberName);
                generatedText = generatedText.Replace(propertyNameTag, member.PropertyName);

                builder.Append(generatedText);
            }

            return builder.ToString();

        }

        #endregion

        #region overrides
        /// <summary>
        /// Need to evaluate if the column is a key field and if so make sure we remember that
        /// </summary>
        /// <param name="codeColumn">PrivateDataMemberProperty</param>
        /// <param name="databaseColumn">ColumnDefinition</param>
        protected override void ProcessColumn(PrivateDataMemberProperty codeColumn, ColumnDefinition databaseColumn)
        {
            // if the column is part of the primary key, add that to our list of key fields
            // for generation of where clauses
            if (true == databaseColumn.IsPrimaryKey)
                _tableKeys.Add(codeColumn);
        }
        #endregion

        #region ctor/init/cleanup
        public GenerateCRUD(TableDefinition table, TemplateData template) : base(table, template)
        {
            _options = _currentTemplate.Options;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Causes the class to evaluate all the columns and creating the code "parts" for
        /// each column
        /// </summary>
        public void Initialize()
        {
            ProcessColumns();
        }

        /// <summary>
        /// Generates combined properties and private data members based 
        /// on dataproperty.cs.snippet
        /// </summary>
        /// <returns>string</returns>
        public string GenerateAll()
        {
            // if this assert fires it means initialize was never called
            System.Diagnostics.Debug.Assert(0 < _privateData.Count);

            StringBuilder text = new StringBuilder();
            text.Append(GenerateCreate());
            text.Append(GenerateRetrieve());
            text.Append(GenerateUpdate());
            text.Append(GenerateDelete());

            return text.ToString(); ;
        }

        /// <summary>
        /// Generates the INSERT statement
        /// </summary>
        /// <returns>string, code</returns>
        public string GenerateCreate()
        {
            // if this assert fires it means initialize was never called
            System.Diagnostics.Debug.Assert(0 < _privateData.Count);

            string sqlTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.SqlStatement).Region;
            string paramTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.SqlParams).Region;
            SnippetData snippet = _templateMgr.Snippets.GetByName("CRUDInsert");
            string snippetBody = snippet.Load();
            string paramClause = MakeParamStatements("CRUDAddParamWithValue", _privateData);
            string separator = string.Empty;

            StringBuilder columnSet = new StringBuilder();
            StringBuilder valuesSet = new StringBuilder();

            foreach (PrivateDataMemberProperty col in _privateData)
            {

                columnSet.AppendFormat("{0}{1}", separator, col.ColumnName);
                valuesSet.AppendFormat("{0}@{1}_PARAMS", separator, col.ColumnName);

                if (true == string.IsNullOrEmpty(separator))
                    separator = ", ";
            }

            string sql = string.Format("\"INSERT INTO {0} ({1}) VALUES ({2})\"", _currentTable.Name, columnSet.ToString(), valuesSet.ToString());

            //
            // INSERT INTO $TABLENAME$ ($COLUMNS$) VALUES ($VALUES$)
            //
            // generate: sql column names and value set
            // 
            Dictionary<string, string> paramsAndCode = new Dictionary<string, string>();
            paramsAndCode.Add(sqlTag, sql);
            paramsAndCode.Add(paramTag, paramClause);

            snippetBody = ReplaceTagsWithCode(snippetBody, paramsAndCode);

            return snippetBody;

        }

        /// <summary>
        /// generates an update function, updating the row based on primary key
        /// </summary>
        /// <returns>string, C# code</returns>
        public string GenerateUpdate()
        {
            // if this assert fires it means initialize was never called
            System.Diagnostics.Debug.Assert(0 < _privateData.Count);

            string sqlTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.SqlStatement).Region;
            string paramTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.SqlParams).Region;
            SnippetData snippet = _templateMgr.Snippets.GetByName("CRUDUpdate");
            string snippetBody = snippet.Load();
            string whereClause = MakeWhereClause();
            string paramClause = MakeParamStatements("CRUDAddParamWithValue", _privateData);
            StringBuilder fieldSetClause = new StringBuilder();
            string separator = string.Empty;

            foreach (PrivateDataMemberProperty col in _privateData)
            {
               
                fieldSetClause.AppendFormat("{0}{1}=@{1}_PARAMS", separator, col.ColumnName);

                if (true == string.IsNullOrEmpty(separator))
                    separator = ", ";
            }

            string sql = string.Format("\"UPDATE {0} SET {1} WHERE {2}\"", _currentTable.Name, fieldSetClause.ToString(), whereClause);

            // 
            // UPDATE $TABLENAME$ SET $COL$=$VALUE$
            //  WHERE $COL$=$VALUE$
            //
            // generate: SQL colum value pairs
            //           sql WHERE clause
            //           C# parameters for the where clause
            //
            Dictionary<string, string> paramsAndCode = new Dictionary<string, string>();
            paramsAndCode.Add(sqlTag, sql);
            paramsAndCode.Add(paramTag, paramClause);

            snippetBody = ReplaceTagsWithCode(snippetBody, paramsAndCode);

            return snippetBody;
        }

        /// <summary>
        /// Generates a delete function
        /// </summary>
        /// <returns>string, C# code</returns>
        public string GenerateDelete()
        {
            // if this assert fires it means initialize was never called
            System.Diagnostics.Debug.Assert(0 < _privateData.Count);

            string sqlTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.SqlStatement).Region;
            string paramTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.SqlParams).Region;            
            SnippetData snippet = _templateMgr.Snippets.GetByName("CRUDDelete");
            string snippetBody = snippet.Load();
            string whereClause = MakeWhereClause();
            string paramClause = MakeWhereClauseParams();
            string sql = string.Format("\"DELETE FROM {0} WHERE {1}\"", _currentTable.Name, whereClause);
            // 
            // DELETE FROM $TABLENAME$ 
            //  WHERE $COL$=$VALUE$
            //
            // generate: sql WHERE clause
            //           C# parameters for the where clause
            //
            Dictionary<string, string> paramsAndCode = new Dictionary<string, string>();
            paramsAndCode.Add(sqlTag, sql);
            paramsAndCode.Add(paramTag, paramClause);

            snippetBody = ReplaceTagsWithCode(snippetBody, paramsAndCode);


            return snippetBody;
        }

        /// <summary>
        /// Generates the select statement
        /// </summary>
        /// <returns>string, C# code</returns>
        public string GenerateRetrieve()
        {
            // if this assert fires it means initialize was never called
            System.Diagnostics.Debug.Assert(0 < _privateData.Count);

            string sqlTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.SqlStatement).Region;
            string paramTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.SqlParams).Region;
            string dataNameTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.MemberDataName).Region;
            string columnNameTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.ColumnName).Region;
            string columnIndexTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.ColumnIndex).Region;
            string readerSectionTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.ReaderAssignment).Region;
            string readerTypeFunTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.ReaderType).Region;
            SnippetData snippet = _templateMgr.Snippets.GetByName("CRUDRetrieve");
            SnippetData readerSnippet = _templateMgr.Snippets.GetByName("CRUDReader");
            string snippetBody = snippet.Load();
            string readerBody = readerSnippet.Load();
            string whereClause = MakeWhereClause();
            string paramClause = MakeWhereClauseParams();
            StringBuilder fieldSet = new StringBuilder();
            StringBuilder readerCode = new StringBuilder();
            string separator = string.Empty;
            int columnIndex = 0;

            // in the loop, build the select clause as well as the reader statements
            foreach (PrivateDataMemberProperty col in _privateData)
            {

                fieldSet.AppendFormat("{0}{1}", separator, col.ColumnName);

                string readerSyntax = readerBody;

                readerSyntax = readerSyntax.Replace(dataNameTag, col.MemberName);
                readerSyntax = readerSyntax.Replace(columnIndexTag, columnIndex.ToString());
                readerSyntax = readerSyntax.Replace(columnNameTag, col.ColumnName);
                readerSyntax = readerSyntax.Replace(readerTypeFunTag, _ddtInstance.FindDataReaderFunc(col.Definition.SchemeType));

                readerCode.Append(readerSyntax);

                columnIndex++;

                if (true == string.IsNullOrEmpty(separator))
                    separator = ", ";
            }

            string sql = string.Format("\"SELECT {0} FROM {1} WHERE {2}\"", fieldSet.ToString(), _currentTable.Name, whereClause);

            // 
            // SELECT $COLUMNS$ FROM $TABLENAME$ 
            //  WHERE $COL$=$VALUE$
            //
            // generate: SQL colum names
            //           sql WHERE clause
            //
            //           C# parameters for the where clause
            //           C# reader statements for each column selected
            Dictionary<string, string> paramsAndCode = new Dictionary<string, string>();
            paramsAndCode.Add(sqlTag, sql);
            paramsAndCode.Add(paramTag, paramClause);
            paramsAndCode.Add(readerSectionTag, readerCode.ToString());

            snippetBody = ReplaceTagsWithCode(snippetBody, paramsAndCode);

            return snippetBody;
        }
        #endregion

    }
}
