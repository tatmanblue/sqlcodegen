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
#endregion

namespace BigWoo.Apps.SqlCodeGen.Classes
{
    #region DataType implementation
    /// <summary>
    /// Represents a single database to C# type conversion
    /// </summary>
    [System.Xml.Serialization.XmlRoot("DataType")]
    public class DataType
    {
        #region private data
        private string _sqlTypeName = string.Empty;
        private string _csTypeName = string.Empty;
        private string _csReaderFunctionName = string.Empty;
        #endregion

        #region properties
        [System.Xml.Serialization.XmlAttribute("SQL")]
        public string SqlType
        {
            get { return _sqlTypeName; }
            set { _sqlTypeName = value; }
        }

        [System.Xml.Serialization.XmlAttribute("CS")]
        public string CSType
        {
            get { return _csTypeName; }
            set { _csTypeName = value; }
        }

        [System.Xml.Serialization.XmlAttribute("CSReaderFunc")]
        public string CSReaderFunctionName
        {
            get { return _csReaderFunctionName; }
            set { _csReaderFunctionName = value; }
        }
        #endregion

        #region ctor/init/cleanup
        /// <summary>
        /// default Ctor, mostly for serialization
        /// </summary>
        public DataType() { }

        public DataType(string sqlType, string csType)
        {
            _sqlTypeName = sqlType;
            _csTypeName = csType;
        }
        #endregion


    } 
    #endregion

    #region DataTypeCollection implementation
    [System.Xml.Serialization.XmlRoot("datatypes")]
    public class DataTypeCollection : System.Collections.Generic.List<DataType> { }
    #endregion

}
