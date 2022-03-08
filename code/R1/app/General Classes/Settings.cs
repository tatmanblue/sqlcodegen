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
#region using statements
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using MattRaffelNetCode.Apps.SqlCodeGen;
using BigWoo.NET.ApplicationSupport;
#endregion

namespace MattRaffelNetCode.Apps.SqlCodeGen.Properties
{

    #region settings class implementation (partial class)
    //  This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class Settings
    {

        public Settings()
        {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            // Add code to handle the SettingChangingEvent event here.
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Add code to handle the SettingsSaving event here.
        }

    } 
    #endregion

    #region ApplicationSettingsSection implementation
    /// <summary>
    /// override our ApplicationSettingsSection to add sections for ourselves
    /// </summary>
    public class SqlCodeGenSettingsSection : ApplicationSettingsSection
    {
        [ConfigurationProperty("Templates")]
        public TemplateElementCollection Templates
        { get { return this["Templates"] as TemplateElementCollection; } }

        [ConfigurationProperty("Snippets")]
        public SnippetElementCollection Snippets
        { get { return this["Snippets"] as SnippetElementCollection; } }
    } 
    #endregion

    #region TemplateElementCollection implementation
    [ConfigurationCollection(typeof(TemplateElement), AddItemName = "templateData")]
    public class TemplateElementCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        { return new TemplateElement(); }

        protected override object GetElementKey(ConfigurationElement element)
        { return ((TemplateElement)element).Name; }

    } 
    #endregion

    #region SnippetElementCollection implementation
    [ConfigurationCollection(typeof(SnippetElement), AddItemName = "snippet")]
    public class SnippetElementCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        { return new SnippetElement(); }

        protected override object GetElementKey(ConfigurationElement element)
        { return ((SnippetElement)element).Name; }

    }
    #endregion

    #region TemplateElement implementation
    /// <summary>
    /// 
    /// </summary>
    public class TemplateElement : ConfigurationElement
    {
        #region private constants
        private const string PROPERTY_SECTION = "propertySection";
        private const string NAME = "name";
        private const string FILE_NAME = "fileName";
        private const string GENERATE_CRUD = "generateCrud";
        private const string COMBINED_PROPERY_PRIVATE_DATA = "CombinePropertiesAndData";
        #endregion

        #region private methods
        #endregion

        #region properties
        [ConfigurationProperty(NAME, IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                object obj = this[NAME];
                return (obj != null) ? (string)obj : "";
            }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(FILE_NAME, IsRequired = true)]
        public string FileName
        {
            get
            {
                object obj = this[FILE_NAME];
                return (obj != null) ? (string)obj : "";
            }
            set { this[FILE_NAME] = value; }
        }

        [ConfigurationProperty(GENERATE_CRUD, IsRequired = false)]
        public string GenerateCrud
        {
            get
            {
                object obj = this[GENERATE_CRUD];
                return (obj != null) ? (string)obj : "";
            }
            set 
            {
                ValidateCRUDProperty(value);
                this[GENERATE_CRUD] = value; 
            }
        }

        [ConfigurationProperty(COMBINED_PROPERY_PRIVATE_DATA, IsRequired = false)]
        public string CombinePropertiesWithData
        {
            get
            {
                object obj = this[COMBINED_PROPERY_PRIVATE_DATA];
                return (obj != null) ? (string)obj : "";
            }
            set 
            { 
                this[COMBINED_PROPERY_PRIVATE_DATA] = value; 
            }
        }
        #endregion

        #region property accessor methods
        /// <summary>
        /// 
        /// TODO: use the DescriptionAttribute from the enum
        /// </summary>
        /// <param name="text"></param>
        private void ValidateCRUDProperty(string text)
        {            
            if (true == string.IsNullOrEmpty(text))
                return;

            string[] crudOptions = text.Split(new char[] { ',' });
            foreach (string crudOption in crudOptions)
            {
                string lowerOption = crudOption.ToLower();
                if (0 == lowerOption.CompareTo("none"))
                {
                    continue;
                }
                else if (0 == lowerOption.CompareTo("all"))
                {
                    continue;
                }
                else if (0 == lowerOption.CompareTo("create"))
                {
                    continue;
                }
                else if (0 == lowerOption.CompareTo("retrieve"))
                {
                    continue;
                }
                else if (0 == lowerOption.CompareTo("update"))
                {
                    continue;
                }
                else if (0 == lowerOption.CompareTo("delete"))
                {
                    continue;
                }
                else
                    throw new ConfigurationErrorsException(string.Format("{0} is not valid for {1} attribute", crudOption, GENERATE_CRUD));
            }

        }

        private void ValidateCombinedProperty()
        {

        }
        #endregion

        #region overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            //if (elementName == "value")
            //{
            //    Value = reader.ReadElementContentAsString();
            //    return true;
            //};
            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
        #endregion

    } 
    #endregion

    #region SnippetElement implementation
    /// <summary>
    /// 
    /// </summary>
    public class SnippetElement : ConfigurationElement
    {
        #region private constants
        private const string NAME = "name";
        private const string FILE_NAME = "fileName";
        #endregion

        #region private methods
        #endregion

        #region properties
        [ConfigurationProperty(NAME, IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                object obj = this[NAME];
                return (obj != null) ? (string)obj : "";
            }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(FILE_NAME, IsRequired = true)]
        public string FileName
        {
            get
            {
                object obj = this[FILE_NAME];
                return (obj != null) ? (string)obj : "";
            }
            set { this[FILE_NAME] = value; }
        }
        #endregion

        #region property accessor methods
        #endregion

        #region overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            //if (elementName == "value")
            //{
            //    Value = reader.ReadElementContentAsString();
            //    return true;
            //};
            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
        #endregion

    }
    #endregion

}
