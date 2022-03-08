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
using BigWoo.NET.Utility;
using System.Configuration;
#endregion

namespace MattRaffelNetCode.Apps.SqlCodeGen
{
    #region code substitution tags
    /// <summary>
    /// ID for the different tags in templates that will be replaced with generated code
    /// </summary>
    internal enum CodeSubstutionType
    {
        NA,
        ClassName,
        ColumnName,
        ColumnIndex,
        CRUDSection,
        CRUDInsert,
        CRUDSelect,
        CRUDUpdate,
        CRUDDelete,
        DataPropertySection,
        FunctionParameters,
        MemberType,
        MemberDataName,
        MemberPropertyName,
        Namespace,
        ParamName,
        ParamValue,
        PropertySection,
        PrivateData,
        SqlStatement,
        SqlParams,
        ReaderAssignment,
        ReaderType
    }

    /// <summary>
    /// Inside of a template we expect something like $NAMESPACE$ to mean that 
    /// we will substite that tag with some meaningful code.  Because the plan
    /// is to allow the user override the default tags with their own the tags,
    /// we will use this class to manage
    /// </summary>
    internal class CodeSubstitutionTag
    {
        #region private data
        private ClassProperty _region;
        private CodeSubstutionType _type = CodeSubstutionType.NA;
        #endregion

        #region properties
        public string Region
        {
            get { return _region.Value.ToString(); }
            set { _region.Value = value; }
        }

        public CodeSubstutionType Type
        {
            get { return _type; }
        }
        #endregion

        #region ctor/init/cleaup
        private CodeSubstitutionTag() { }

        public CodeSubstitutionTag(string defaultValue, CodeSubstutionType type)
        {
            _region = new ClassProperty(defaultValue);
            _type = type;
        }
        #endregion
    }
    #endregion

    #region CodeSubstitutionTagCollection implementation
    /// <summary>
    /// A collection of CodeSubstitutionTag.  For the default collection
    /// there sould be one CodeSubstitutionTag for each of the CodeSubstutionType
    /// enumeration values.
    /// </summary>
    internal class CodeSubstitutionTagCollection : List<CodeSubstitutionTag>
    {
        /// <summary>
        /// Iterates through the list of CodeSubstitutionTag until one 
        /// of said type is found.   Throws ProgramFlowException if the type isnt found
        /// </summary>
        /// <param name="type">CodeSubstutionType</param>
        /// <returns>CodeSubstitutionTag</returns>
        public CodeSubstitutionTag GetByType(CodeSubstutionType type)
        {
            CodeSubstitutionTag ret = null;

            foreach (CodeSubstitutionTag tag in this)
            {
                if (tag.Type == type)
                {
                    ret = tag;
                    break;
                }
            }

            // this exception means the list was never populated correctly as it is expected
            // that all types get populated.  there is actually an exception to the exception
            // and the caller should be prepared for that.  the exception is user overridden
            // values, as we may expect that they will over ride some of the tags but not all            
            if (null == ret)
            {
                throw new ProgramFlowException(string.Format("{0} expected but not found", type.ToString()));
            }

            return ret;
        }
    }
    #endregion
}
