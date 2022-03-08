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
#endregion

namespace BigWoo.Apps.SqlCodeGen.Interfaces
{
    /// <summary>
    /// While declared public, this interface is currently used with in the program only as there are no means
    /// for defining custom IDataTypeConversions implementations.
    /// </summary>
    public interface IDataTypeConversions
    {
        /// <summary>
        /// This method returns the method name used to read a column of the input type from a data reader.
        /// For example, column "ID" is an int, so we would use myReader.GetInt32(int colIndex) to read it.
        /// Thus, this method would return "GetInt32"
        ///
        /// This method is important for generating CRUD functionality. 
        /// </summary>
        /// <param name="sqlType">data type of a column</param>
        /// <returns>string, reader method used in C# to read the given type from a data reader</returns>
        string FindDataReaderFunc(string sqlType);

        /// <summary>
        /// This method returns the C# equivilant data type for a given database data type. 
        /// For example, column "ID" is an int, so we will use a int to represent this field in so.
        /// Thus, this method returns "int".
        ///
        /// This method is important for generating code
        /// </summary>
        /// <param name="sqlType">data type of a column</param>
        /// <returns></returns>
        string FindConversion(string sqlType);

    }
}
