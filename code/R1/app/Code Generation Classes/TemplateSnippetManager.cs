/*
 ******************************************************************************
 This file is part of MattRaffelNetCode.

    MattRaffelNetCode is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    MattRaffelNetCode is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MattRaffelNetCode; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA


    architected and written by 
    matt raffel 
    matt.raffel@mindspring.com

       copyright (c) 2007 by matt raffel unless noted otherwise

 ******************************************************************************
*/
#region using statments
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MattRaffelNetCode.Apps.SqlCodeGen.Properties;
using System.Configuration;
#endregion

namespace MattRaffelNetCode.Apps.SqlCodeGen
{
    #region BaseData implementation -- needs to be finished
    internal class BaseData<T>  
        where T : BaseData<T>        
    {
        #region private data
        private string _name = string.Empty;
        private string _fileName = string.Empty;
        #endregion

        #region properties
        /// <summary>
        /// Name of the template, name must match a member in the config file
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Unqualified file name of the template on disk
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>
        /// Fully qualified file name of the template on disk
        /// </summary>
        public string FullFileName
        {
            get { return ProgramConfiguration.Instance.TemplatePath + FileName; }
        }

        #endregion

        #region ctor/init/cleanup
        public BaseData() { }

        #endregion

        #region public methods

        /// <summary>
        /// loads the template source from the harddrive into memory.  Throws exception
        /// if the file is not found
        /// </summary>
        /// <returns>string</returns>
        public string Load()
        {
            string text = string.Empty;
            string fileName = FullFileName;

            if (false == File.Exists(fileName))
                throw new FileNotFoundException("template file was not found", fileName);

            using (TextReader reader = new StreamReader(fileName))
            {
                text = reader.ReadToEnd();
                reader.Close();
            }

            return text;
        }
        #endregion

    }
    #endregion

    #region TemplateSnippetManager implementation
    /// <summary>
    /// Templates and their support, snippets, are the center of code generation.
    /// 
    /// Templates are user defined.  They are effectively C# code with some special
    /// markups that help identify what to generate and where to put it.  The creator 
    /// of the templates must put some special tags in the code to identify where
    /// certain pieces of code can go.   The following default special tags have are supported
    /// 
    /// $CLASS_NAME$
    /// $PROPERTY_SECTION&
    /// $PRIVATE_DATA$
    /// $NAME_SPACE$
    /// $CRUD_SECTION$  -- which means we use our own stubs for the methods
    /// $INSERT_SECTION$ -- which means put the insert SQL here
    /// $UPDATE_SECTION$ -- which means put the update SQL here
    /// $DELETE_SECTION$ -- which means put the delete SQL here
    /// $SELECT_SECTION$ -- which means put the select SQL here
    /// "$DATA_TYPE$"     
    /// "$PROPERTY_NAME$"
    /// "$DATA_NAME$" 
    /// 
    /// TODO: snippets
    /// TODO: The tags can be overridden to be user defined tags on a per template basis
    /// </summary>
    internal class TemplateSnippetManager
    {
        #region private data
        private static TemplateSnippetManager       _instance = new TemplateSnippetManager();
        private TemplateDataCollection              _templates = new TemplateDataCollection();
        private CodeSubstitutionTagCollection       _defaultCodeTags = new CodeSubstitutionTagCollection();
        private SnippetDataCollection               _snippets = new SnippetDataCollection();
        #endregion

        #region private methods
        /// <summary>
        /// Gets all the template information from the config file into program usable data
        /// </summary>
        private void InitializeFromConfigFile()
        {
            SqlCodeGenSettingsSection section = System.Configuration.ConfigurationManager.GetSection("ApplicationSettings") as SqlCodeGenSettingsSection;

            // its possible that the config file would be missing the <Templates> tag 
            // so if no templates exist, need to error
            if (null == section)
                throw new System.Configuration.ConfigurationErrorsException("no templates were defined.  this application is exiting");

            System.Diagnostics.Debug.Assert(null != section, "ApplicationSettings is missing from the config file");

            foreach (TemplateElement template in section.Templates)
            {
                _templates.Add(TemplateData.FromTemplateElement(template));
            }

            // its possible that the config file would have <Templates> tag but no <Template>
            // so if no templates exist, need to error
            if (0 == _templates.Count)
                throw new System.Configuration.ConfigurationErrorsException("no templates were defined.  this application is exiting");

            foreach (SnippetElement snippet in section.Snippets)
            {
                _snippets.Add(SnippetData.FromTemplateElement(snippet));
            }


        }

