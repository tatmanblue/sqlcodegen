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
    /// Generates the code for Properties and data members
    /// </summary>
    internal class GenerateDataProperty : BaseCodeGeneratorPart
    {
        #region private data
        #endregion

        #region propties
        #endregion

        #region private methods
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

        #region ctor/init/cleanup
        public GenerateDataProperty(TableDefinition table, TemplateData template) : base(table, template) { }
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
        /// Generates Propertye code based on the property.cs.snippet 
        /// </summary>
        /// <returns>string</returns>
        public string GenerateProperties()
        {
            // if this assert fires it means initialize was never called
            System.Diagnostics.Debug.Assert(0 < _privateData.Count);

            return GenerateFromSnippet("Property");
        }

        /// <summary>
        /// Generates the private data members based on data.cs.snippet
        /// </summary>
        /// <returns>string</returns>
        public string GenerateDataMembers()
        {
            // if this assert fires it means initialize was never called
            System.Diagnostics.Debug.Assert(0 < _privateData.Count);

            return GenerateFromSnippet("PrivateData");
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

            return GenerateFromSnippet("DataProperty");
        }

        #endregion
    }
}
