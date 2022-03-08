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
using MattRaffelNetCode.Apps.SqlCodeGen.Classes;

#endregion
namespace MattRaffelNetCode.Apps.SqlCodeGen.Interfaces
{
    /// <summary>
    /// Data members generated for class typically follow some form of naming conventions.  Because these
    /// conventions vary across enivironments, there is no way the program can anticipate all of the needs. So
    /// this interface defines the methods that will be used to generate a variable name.  
    /// </summary>
    public interface IDataMemberNameFormatter
    {
        /// <summary>
        /// The method that does the formatting.  Use the ColumnDefinition and ColumnSubstitution classes
        /// to determine what to create.  
        /// </summary>
        /// <param name="definition">ColumnDefinition</param>
        /// <param name="substitution">ColumnSubstitution</param>
        /// <returns>string, variable name property formatted</returns>
        string MakeDataMemberName(ColumnDefinition definition, ColumnSubstitution substitution);
    }
}