        private void InitializeDefaultCodeTags()
        {
            // TODO:  be nice if ths was loaded from a file, so that the user (or me) could define their own
            // tags...
            _defaultCodeTags.Add(new CodeSubstitutionTag("$NAME_SPACE$",       CodeSubstutionType.Namespace));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$CLASS_NAME$",       CodeSubstutionType.ClassName));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$PRIVATE_DATA$",     CodeSubstutionType.PrivateData));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$PROPERTY_SECTION$", CodeSubstutionType.PropertySection));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$DATA_PROPERTY_SECTION$", CodeSubstutionType.DataPropertySection));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$CRUD_SECTION$",     CodeSubstutionType.CRUDSection));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$INSERT_SECTION$",   CodeSubstutionType.CRUDInsert));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$SELECT_SECTION$",   CodeSubstutionType.CRUDSelect));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$UPDATE_SECTION$",   CodeSubstutionType.CRUDUpdate));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$DELETE_SECTION$",   CodeSubstutionType.CRUDDelete));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$DATA_TYPE$",        CodeSubstutionType.MemberType));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$PROPERTY_NAME$",    CodeSubstutionType.MemberPropertyName));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$DATA_NAME$",        CodeSubstutionType.MemberDataName));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$SQL_STATMENT$",     CodeSubstutionType.SqlStatement));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$SQL_PARAMS$",       CodeSubstutionType.SqlParams));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$PARAM_NAME$", CodeSubstutionType.ParamName));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$PARAM_VALUE$",      CodeSubstutionType.ParamValue));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$ASSIGN_DATA_MEMBERS$", CodeSubstutionType.ReaderAssignment));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$READER_TYPE_METHOD$", CodeSubstutionType.ReaderType));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$FUNC_PARAMS$",      CodeSubstutionType.FunctionParameters));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$COLUMN_NAME$", CodeSubstutionType.ColumnName));
            _defaultCodeTags.Add(new CodeSubstitutionTag("$COL_INDEX$", CodeSubstutionType.ColumnIndex));
        }
        #endregion

        #region properties
        public static TemplateSnippetManager Instance
        {
            get { return _instance; }
        }

        public TemplateDataCollection Templates
        {
            get { return _templates; }
        }

        public SnippetDataCollection Snippets
        {
            get { return _snippets; }
        }

        public CodeSubstitutionTagCollection DefaultCodeTags
        {
            get { return _defaultCodeTags; }
        }
        #endregion

        #region ctor/init/cleanup
        private TemplateSnippetManager()
        {
            InitializeFromConfigFile();
            InitializeDefaultCodeTags();
        }
        #endregion

        #region public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string LoadTemplateFile(string fileName)
        {
            string text = string.Empty;

            if (false == File.Exists(fileName))
                throw new FileNotFoundException("template file was not found", fileName);

            using (TextReader reader = new StreamReader(fileName))
            {
                text = reader.ReadToEnd();
                reader.Close();
            }

            return text;
        }

        /// <summary>
        /// Given a template name, using the configuration information, builds a complete
        /// file name for a template and checks that the template exists
        /// </summary>
        /// <param name="tableTemplateName">string, should be Table.Template value</param>
        /// <returns>string, full path</returns>
        public string GetTemplateFileNameName(string tableTemplateName)
        {
            string ret = string.Empty;

            foreach (TemplateData template in _templates)
            {
                if (0 == template.Name.CompareTo(tableTemplateName))
                {
                    ret = ProgramConfiguration.Instance.TemplatePath + template.FileName;

                    if (false == File.Exists(ret))
                        throw new FileNotFoundException("template file was not found", template.FileName);

                    break;
                }
            }

            // sanity check
            if (true == string.IsNullOrEmpty(ret))
                throw new ProgramFlowException(string.Format("{0} template not found", tableTemplateName));

            return ret;
        }
        #endregion
    } 
    #endregion
}
