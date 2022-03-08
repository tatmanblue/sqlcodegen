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
    /// This class is the master control logic for generating data access layer objects.
    /// </summary>
    internal class CodeGeneratorController : BaseCodeGeneratorPart
    {
        #region private data
        #endregion

        #region private methods
        /// <summary>
        /// Debugging method for listing the columns in the table
        /// </summary>
        /// <param name="columns"></param>
        [System.Diagnostics.Conditional("Debug")]        
        private void LogColumnsForTable(List<ColumnDefinition> columns)
        {
            foreach (ColumnDefinition col in columns)
            {
                Console.WriteLine(string.Format("{0} type is {1}", col.Name, col.SchemeType));
            }
        }

        /// <summary>
        /// This is the workhorse of the application.  Using the information in a TableDefinition
        /// and using the templated source code files, generate the crud code 
        /// </summary>
        /// <param name="table">TableDefinition</param>
        private void GenerateCode(TableDefinition table)
        {
            try
            {
                Console.WriteLine(string.Format("Generating code for {0}:", table.Name));
                LogColumnsForTable(table.Columns);

                // assign member variables 
                _currentTable = table;
                _currentTemplate = _templateMgr.Templates.GetByName(table.Template);

                if (null == _currentTemplate)
                    throw new InvalidCommandLineException(string.Format("Template '{0}' does not exist", table.Template));

                string classNameTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.ClassName).Region;
                string dataPropertySectionTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.DataPropertySection).Region;
                string propertySectionTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.PropertySection).Region;
                string privateDataTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.PrivateData).Region;
                string nameSpaceTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.Namespace).Region;
                string crudTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.CRUDSection).Region;
                string createTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.CRUDInsert).Region;
                string retrieveTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.CRUDSelect).Region;
                string updateTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.CRUDUpdate).Region;
                string deleteTag = _currentTemplate.GetCodeSubstitutionTag(CodeSubstutionType.CRUDDelete).Region;

                string properties = string.Empty;
                string crudText = string.Empty;

                // initialize the sub generators
                GenerateDataProperty dpGenerator = new GenerateDataProperty(_currentTable, _currentTemplate);
                GenerateCRUD crudGenerator = new GenerateCRUD(_currentTable, _currentTemplate);
                crudGenerator.Initialize();
                dpGenerator.Initialize();

                // loading the template
                string text = _currentTemplate.Load();

                // replacing the simple stuff
                text = text.Replace(classNameTag, table.Name);
                text = text.Replace(nameSpaceTag, _programConfig.NameSpace);


                // now generate and replace private data members and
                // properties
                if (true == text.Contains(dataPropertySectionTag))
                {
                    properties = dpGenerator.GenerateAll();
                    text = text.Replace(dataPropertySectionTag, properties);
                }
                else
                {
                    properties = dpGenerator.GenerateProperties();
                    text = text.Replace(propertySectionTag, properties);

                    properties = dpGenerator.GenerateDataMembers();
                    text = text.Replace(privateDataTag, properties);
                }

                // now generate and replace CRUD
                CRUDOptions options = _currentTemplate.Options;

                if (CRUDOptions.All == (options & CRUDOptions.All))
                {
                    crudText = crudGenerator.GenerateAll();
                    text = text.Replace(crudTag, crudText);
                }
                else
                {
                    if (CRUDOptions.Create == (options & CRUDOptions.Create))
                    {
                        crudText = crudGenerator.GenerateCreate();
                        text = text.Replace(createTag, crudText);
                    }

                    if (CRUDOptions.Delete == (options & CRUDOptions.Delete))
                    {
                        crudText = crudGenerator.GenerateDelete();
                        text = text.Replace(deleteTag, crudText);
                    }

                    if (CRUDOptions.Retrieve == (options & CRUDOptions.Retrieve))
                    {
                        crudText = crudGenerator.GenerateRetrieve();
                        text = text.Replace(retrieveTag, crudText);
                    }

                    if (CRUDOptions.Update == (options & CRUDOptions.Update))
                    {
                        crudText = crudGenerator.GenerateUpdate();
                        text = text.Replace(updateTag, crudText);
                    }

                }

                // and now spew out the results to the file
                using (TextWriter writer = new StreamWriter(_programConfig.OutputPath + table.FileName))
                {
                    writer.Write(text);
                    writer.Close();
                }
            }
            finally
            {
                _currentTable = null;
                _currentTemplate = null;
            }

        }
        
        #endregion

        #region ctor/init/cleanup
        public CodeGeneratorController() : base(null, null) {}
        #endregion

        #region public methods
        /// <summary>
        /// For each table we should have enough information now to generate CRUD Code.  This 
        /// method is the control loop for generating the code for each table
        /// </summary>
        public void ProcessTables()
        {
            foreach(TableDefinition table in ProgramConfiguration.Instance.Tables)
            {
                GenerateCode(table);
            }

        }
        #endregion
    }

}
