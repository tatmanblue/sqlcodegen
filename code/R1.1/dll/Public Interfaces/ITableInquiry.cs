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
using BigWoo.Apps.SqlCodeGen.Classes;
#endregion

namespace BigWoo.Apps.SqlCodeGen.Interfaces
{
    /// <summary>
    /// In order for CRUD code generation to work, the program has to have an understanding of the data source structure.
    /// This interface defines the methods that should be implemented to build that understanding in the form of TableDefinition
    /// classes.
    /// </summary>
    public interface ITableInquiry
    {
        /// <summary>
        /// The connection string from the config file to this class.  It will already be formatted per the formatting.
        /// </summary>
        string ConnectionStr
        {
            get;
            set;
        }

        /// <summary>
        /// At this point each TableDefinitions contain nothing more than a table name.  This information was received from the command line.
        /// It is currently incomplete so its time to query the database to figure out the table schema and populate 
        /// that information into the TableDefinition.
        /// 
        /// The information gathered here becomes the source for code generation.
        /// </summary>
        /// <param name="tables">TableDefinitionCollection</param>
        void ProcessTables(TableDefinitionCollection tables);
    }
}
