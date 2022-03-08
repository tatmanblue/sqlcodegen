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

namespace BigWoo.Apps.SqlCodeGen
{
    /// <summary>
    /// if the ? was encountered in the command line, this exception will be thrown
    /// and need to be caught at the program main execution point
    /// </summary>
    internal class HelpException : Exception {}

    /// <summary>
    /// if the command line arguments are invalid in any way (format or missing)
    /// this exception will be thrown and needs to be handled at the 
    /// main exceution point
    /// </summary>
    internal class InvalidCommandLineException : Exception 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public InvalidCommandLineException(string msg) : base(msg) { }
    }
    
    /// <summary>
    /// exceptions of this nature mean that the program attempted to do something
    /// that made no sense and other mechanisms for finding this error failed to
    /// catch it.  A good example would be bad data in the configuration file got
    /// past the point of loading the configuration file.  I would think that
    /// this exception raised means the programmer did something wrong
    /// </summary>
    internal class ProgramFlowException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public ProgramFlowException(string msg) : base(msg) { }
    }
}
