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
using System.Configuration;
#endregion

namespace BigWoo.Apps.SqlCodeGen
{
    #region ApplicationSettingsSection implementation
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationSettingsSection : ConfigurationSection
    {
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("Settings")]
        public SettingsElementCollection Settings
        { get { return this["Settings"] as SettingsElementCollection; } }
    }
    #endregion

    #region SettingsElementCollection implementation
    /// <summary>
    /// 
    /// </summary>
    [ConfigurationCollection(typeof(SettingsElement), AddItemName = "setting")]
    public class SettingsElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            SettingsElement element = this[key] as SettingsElement;
            return element.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        { return new SettingsElement(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        { return ((SettingsElement)element).Name; }

    }
    #endregion

    #region SettingsElement implementation
    /// <summary>
    /// 
    /// </summary>
    public class SettingsElement : ConfigurationElement
    {
        #region private constants
        #endregion

        #region private data
        #endregion

        #region properties
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                object obj = this["name"];
                return (obj != null) ? (string)obj : "";
            }

            set { this["name"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("value", IsKey = true, IsRequired = true)]
        public string Value
        {
            get
            {
                object obj = this["value"];
                return (obj != null) ? (string)obj : "";
            }

            set { this["value"] = value; }
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
}
