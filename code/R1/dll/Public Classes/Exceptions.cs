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
#endregion

namespace MattRaffelNetCode.Apps.SqlCodeGen.Classes
{
    /// <summary>
    /// Indicates on the commandline that a table was specified but it was not
    /// found in the database
    /// </summary>
    public class MissingTableException : Exception 
    {
        public MissingTableException(string msg) : base(msg) { }
    }

    /// <summary>
    /// TypeLoaderException is thrown when one of the loader classes fails to load
    /// the type it intended to load
    /// </summary>
    public class TypeLoaderException : Exception
    {
        public TypeLoaderException(string msg) : base(msg) { }
    }
    
}

