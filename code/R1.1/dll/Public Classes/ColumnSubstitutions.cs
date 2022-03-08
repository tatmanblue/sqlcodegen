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
using System.Xml;
using System.Xml.Serialization;
using BigWoo.Apps.SqlCodeGen.Interfaces;
#endregion

namespace BigWoo.Apps.SqlCodeGen.Classes
{
    #region ColumnSubstitution implementation
    /// <summary>
    /// this class helps convert a database column name into data member and property
    /// names based on the database column name.  
    /// </summary>
    [System.Xml.Serialization.XmlRoot("ColumnSubstitution")]
    public class ColumnSubstitution
    {
        #region private data
        private string _sqlColumnName = string.Empty;
        private string _csDataMemberName = string.Empty;
        private string _csPropertyName = string.Empty;
        private bool   _keepCase = false;
        private bool   _exclude = false;
        #endregion

        #region properties
        [System.Xml.Serialization.XmlAttribute("SQL")]
        public string SqlName
        {
            get { return _sqlColumnName; }
            set { _sqlColumnName = value; }
        }

        [System.Xml.Serialization.XmlAttribute("CS")]
        public string DataMemberName
        {
            get { return _csDataMemberName; }
            set { _csDataMemberName = value; }
        }

        [System.Xml.Serialization.XmlAttribute("Property")]
        public string PropertyName
        {
            get { return _csPropertyName; }
            set { _csPropertyName = value; }
        }

        [System.Xml.Serialization.XmlAttribute("KeepCase")]
        public bool KeepCase
        {
            get { return _keepCase; }
            set { _keepCase = value; }
        }

        [System.Xml.Serialization.XmlAttribute("Exclude")]
        public bool Exclude
        {
            get { return _exclude; }
            set { _exclude = value; }
        }
        #endregion

        #region ctor/init/cleanup
        public ColumnSubstitution() { }
        #endregion

        #region
        #endregion
    } 
    #endregion

    #region ColumnSubstitutions List
    [System.Xml.Serialization.XmlRoot("ColumnSubstitutions")]
    public class ColumnSubstitutionsCollection : System.Collections.Generic.List<ColumnSubstitution> { }
    #endregion

}
