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
    #region template data class
    /// <summary>
    /// Identifies which CRUD types to generate
    /// The Description identifies the text expected from the config file
    /// </summary>
    [Flags]
    internal enum CRUDOptions
    {
        [System.ComponentModel.Description("None")]
        None,
        [System.ComponentModel.Description("Create")]
        Create,
        [System.ComponentModel.Description("Retrieve")]
        Retrieve,
        [System.ComponentModel.Description("Update")]
        Update,
        [System.ComponentModel.Description("Delete")]
        Delete,
        [System.ComponentModel.Description("All")]
        All = CRUDOptions.Create | CRUDOptions.Delete | CRUDOptions.Retrieve | CRUDOptions.Update
    }

    /// <summary>
    /// Represents template information in the application config file.  The key into this section
    /// is the Table.Template value which is the match to TemplateData.Name
    /// 
    /// Design Note: chose not to use the TemplateElement class because I just didnt like it's
    /// implementation (being derived of ConfigurationElement), 
    /// so I made this class which could "auto create" from a TemplateElement instance
    /// </summary>
    internal class TemplateData : BaseData<TemplateData>
    {
        #region private data
        private CRUDOptions _crudOptions = CRUDOptions.None;
        private CodeSubstitutionTagCollection _overrideTags = new CodeSubstitutionTagCollection();
        #endregion

        #region properties
        /// <summary>
        /// Options for generating CRUD code
        /// </summary>
        public CRUDOptions Options
        {
            get { return _crudOptions; }
            set { _crudOptions = value; }
        }
        #endregion

        #region private methods
        #endregion

        #region ctor/init/cleanup
        public TemplateData() { }

        /// <summary>
        /// Creates and instance of TemplateData from a TemplateElement, which is information from the app.config file.
        /// </summary>
        /// <param name="element">TemplateElement </param>
        /// <returns>TemplateData</returns>
        public static TemplateData FromTemplateElement(TemplateElement element)
        {
            TemplateData ret = new TemplateData();

            ret.Name = element.Name;
            ret.FileName = element.FileName;

            // TODO: use the DescriptionAttribute from the enum
            if (false == string.IsNullOrEmpty(element.GenerateCrud))
            {
                string[] crudOptions = element.GenerateCrud.Split(new char[] { ',', '|' });
                foreach (string crudOption in crudOptions)
                {
                    if (0 == string.Compare(crudOption, "none", true))
                    {
                        ret.Options = CRUDOptions.None;
                        break;
                    }
                    else if (0 == string.Compare(crudOption, "all", true))
                    {
                        ret.Options = CRUDOptions.All;
                        break;
                    }
                    else if (0 == string.Compare(crudOption, "create", true))
                    {
                        ret.Options |= CRUDOptions.Create;
                    }
                    else if (0 == string.Compare(crudOption, "retrieve", true))
                    {
                        ret.Options |= CRUDOptions.Retrieve;
                    }
                    else if (0 == string.Compare(crudOption, "update", true))
                    {
                        ret.Options |= CRUDOptions.Update;
                    }
                    else if (0 == string.Compare(crudOption, "delete", true))
                    {
                        ret.Options |= CRUDOptions.Delete;
                    }
                }
            }

            return ret;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Looks for the appropriate CodeSubstitutionTag of a give type.  Looks first in the template
        /// overrides.  If nothing exists in the overrides, returns the default.
        /// </summary>
        /// <param name="type">CodeSubstutionType</param>
        /// <returns>CodeSubstitutionTag</returns>
        public CodeSubstitutionTag GetCodeSubstitutionTag(CodeSubstutionType type)
        {
            CodeSubstitutionTag ret = TemplateSnippetManager.Instance.DefaultCodeTags.GetByType(type);

            if (0 < _overrideTags.Count)
            {
                foreach (CodeSubstitutionTag tag in _overrideTags)
                {
                    if (type == tag.Type)
                    {
                        ret = tag;
                        break;
                    }
                }
            }

            return ret;
        }
        #endregion

    }
    #endregion

    #region TemplateDataCollection implementation
    /// <summary>
    /// 
    /// </summary>
    [System.Xml.Serialization.XmlRoot("Templates")]
    internal class TemplateDataCollection : System.Collections.Generic.List<TemplateData>
    {
        #region public methods
        /// <summary>
        /// Looks for a template by name.  
        /// </summary>
        /// <param name="name">string, name of a template.  Name is specified on the commandline and should match name attribute</param>
        /// <returns>TemplateData, null if template not found</returns>
        public TemplateData GetByName(string name)
        {
            TemplateData ret = null;

            foreach (TemplateData template in this)
            {
                if (0 == template.Name.CompareTo(name))
                {
                    ret = template;
                    break;
                }
            }

            return ret;
        }
        #endregion
    }
    #endregion 
}
