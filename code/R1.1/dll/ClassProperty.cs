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
    /// <summary>
    /// ClassProperty -- wrapper for properties that might have defaults and user overrides and ability
    /// to reset back to originial state
    /// </summary> 					 
    public class ClassProperty
    {
        #region private data
        private object _originalValue = null;
        private object _currentValue = null;
        private string _appSettingsKey = string.Empty;
        private string _sectionName = "ApplicationSettings";
        private bool _optional = false;
        private bool _hasChanged;
        #endregion

        #region properties
        public object Value
        {
            get { return GetCorrectValue(); }
            set { SetValue(value); }
        }

        public string SectionName
        {
            get { return _sectionName; }
            set { _sectionName = value; }
        }

        public bool Optional
        {
            get { return _optional; }
            set { _optional = value; }
        }
        #endregion

        #region private/protected methods
        private void SetValue(object value)
        {
            _currentValue = value;
            _hasChanged = true;
        }

        /// <summary>
        /// Here's the idea.  We have in memory defaults.  And we have application defaults set in application.config file.
        /// But wait theres more!  The user can change values (potentially) through the command line or something.  What
        /// this method has to do is decide what to return.  Here's the rules:
        /// 
        /// If the user has changed the property via the setter, return that that value, which should be in the _currentValue
        /// field.  
        /// 
        /// If the user has not changed the property, check to see whats in the application.config file.  If there is a value
        /// there, return that otherwise return the in memory default (which is _originalValue).
        /// </summary>
        /// <returns></returns>
        private object GetCorrectValue()
        {
            object ret = null;

            if (false == _hasChanged)
            {
                object appSettings = null;

                // it is possible there never was intended to be a default in the applications property
                // in which case there will be no valid _appSettingsKey value.  only make our
                // check for something in the applications properties if _appSettingsKey appears to be
                // a valid string
                if (false == string.IsNullOrEmpty(_appSettingsKey))
                {
                    ApplicationSettingsSection section = System.Configuration.ConfigurationManager.GetSection(_sectionName) as ApplicationSettingsSection;
                    SettingsElementCollection settings = section.Settings;
                    foreach (SettingsElement element in settings)
                    {
                        if (0 == string.Compare(element.Name, _appSettingsKey, true))
                        {
                            appSettings = element.Value;
                        }
                    }

                    System.Diagnostics.Debug.Assert(null != appSettings, string.Format("appSettings '{0}' is null which means its missing from the appconfig file and its not optional", _appSettingsKey));
                }

                if (false == string.IsNullOrEmpty((string) appSettings))
                    _currentValue = appSettings;
                
            }
            
            ret = _currentValue;

            return ret;
        }
        #endregion

        #region ctor/init
        /// <summary>
        /// ProgramProperty constructor
        /// </summary> 					 
        public ClassProperty(object originalValue) 
        {
            _originalValue = originalValue;
            _currentValue = _originalValue;
        }

        /// <summary>
        /// ProgramProperty constructor
        /// </summary> 					 
        public ClassProperty(object originalValue, string appSettingsKey) : this(originalValue)
        {
            _appSettingsKey = appSettingsKey;
        }

        /// <summary>
        /// ProgramProperty constructor
        /// </summary> 					 
        public ClassProperty(object originalValue, string appSettingsKey, bool optional)
            : this(originalValue, appSettingsKey)
        {
            _optional = optional;
        }
        #endregion

        #region public methods
        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _hasChanged = false;
            _currentValue = _originalValue;
        }
        #endregion

        #region overrides
        public override string ToString()
        {
            return (string) this.Value;
        }
        #endregion
    }

}
